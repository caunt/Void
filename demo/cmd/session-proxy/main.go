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
	ID                  string
	DashboardName       string
	VoidName            string
	ClientName          string
	SessionNetworkName  string
	DashboardHostPort   int
	CreatedUtc          time.Time
	ExpiresUtc          time.Time
	DeleteTimer         *time.Timer
	LastReadyCheckUtc   time.Time
	LastReady           bool
	LastNotReadyLogUtc  time.Time
}

type Server struct {
	listenAddr         string
	sessionTTL         time.Duration
	backendNetwork     string
	dashboardImage     string
	voidImage          string
	clientImage        string
	itzgImage          string
	sharedItzgStarted  bool
	sharedItzgLock     sync.Mutex

	sessionsLock sync.RWMutex
	sessions     map[string]*Session
}

func main() {
	server := &Server{
		listenAddr:        getEnvString("LISTEN_ADDR", "0.0.0.0:8080"),
		sessionTTL:        time.Duration(getEnvInt("SESSION_TTL_SECONDS", 7200)) * time.Second,
		backendNetwork:    getEnvString("BACKEND_NETWORK", "backend"),
		dashboardImage:    getEnvString("DASHBOARD_IMAGE", "dashboard:latest"),
		voidImage:         getEnvString("VOID_IMAGE", "terminal-void:latest"),
		clientImage:       getEnvString("CLIENT_IMAGE", "client:latest"),
		itzgImage:         getEnvString("ITZG_IMAGE", "terminal-itzg:latest"),
		sharedItzgStarted: false,
		sessions:          map[string]*Session{},
	}

	if err := server.ensureBackendNetworkExists(); err != nil {
		log.Fatalf("Failed to ensure backend network exists: %v", err)
	}

	if err := server.ensureImagesAvailable(); err != nil {
		log.Fatalf("Failed to ensure images are available: %v", err)
	}

	if err := server.ensureSharedItzgRunning(); err != nil {
		log.Fatalf("Failed to ensure shared itzg server is running: %v", err)
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
	log.Printf("Dashboard image: %s", server.dashboardImage)
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

	dashboardHostPort, err := allocateFreeTcpPort()
	if err != nil {
		http.Error(writer, "Failed to allocate local port", http.StatusInternalServerError)
		return
	}

	sanitizedId := sanitizeForDockerName(sessionId)
	session := &Session{
		ID:                 sessionId,
		DashboardName:      "dashboard-" + sanitizedId,
		VoidName:           "void-proxy-" + sanitizedId,
		ClientName:         "client-" + sanitizedId,
		SessionNetworkName: "sess-" + sanitizedId,
		DashboardHostPort:  dashboardHostPort,
		CreatedUtc:         time.Now().UTC(),
	}
	session.ExpiresUtc = session.CreatedUtc.Add(server.sessionTTL)

	log.Printf("Creating new session %s (dashboard port: %d, TTL: %s)", session.ID, session.DashboardHostPort, server.sessionTTL)

	if err := server.startSessionContainers(session); err != nil {
		log.Printf("Failed to start session containers for session %s: %v", session.ID, err)
		server.writeLiveHtml(writer, http.StatusServiceUnavailable, "Starting session", "Failed to start session containers. Retrying…", "/new")
		return
	}

	server.sessionsLock.Lock()
	server.sessions[session.ID] = session
	server.sessionsLock.Unlock()

	session.DeleteTimer = time.AfterFunc(server.sessionTTL, func() {
		server.deleteSession(session.ID)
	})

	log.Printf("Session %s created successfully, redirecting to /s/%s/", session.ID, session.ID)
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
		Host:   fmt.Sprintf("127.0.0.1:%d", session.DashboardHostPort),
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
	lastNotReadyLogUtc := session.LastNotReadyLogUtc
	server.sessionsLock.RUnlock()

	if !lastCheckUtc.IsZero() && now.Sub(lastCheckUtc) < 1*time.Second {
		return lastReady
	}

	address := fmt.Sprintf("127.0.0.1:%d", session.DashboardHostPort)
	connection, err := net.DialTimeout("tcp", address, 200*time.Millisecond)
	readyValue := err == nil
	if connection != nil {
		_ = connection.Close()
	}

	server.sessionsLock.Lock()
	defer server.sessionsLock.Unlock()

	// Log state changes to help diagnose startup issues
	if !lastReady && readyValue {
		log.Printf("Session %s: Dashboard became ready on port %d", session.ID, session.DashboardHostPort)
	} else if lastReady && !readyValue {
		log.Printf("Session %s: Dashboard became unreachable on port %d", session.ID, session.DashboardHostPort)
	} else if !readyValue && !lastCheckUtc.IsZero() {
		// Only log every 10 seconds for "still not ready" to avoid spam
		if lastNotReadyLogUtc.IsZero() || now.Sub(lastNotReadyLogUtc) >= 10*time.Second {
			log.Printf("Session %s: Dashboard still not ready on port %d (error: %v)", session.ID, session.DashboardHostPort, err)
			session.LastNotReadyLogUtc = now
		}
	}

	session.LastReadyCheckUtc = now
	session.LastReady = readyValue

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

	log.Printf("Deleting session %s", session.ID)

	// Stop dashboard container
	if err := server.stopContainer(session.DashboardName); err != nil {
		log.Printf("Failed to stop dashboard container %s: %v", session.DashboardName, err)
	}

	// Stop void proxy container
	if err := server.stopContainer(session.VoidName); err != nil {
		log.Printf("Failed to stop void container %s: %v", session.VoidName, err)
	}

	// Stop client container
	if err := server.stopContainer(session.ClientName); err != nil {
		log.Printf("Failed to stop client container %s: %v", session.ClientName, err)
	}

	// Remove session network
	if err := server.removeNetwork(session.SessionNetworkName); err != nil {
		log.Printf("Failed to remove session network %s: %v", session.SessionNetworkName, err)
	}

	log.Printf("Session %s cleanup completed", session.ID)
}

func (server *Server) ensureBackendNetworkExists() error {
	inspect := dockerCommand("network", "inspect", server.backendNetwork)
	inspect.Stdout = nil
	inspect.Stderr = nil
	if err := inspect.Run(); err == nil {
		return nil
	}

	create := dockerCommand("network", "create", server.backendNetwork)
	outputBytes, err := create.CombinedOutput()
	if err != nil {
		return fmt.Errorf("docker network create failed: %v: %s", err, string(outputBytes))
	}

	log.Printf("Created backend network: %s", server.backendNetwork)
	return nil
}

func (server *Server) ensureImagesAvailable() error {
	userspaceDirectory := "/opt/userspace"
	if _, err := os.Stat(userspaceDirectory); err != nil {
		return fmt.Errorf("userspace directory not found at %s: %v", userspaceDirectory, err)
	}

	images := []struct {
		name      string
		buildPath string
	}{
		{server.dashboardImage, userspaceDirectory + "/dashboard"},
		{server.voidImage, userspaceDirectory + "/terminal-void"},
		{server.clientImage, userspaceDirectory + "/client"},
		{server.itzgImage, userspaceDirectory + "/terminal-itzg"},
	}

	for _, img := range images {
		inspect := dockerCommand("image", "inspect", img.name)
		inspect.Stdout = nil
		inspect.Stderr = nil
		if err := inspect.Run(); err == nil {
			continue
		}

		log.Printf("Image %s not found. Building from %s ...", img.name, img.buildPath)

		build := dockerCommand("build", "--platform", "linux/amd64", "-t", img.name, img.buildPath)
		outputBytes, err := build.CombinedOutput()
		if err != nil {
			return fmt.Errorf("docker build failed for %s: %v: %s", img.name, err, string(outputBytes))
		}

		log.Printf("Successfully built image: %s", img.name)
	}

	return nil
}

func (server *Server) ensureSharedItzgRunning() error {
	server.sharedItzgLock.Lock()
	defer server.sharedItzgLock.Unlock()

	if server.sharedItzgStarted {
		return nil
	}

	containerName := "itzg-server"

	// Check if itzg container is already running
	inspect := dockerCommand("inspect", "--format", "{{.State.Running}}", containerName)
	inspect.Stderr = nil
	outputBytes, err := inspect.Output()
	if err == nil && strings.TrimSpace(string(outputBytes)) == "true" {
		log.Printf("Shared itzg server already running: %s", containerName)
		server.sharedItzgStarted = true
		return nil
	}

	// Remove existing stopped container if any
	_ = dockerCommand("rm", "-f", containerName).Run()

	log.Printf("Starting shared itzg server: %s", containerName)

	args := []string{
		"run",
		"-d",
		"--rm",
		"--name", containerName,
		"--network", server.backendNetwork,
		"-e", "EULA=true",
		"-e", "TYPE=PAPER",
		"-e", "VERSION=LATEST",
		"-e", "ONLINE_MODE=false",
		"-e", "OVERRIDE_SERVER_PROPERTIES=true",
		"-e", "SERVER_PORT=25565",
		server.itzgImage,
	}

	outputBytes, err = dockerCommand(args...).CombinedOutput()
	if err != nil {
		return fmt.Errorf("failed to start shared itzg server: %v: %s", err, string(outputBytes))
	}

	server.sharedItzgStarted = true
	log.Printf("Shared itzg server started: %s", containerName)
	return nil
}

func (server *Server) createSessionNetwork(networkName string) error {
	create := dockerCommand("network", "create", networkName)
	outputBytes, err := create.CombinedOutput()
	if err != nil {
		return fmt.Errorf("docker network create failed: %v: %s", err, string(outputBytes))
	}

	log.Printf("Created session network: %s", networkName)
	return nil
}

func (server *Server) startSessionContainers(session *Session) error {
	log.Printf("Session %s: Starting container orchestration", session.ID)
	
	// 1. Create per-session network
	log.Printf("Session %s: Creating network %s", session.ID, session.SessionNetworkName)
	if err := server.createSessionNetwork(session.SessionNetworkName); err != nil {
		log.Printf("Session %s: Failed to create network: %v", session.ID, err)
		return fmt.Errorf("failed to create session network: %w", err)
	}

	// Set up cleanup on failure - remove network if we don't complete successfully
	cleanupNetwork := true
	defer func() {
		if cleanupNetwork {
			log.Printf("Session %s: Cleaning up network %s due to failure", session.ID, session.SessionNetworkName)
			_ = server.removeNetwork(session.SessionNetworkName)
		}
	}()

	// 2. Start void proxy container (connected to both networks)
	log.Printf("Session %s: Starting void-proxy container %s", session.ID, session.VoidName)
	voidArgs := []string{
		"run",
		"-d",
		"--rm",
		"--name", session.VoidName,
		"--network", server.backendNetwork,
		"-e", "ARGUMENTS=--offline --ignore-file-servers --server itzg-server",
		server.voidImage,
	}

	outputBytes, err := dockerCommand(voidArgs...).CombinedOutput()
	if err != nil {
		log.Printf("Session %s: Failed to start void container: %v, output: %s", session.ID, err, string(outputBytes))
		return fmt.Errorf("failed to start void container: %v: %s", err, string(outputBytes))
	}
	log.Printf("Session %s: Void-proxy container %s started", session.ID, session.VoidName)

	// Connect void to session network with alias "proxy"
	log.Printf("Session %s: Connecting void-proxy to session network with alias 'proxy'", session.ID)
	connectArgs := []string{
		"network", "connect",
		"--alias", "proxy",
		session.SessionNetworkName,
		session.VoidName,
	}

	outputBytes, err = dockerCommand(connectArgs...).CombinedOutput()
	if err != nil {
		log.Printf("Session %s: Failed to connect void to session network: %v, output: %s", session.ID, err, string(outputBytes))
		_ = server.stopContainer(session.VoidName)
		return fmt.Errorf("failed to connect void to session network: %v: %s", err, string(outputBytes))
	}
	log.Printf("Session %s: Void-proxy connected to session network", session.ID)

	// 3. Start client container (only on session network)
	log.Printf("Session %s: Starting client container %s", session.ID, session.ClientName)
	clientArgs := []string{
		"run",
		"-d",
		"--rm",
		"--name", session.ClientName,
		"--network", session.SessionNetworkName,
		"-e", "SERVER=proxy",
		server.clientImage,
	}

	outputBytes, err = dockerCommand(clientArgs...).CombinedOutput()
	if err != nil {
		log.Printf("Session %s: Failed to start client container: %v, output: %s", session.ID, err, string(outputBytes))
		_ = server.stopContainer(session.VoidName)
		return fmt.Errorf("failed to start client container: %v: %s", err, string(outputBytes))
	}
	log.Printf("Session %s: Client container %s started", session.ID, session.ClientName)

	// 4. Start dashboard container (connected to session network for routing)
	log.Printf("Session %s: Starting dashboard container %s on port %d", session.ID, session.DashboardName, session.DashboardHostPort)
	dashboardArgs := []string{
		"run",
		"-d",
		"--rm",
		"--name", session.DashboardName,
		"--network", session.SessionNetworkName,
		"-p", fmt.Sprintf("127.0.0.1:%d:8080", session.DashboardHostPort),
		"-e", fmt.Sprintf("VOID_CONTAINER_NAME=%s", session.VoidName),
		"-e", fmt.Sprintf("CLIENT_CONTAINER_NAME=%s", session.ClientName),
		server.dashboardImage,
	}

	outputBytes, err = dockerCommand(dashboardArgs...).CombinedOutput()
	if err != nil {
		log.Printf("Session %s: Failed to start dashboard container: %v, output: %s", session.ID, err, string(outputBytes))
		_ = server.stopContainer(session.VoidName)
		_ = server.stopContainer(session.ClientName)
		return fmt.Errorf("failed to start dashboard container: %v: %s", err, string(outputBytes))
	}
	log.Printf("Session %s: Dashboard container %s started", session.ID, session.DashboardName)

	// Wait briefly and verify dashboard container is still running before connecting to network
	time.Sleep(500 * time.Millisecond)
	
	inspectArgs := []string{"inspect", "--format", "{{.State.Running}}", session.DashboardName}
	inspectOutput, inspectErr := dockerCommand(inspectArgs...).Output()
	if inspectErr != nil || strings.TrimSpace(string(inspectOutput)) != "true" {
		log.Printf("Session %s: Dashboard container exited before network connection (running: %s, err: %v)", session.ID, strings.TrimSpace(string(inspectOutput)), inspectErr)
		
		// Get container logs to diagnose the issue
		logsCmd := dockerCommand("logs", "--tail", "50", session.DashboardName)
		if logsOutput, logsErr := logsCmd.CombinedOutput(); logsErr == nil {
			log.Printf("Session %s: Dashboard container logs:\n%s", session.ID, string(logsOutput))
		}
		
		_ = server.stopContainer(session.DashboardName)
		_ = server.stopContainer(session.VoidName)
		_ = server.stopContainer(session.ClientName)
		return fmt.Errorf("dashboard container exited immediately after start")
	}

	// Connect dashboard to backend network to reach shared itzg
	log.Printf("Session %s: Connecting dashboard to backend network", session.ID)
	connectArgs = []string{
		"network", "connect",
		server.backendNetwork,
		session.DashboardName,
	}

	outputBytes, err = dockerCommand(connectArgs...).CombinedOutput()
	if err != nil {
		log.Printf("Session %s: Failed to connect dashboard to backend network: %v, output: %s", session.ID, err, string(outputBytes))
		_ = server.stopContainer(session.DashboardName)
		_ = server.stopContainer(session.VoidName)
		_ = server.stopContainer(session.ClientName)
		return fmt.Errorf("failed to connect dashboard to backend network: %v: %s", err, string(outputBytes))
	}
	log.Printf("Session %s: Dashboard connected to backend network", session.ID)

	// Success - don't clean up the network
	cleanupNetwork = false
	log.Printf("Session %s: All containers started successfully (dashboard port: %d)", session.ID, session.DashboardHostPort)
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

func getEnvBool(name string, defaultValue bool) bool {
	value := strings.TrimSpace(os.Getenv(name))
	if value == "" {
		return defaultValue
	}

	valueLower := strings.ToLower(value)
	return valueLower == "1" || valueLower == "true" || valueLower == "yes" || valueLower == "on"
}
