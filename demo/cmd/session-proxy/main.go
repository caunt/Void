package main

import (
	"crypto/rand"
	"encoding/base64"
	"encoding/json"
	"errors"
	"fmt"
	"log"
	"net"
	"net/http"
	"net/http/httputil"
	"net/url"
	"os"
	"os/exec"
	"strconv"
	"strings"
	"sync"
	"time"
)

type Session struct {
	ID             string
	ClientName     string
	ProxyName      string
	SessionNetwork string
	ClientHostPort int
	CreatedUtc     time.Time
	ExpiresUtc     time.Time
	DeleteTimer    *time.Timer
	LastReadyCheckUtc time.Time
	LastReady      bool
}

type Server struct {
	listenAddr      string
	sessionTTL      time.Duration
	backendNetwork  string
	clientImage     string
	proxyImage      string
	clientPort      int

	sessionsLock sync.RWMutex
	sessions     map[string]*Session
}

func main() {
	server := &Server{
		listenAddr:     getEnvString("LISTEN_ADDR", "0.0.0.0:8080"),
		sessionTTL:     time.Duration(getEnvInt("SESSION_TTL_SECONDS", 300)) * time.Second,
		backendNetwork: getEnvString("BACKEND_NETWORK", "backend"),
		clientImage:    getEnvString("CLIENT_IMAGE", "client:latest"),
		proxyImage:     getEnvString("PROXY_IMAGE", "tcp-proxy:latest"),
		clientPort:     getEnvInt("CLIENT_PORT", 6080),
		sessions:       map[string]*Session{},
	}

	if err := server.ensureNetworkExists(server.backendNetwork); err != nil {
		log.Fatalf("Failed to ensure backend network exists: %v", err)
	}

	if err := server.ensureClientImageAvailable(); err != nil {
		log.Fatalf("Failed to ensure client image is available: %v", err)
	}

	mux := http.NewServeMux()
	mux.HandleFunc("/", server.handleRoot)
	mux.HandleFunc("/new", server.handleNewSession)
	mux.HandleFunc("/status/", server.handleStatus)
	mux.HandleFunc("/s/", server.handleSessionProxy)

	httpServer := &http.Server{
		Addr:              server.listenAddr,
		Handler:           withBasicLogging(mux),
		ReadHeaderTimeout: 10 * time.Second,
	}

	log.Printf("Listening on http://%s", server.listenAddr)
	log.Printf("Backend network: %s", server.backendNetwork)
	log.Printf("Client image: %s (port %d)", server.clientImage, server.clientPort)
	log.Printf("Proxy image: %s", server.proxyImage)
	log.Printf("Session TTL: %s", server.sessionTTL)

	if err := httpServer.ListenAndServe(); err != nil && !errors.Is(err, http.ErrServerClosed) {
		log.Fatalf("HTTP server failed: %v", err)
	}
}

func (server *Server) handleRoot(writer http.ResponseWriter, request *http.Request) {
	if request.URL.Path != "/" {
		http.NotFound(writer, request)
		return
	}

	server.handleNewSession(writer, request)
}

func (server *Server) handleNewSession(writer http.ResponseWriter, request *http.Request) {
	sessionId, err := createSessionId()
	if err != nil {
		http.Error(writer, "Failed to generate session id", http.StatusInternalServerError)
		return
	}

	hostPort, err := allocateFreeTcpPort()
	if err != nil {
		http.Error(writer, "Failed to allocate local port", http.StatusInternalServerError)
		return
	}

	sanitizedId := sanitizeForDockerName(sessionId)
	session := &Session{
		ID:             sessionId,
		ClientName:     "client-" + sanitizedId,
		ProxyName:      "proxy-" + sanitizedId,
		SessionNetwork: "sess-" + sanitizedId,
		ClientHostPort: hostPort,
		CreatedUtc:     time.Now().UTC(),
	}
	session.ExpiresUtc = session.CreatedUtc.Add(server.sessionTTL)

	if err := server.createSessionInfrastructure(session); err != nil {
		log.Printf("Failed to create session infrastructure for %s: %v", session.ID, err)
		server.writeLiveHtml(writer, http.StatusServiceUnavailable, "Starting session", "Failed to start session containers. Retrying…", "/new")
		return
	}

	server.sessionsLock.Lock()
	server.sessions[session.ID] = session
	server.sessionsLock.Unlock()

	session.DeleteTimer = time.AfterFunc(server.sessionTTL, func() {
		server.deleteSession(session.ID)
	})

	location := "/s/" + session.ID + "/"
	http.Redirect(writer, request, location, http.StatusTemporaryRedirect)
}

func (server *Server) handleStatus(writer http.ResponseWriter, request *http.Request) {
	sessionId := strings.TrimPrefix(request.URL.Path, "/status/")
	sessionId = strings.Trim(sessionId, "/")
	if sessionId == "" {
		http.NotFound(writer, request)
		return
	}

	server.sessionsLock.RLock()
	session, ok := server.sessions[sessionId]
	server.sessionsLock.RUnlock()

	type statusResponse struct {
		Exists      bool  `json:"exists"`
		Ready       bool  `json:"ready"`
		SessionId   string `json:"sessionId"`
		SecondsLeft int64  `json:"secondsLeft"`
	}

	response := statusResponse{
		Exists:    ok,
		SessionId: sessionId,
	}

	if ok {
		secondsLeft := time.Until(session.ExpiresUtc).Seconds()
		if secondsLeft < 0 {
			secondsLeft = 0
		}
		response.SecondsLeft = int64(secondsLeft)

		readyValue := server.isSessionReady(session)
		response.Ready = readyValue
	}

	writer.Header().Set("Content-Type", "application/json")
	_ = json.NewEncoder(writer).Encode(response)
}

func (server *Server) handleSessionProxy(writer http.ResponseWriter, request *http.Request) {
	// Path: /s/<sessionId>/<rest...>
	path := strings.TrimPrefix(request.URL.Path, "/s/")
	firstSlashIndex := strings.IndexByte(path, '/')
	if firstSlashIndex <= 0 {
		http.NotFound(writer, request)
		return
	}

	sessionId := path[:firstSlashIndex]
	restPath := path[firstSlashIndex:] // includes leading '/'

	server.sessionsLock.RLock()
	session, ok := server.sessions[sessionId]
	server.sessionsLock.RUnlock()

	if !ok {
		server.writeSessionExpiredHtml(writer, sessionId)
		return
	}

	targetUrl := &url.URL{
		Scheme: "http",
		Host:   fmt.Sprintf("127.0.0.1:%d", session.ClientHostPort),
	}

	reverseProxy := httputil.NewSingleHostReverseProxy(targetUrl)

	originalDirector := reverseProxy.Director
	reverseProxy.Director = func(proxyRequest *http.Request) {
		originalDirector(proxyRequest)
		proxyRequest.URL.Path = restPath
		proxyRequest.URL.RawPath = restPath
		proxyRequest.Host = targetUrl.Host
	}

	reverseProxy.ErrorHandler = func(proxyWriter http.ResponseWriter, proxyRequest *http.Request, proxyError error) {
		log.Printf("Session %s upstream not reachable yet: %v", sessionId, proxyError)

		acceptHeader := proxyRequest.Header.Get("Accept")
		isHtmlNavigation := strings.Contains(acceptHeader, "text/html") || acceptHeader == "" || acceptHeader == "*/*"
		if isHtmlNavigation && proxyRequest.Method == http.MethodGet {
			server.writeSessionStartingHtml(proxyWriter, sessionId)
			return
		}

		proxyWriter.Header().Set("Content-Type", "text/plain; charset=utf-8")
		proxyWriter.WriteHeader(http.StatusServiceUnavailable)
		_, _ = proxyWriter.Write([]byte("starting"))
	}

	reverseProxy.ServeHTTP(writer, request)
}


func (server *Server) isSessionReady(session *Session) bool {
	// Cache readiness for a short interval to avoid hammering.
	now := time.Now().UTC()

	server.sessionsLock.RLock()
	lastCheckUtc := session.LastReadyCheckUtc
	lastReady := session.LastReady
	server.sessionsLock.RUnlock()

	if !lastCheckUtc.IsZero() && now.Sub(lastCheckUtc) < 1*time.Second {
		return lastReady
	}

	address := fmt.Sprintf("127.0.0.1:%d", session.ClientHostPort)
	connection, err := net.DialTimeout("tcp", address, 200*time.Millisecond)
	readyValue := err == nil
	if connection != nil {
		_ = connection.Close()
	}

	server.sessionsLock.Lock()
	session.LastReadyCheckUtc = now
	session.LastReady = readyValue
	server.sessionsLock.Unlock()

	return readyValue
}


func (server *Server) writeSessionStartingHtml(writer http.ResponseWriter, sessionId string) {
	server.writeLiveHtml(writer, http.StatusOK, "Starting session", "Your session container is starting. Auto-refreshing…", "/s/"+sessionId+"/")
}

func (server *Server) writeSessionExpiredHtml(writer http.ResponseWriter, sessionId string) {
	server.writeLiveHtml(writer, http.StatusNotFound, "Session expired", "This session no longer exists. Redirecting to a new session…", "/")
}

func (server *Server) writeLiveHtml(writer http.ResponseWriter, statusCode int, title string, subtitle string, retryPath string) {
	writer.Header().Set("Content-Type", "text/html; charset=utf-8")
	writer.WriteHeader(statusCode)

	retryPathHtml := htmlEscape(retryPath)
	retryPathJs := strconv.Quote(retryPath)

	sessionId := ""
	if strings.HasPrefix(retryPath, "/s/") {
		trimmed := strings.TrimPrefix(retryPath, "/s/")
		trimmed = strings.Trim(trimmed, "/")
		firstSlashIndex := strings.IndexByte(trimmed, '/')
		if firstSlashIndex >= 0 {
			sessionId = trimmed[:firstSlashIndex]
		} else {
			sessionId = trimmed
		}
	}

	sessionIdJs := strconv.Quote(sessionId)

	page := fmt.Sprintf(`<!doctype html>
<html>
<head>
  <meta charset="utf-8"/>
  <meta name="viewport" content="width=device-width,initial-scale=1"/>
  <title>%s</title>
  <style>
    html,body{height:100%%;margin:0;background:#0d1117;color:#c9d1d9;font-family:ui-sans-serif,system-ui,-apple-system,Segoe UI,Roboto,Ubuntu,Cantarell,Noto Sans,sans-serif;}
    .wrap{height:100%%;display:flex;align-items:center;justify-content:center;padding:24px;box-sizing:border-box;}
    .card{max-width:720px;width:100%%;background:#161b22;border:1px solid #30363d;border-radius:16px;padding:22px;box-shadow:0 10px 30px rgba(0,0,0,.35);}
    h1{margin:0 0 10px 0;font-size:22px;font-weight:700;}
    p{margin:0 0 14px 0;line-height:1.5;color:#9da7b3;}
    .row{display:flex;gap:12px;flex-wrap:wrap;align-items:center;}
    .pill{display:inline-flex;align-items:center;gap:10px;padding:10px 12px;border-radius:999px;background:#0b1220;border:1px solid #30363d;}
    .dot{width:10px;height:10px;border-radius:50%%;background:#58a6ff;animation:pulse 1.2s infinite;}
    @keyframes pulse{0%%{transform:scale(1);opacity:.35}50%%{transform:scale(1.35);opacity:1}100%%{transform:scale(1);opacity:.35}}
    a{color:#58a6ff;text-decoration:none}
    a:hover{text-decoration:underline}
    code{background:#0b1220;border:1px solid #30363d;border-radius:8px;padding:2px 6px}
  </style>
</head>
<body>
  <div class="wrap">
    <div class="card">
      <h1>%s</h1>
      <p>%s</p>
      <div class="row">
        <div class="pill"><span class="dot"></span><span id="statusText">Checking…</span></div>
        <div class="pill"><span>Retry:</span> <a href="%s" id="retryLink">%s</a></div>
      </div>
      <p style="margin-top:14px;font-size:13px;color:#6e7681">
        This page auto-checks session status and will switch to the live session when it becomes reachable.
      </p>
    </div>
  </div>
<script>
(function(){
  const retryPath = %s;
  const sessionId = %s;
  const statusTextElement = document.getElementById("statusText");

  if (!statusTextElement){
    return;
  }

  if (typeof retryPath !== "string" || !retryPath.startsWith("/s/") || !sessionId){
    statusTextElement.textContent = "Ready";
    return;
  }

  const redirectOnceKey = "session_redirected_once:" + sessionId;

  let isTickRunning = false;

  function scheduleNextTick(){
    setTimeout(tick, 1000);
  }

  async function tick(){
    if (isTickRunning){
      scheduleNextTick();
      return;
    }

    isTickRunning = true;

    try{
      const abortController = new AbortController();
      const abortTimeoutId = setTimeout(function(){ abortController.abort(); }, 900);

      const response = await fetch("/status/" + sessionId, { cache: "no-store", signal: abortController.signal });
      clearTimeout(abortTimeoutId);

      if (!response.ok){
        statusTextElement.textContent = "Waiting…";
        return;
      }

      const status = await response.json();

      if (!status.exists){
        statusTextElement.textContent = "Session not found";
        sessionStorage.removeItem(redirectOnceKey);
        setTimeout(function(){ location.replace("/"); }, 1200);
        return;
      }

      if (status.ready){
        statusTextElement.textContent = "Session live (" + status.secondsLeft + "s left)";

        if (sessionStorage.getItem(redirectOnceKey) === "1"){
          return;
        }

        sessionStorage.setItem(redirectOnceKey, "1");

        if (location.pathname === retryPath){
          setTimeout(function(){ location.reload(); }, 150);
          return;
        }

        setTimeout(function(){ location.replace(retryPath); }, 150);
        return;
      }

      statusTextElement.textContent = "Starting… (" + status.secondsLeft + "s left)";
    } catch {
      statusTextElement.textContent = "Waiting…";
    } finally {
      isTickRunning = false;
      scheduleNextTick();
    }
  }

  tick();
})();
</script>

</body>
</html>`, htmlEscape(title), htmlEscape(title), htmlEscape(subtitle), retryPathHtml, retryPathHtml, retryPathJs, sessionIdJs)

	_, _ = writer.Write([]byte(page))
}

func htmlEscape(value string) string {
	replacer := strings.NewReplacer(
		`&`, "&amp;",
		`<`, "&lt;",
		`>`, "&gt;",
		`"`, "&quot;",
		`'`, "&#39;",
	)
	return replacer.Replace(value)
}

func (server *Server) deleteSession(sessionId string) {
	server.sessionsLock.Lock()
	session, ok := server.sessions[sessionId]
	if ok {
		delete(server.sessions, sessionId)
	}
	server.sessionsLock.Unlock()

	if !ok {
		return
	}

	log.Printf("Deleting session %s (containers %s, %s, network %s)", session.ID, session.ClientName, session.ProxyName, session.SessionNetwork)

	stopError := server.stopContainer(session.ClientName)
	if stopError != nil {
		log.Printf("Failed to stop container %s: %v", session.ClientName, stopError)
	}

	stopError = server.stopContainer(session.ProxyName)
	if stopError != nil {
		log.Printf("Failed to stop container %s: %v", session.ProxyName, stopError)
	}

	removeError := server.removeNetwork(session.SessionNetwork)
	if removeError != nil {
		log.Printf("Failed to remove network %s: %v", session.SessionNetwork, removeError)
	}
}

func (server *Server) ensureNetworkExists(networkName string) error {
	inspect := dockerCommand("network", "inspect", networkName)
	inspect.Stdout = nil
	inspect.Stderr = nil
	if err := inspect.Run(); err == nil {
		return nil
	}

	create := dockerCommand("network", "create", networkName)
	outputBytes, err := create.CombinedOutput()
	if err != nil {
		return fmt.Errorf("docker network create failed: %v: %s", err, string(outputBytes))
	}

	return nil
}

func (server *Server) ensureClientImageAvailable() error {
	inspect := dockerCommand("image", "inspect", server.clientImage)
	inspect.Stdout = nil
	inspect.Stderr = nil
	if err := inspect.Run(); err == nil {
		return nil
	}

	userspaceDirectory := "/opt/userspace"
	if _, err := os.Stat(userspaceDirectory); err != nil {
		return fmt.Errorf("userspace directory not found at %s: %v", userspaceDirectory, err)
	}

	log.Printf("Client image %s not found. Building from %s/client ...", server.clientImage, userspaceDirectory)

	build := dockerCommand("build", "-t", server.clientImage, userspaceDirectory+"/client")
	outputBytes, err := build.CombinedOutput()
	if err != nil {
		return fmt.Errorf("docker build failed: %v: %s", err, string(outputBytes))
	}

	return nil
}

func (server *Server) createSessionInfrastructure(session *Session) error {
	if err := server.ensureNetworkExists(session.SessionNetwork); err != nil {
		return fmt.Errorf("failed to create session network: %v", err)
	}

	if err := server.startProxyContainer(session); err != nil {
		_ = server.removeNetwork(session.SessionNetwork)
		return fmt.Errorf("failed to start proxy container: %v", err)
	}

	if err := server.startClientContainer(session); err != nil {
		_ = server.stopContainer(session.ProxyName)
		_ = server.removeNetwork(session.SessionNetwork)
		return fmt.Errorf("failed to start client container: %v", err)
	}

	return nil
}

func (server *Server) startProxyContainer(session *Session) error {
	args := []string{
		"run",
		"-d",
		"--rm",
		"--name", session.ProxyName,
		"--network", session.SessionNetwork,
		"--network-alias", "proxy",
		"-e", "UPSTREAM_HOST=mc-server",
		"-e", "UPSTREAM_PORT=25565",
		server.proxyImage,
	}

	outputBytes, err := dockerCommand(args...).CombinedOutput()
	if err != nil {
		return fmt.Errorf("docker run proxy failed: %v: %s", err, string(outputBytes))
	}

	connectArgs := []string{
		"network",
		"connect",
		server.backendNetwork,
		session.ProxyName,
	}

	outputBytes, err = dockerCommand(connectArgs...).CombinedOutput()
	if err != nil {
		return fmt.Errorf("docker network connect failed: %v: %s", err, string(outputBytes))
	}

	return nil
}

func (server *Server) startClientContainer(session *Session) error {
	args := []string{
		"run",
		"-d",
		"--rm",
		"--name", session.ClientName,
		"--network", session.SessionNetwork,
		"-p", fmt.Sprintf("127.0.0.1:%d:%d", session.ClientHostPort, server.clientPort),
		"--shm-size", "512m",
		"-e", "SERVER=proxy",
		server.clientImage,
	}

	outputBytes, err := dockerCommand(args...).CombinedOutput()
	if err != nil {
		return fmt.Errorf("docker run client failed: %v: %s", err, string(outputBytes))
	}

	return nil
}

func (server *Server) removeNetwork(networkName string) error {
	outputBytes, err := dockerCommand("network", "rm", networkName).CombinedOutput()
	if err != nil {
		if strings.Contains(string(outputBytes), "not found") {
			return nil
		}

		return fmt.Errorf("docker network rm failed: %v: %s", err, string(outputBytes))
	}

	return nil
}

func (server *Server) stopContainer(containerName string) error {
	outputBytes, err := dockerCommand("stop", "--time", "0", containerName).CombinedOutput()
	if err != nil {
		if strings.Contains(string(outputBytes), "No such container") {
			return nil
		}

		return fmt.Errorf("docker stop failed: %v: %s", err, string(outputBytes))
	}

	return nil
}


func allocateFreeTcpPort() (int, error) {
	listener, err := net.Listen("tcp", "127.0.0.1:0")
	if err != nil {
		return 0, err
	}

	defer listener.Close()

	address := listener.Addr().String()
	_, portString, err := net.SplitHostPort(address)
	if err != nil {
		return 0, err
	}

	portValue, err := strconv.Atoi(portString)
	if err != nil {
		return 0, err
	}

	return portValue, nil
}

func createSessionId() (string, error) {
	randomBytes := make([]byte, 64)
	if _, err := rand.Read(randomBytes); err != nil {
		return "", err
	}

	return base64.RawURLEncoding.EncodeToString(randomBytes), nil
}

func sanitizeForDockerName(input string) string {
	lowered := strings.ToLower(input)
	builder := strings.Builder{}
	builder.Grow(len(lowered))

	for _, ch := range lowered {
		if (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9') {
			builder.WriteRune(ch)
			continue
		}

		if ch == '.' || ch == '_' || ch == '-' {
			builder.WriteRune(ch)
			continue
		}

		builder.WriteByte('-')
	}

	output := builder.String()
	output = strings.Trim(output, ".-_")
	if output == "" {
		return "session"
	}

	if len(output) > 60 {
		output = output[:60]
	}

	return output
}

func dockerCommand(arguments ...string) *exec.Cmd {
	command := exec.Command("docker", arguments...)
	command.Env = os.Environ()
	return command
}

func withBasicLogging(next http.Handler) http.Handler {
	return http.HandlerFunc(func(writer http.ResponseWriter, request *http.Request) {
		started := time.Now()

		next.ServeHTTP(writer, request)

		remoteHost, _, _ := net.SplitHostPort(request.RemoteAddr)
		if remoteHost == "" {
			remoteHost = request.RemoteAddr
		}

		log.Printf("%s %s %s %s", request.Method, request.URL.Path, remoteHost, time.Since(started).Truncate(time.Millisecond))
	})
}

func getEnvString(name string, defaultValue string) string {
	value := strings.TrimSpace(os.Getenv(name))
	if value == "" {
		return defaultValue
	}

	return value
}

func getEnvInt(name string, defaultValue int) int {
	value := strings.TrimSpace(os.Getenv(name))
	if value == "" {
		return defaultValue
	}

	parsedValue, err := strconv.Atoi(value)
	if err != nil {
		return defaultValue
	}

	return parsedValue
}
