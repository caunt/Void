package main

import (
	"bytes"
	"crypto/rand"
	"encoding/base64"
	"encoding/json"
	"errors"
	"fmt"
	"html"
	"io"
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

type LogPrefixWriter struct {
	Prefix string
	Buffer strings.Builder
	Mutex  sync.Mutex
}

type Session struct {
	Id            string
	SanitizedId   string
	DashboardHost string
	ClientHost    string
	VoidHost      string
	CreatedUtc    time.Time
	ExpiresUtc    time.Time
	DeleteTimer   *time.Timer
}

type Server struct {
	ListenAddress string
	SessionTtl    time.Duration
	RedirectLogs  bool

	Sessions map[string]*Session
}

func main() {
	server := &Server{
		SessionTtl:    time.Duration(getEnvInt("SESSION_TTL_SECONDS", 7200)) * time.Second,
		ListenAddress: getEnvString("LISTEN_ADDRESS", "0.0.0.0:80"),
		RedirectLogs:  getEnvBool("REDIRECT_LOGS", false),
		Sessions:      map[string]*Session{},
	}

	if output, err := dockerCommand("network", "prune", "-f").Output(); err != nil {
		log.Fatalf("Failed to prune docker networks: %v. Output: %s", err, string(output))
	}

	if err := server.ensureSessionImagesAvailable(); err != nil {
		log.Fatalf("Failed to ensure session images are available: %v", err)
	}

	mux := http.NewServeMux()
	mux.HandleFunc("/", func(writer http.ResponseWriter, request *http.Request) {
		if request.URL.Path != "/" {
			http.NotFound(writer, request)
			return
		}

		server.handleNewSession(writer, request)
	})
	mux.HandleFunc("/status/", server.handleStatus)
	mux.HandleFunc("/session/", server.handleSession)

	httpServer := &http.Server{
		Addr:              server.ListenAddress,
		Handler:           withBasicLogging(mux),
		ReadHeaderTimeout: 10 * time.Second,
	}

	log.Printf("Listening on http://%s", server.ListenAddress)
	log.Printf("Session TTL: %s", server.SessionTtl)
	log.Printf("Redirect logs: %v", server.RedirectLogs)

	if err := httpServer.ListenAndServe(); err != nil && !errors.Is(err, http.ErrServerClosed) {
		log.Fatalf("HTTP server failed: %v", err)
	}
}

func (server *Server) handleNewSession(writer http.ResponseWriter, request *http.Request) {
	sessionId, err := createSessionId()
	if err != nil {
		http.Error(writer, "Failed to generate session id", http.StatusInternalServerError)
		return
	}

	session := &Session{Id: sessionId}

	session.SanitizedId = sanitizeForDockerName(sessionId)
	session.CreatedUtc = time.Now().UTC()
	session.ExpiresUtc = session.CreatedUtc.Add(server.SessionTtl)

	server.Sessions[session.Id] = session
	http.Redirect(writer, request, "/session/"+session.Id+"/", http.StatusTemporaryRedirect)

	go func() {
		startedSuccessfully := false
		defer func() {
			if startedSuccessfully {
				return
			}

			delete(server.Sessions, session.Id)
		}()

		log.Printf("Creating new session %s (TTL: %s)", session.Id, server.SessionTtl)

		if err := server.startSessionContainers(session); err != nil {
			log.Printf("Failed to start session containers for session %s: %v", session.Id, err)
			return
		}

		session.DeleteTimer = time.AfterFunc(server.SessionTtl, func() {
			err := server.deleteSession(session.Id)
			if err != nil {
				log.Printf("Failed to delete session on timer %s: %v", session.Id, err)
			}
		})

		startedSuccessfully = true
		log.Printf("Session %s created successfully", session.Id)
	}()
}

func (server *Server) handleStatus(writer http.ResponseWriter, request *http.Request) {
	sessionId := strings.TrimPrefix(request.URL.Path, "/status/")
	sessionId = strings.Trim(sessionId, "/")
	if sessionId == "" {
		http.NotFound(writer, request)
		return
	}

	session, ok := server.Sessions[sessionId]

	type statusResponse struct {
		Exists      bool   `json:"exists"`
		Ready       bool   `json:"ready"`
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

func (server *Server) handleSession(writer http.ResponseWriter, request *http.Request) {
	// Path: /session/<sessionId>[/<rest...>]
	path := strings.TrimPrefix(request.URL.Path, "/session/")
	firstSlashIndex := strings.IndexByte(path, '/')

	var sessionId string
	var restPath string

	switch firstSlashIndex {
	case -1:
		sessionId = path
		restPath = ""
	case 0:
		http.NotFound(writer, request)
		return
	default:
		sessionId = path[:firstSlashIndex]
		restPath = path[firstSlashIndex:] // includes leading '/'
	}

	if sessionId == "" {
		http.NotFound(writer, request)
		return
	}

	session, ok := server.Sessions[sessionId]

	if !ok {
		server.writeSessionExpiredHtml(writer)
		return
	}

	targetUrl := &url.URL{
		Scheme: "http",
		Host:   session.DashboardHost,
	}

	reverseProxy := httputil.NewSingleHostReverseProxy(targetUrl)

	originalDirector := reverseProxy.Director
	reverseProxy.Director = func(proxyRequest *http.Request) {
		originalDirector(proxyRequest)

		proxyRequest.URL.Path = restPath
		proxyRequest.URL.RawPath = restPath
		proxyRequest.Host = targetUrl.Host
		proxyRequest.Header.Set("X-Path-Prefix", "session/"+sessionId)
	}

	reverseProxy.ErrorHandler = func(proxyWriter http.ResponseWriter, proxyRequest *http.Request, proxyError error) {
		log.Printf("Session %s upstream not reachable yet: %v", sessionId, proxyError)
		server.writeSessionStartingHtml(proxyWriter, sessionId)

		proxyWriter.Header().Set("Content-Type", "text/plain; charset=utf-8")
		proxyWriter.WriteHeader(http.StatusServiceUnavailable)
		_, _ = proxyWriter.Write([]byte("starting"))
	}

	if !server.isSessionReady(session) {
		reverseProxy.ErrorHandler(writer, request, fmt.Errorf("session %s not ready", sessionId))
	} else {
		reverseProxy.ServeHTTP(writer, request)
	}
}

func (server *Server) isSessionReady(session *Session) bool {
	httpClient := &http.Client{
		Timeout: time.Second,
	}

	probe := func(host string) bool {
		request, err := http.NewRequest(http.MethodGet, "http://"+host+"/", nil)
		if err != nil {
			log.Printf("Failed to create probe request for host %s: %v", host, err)
			return false
		}

		response, err := httpClient.Do(request)
		if err != nil {
			log.Printf("Probe request to host %s failed: %v", host, err)
			return false
		}

		_ = response.Body.Close()

		log.Printf("Probed %s: %d", host, response.StatusCode)

		return response.StatusCode == http.StatusOK
	}

	return probe(session.DashboardHost) && probe(session.VoidHost) && probe(session.ClientHost)
}

func (server *Server) writeSessionStartingHtml(writer http.ResponseWriter, sessionId string) {
	server.writeLiveHtml(writer, http.StatusOK, "Starting session", "Your session container is starting", "/session/"+sessionId+"/")
}

func (server *Server) writeSessionExpiredHtml(writer http.ResponseWriter) {
	server.writeLiveHtml(writer, http.StatusNotFound, "Session expired", "This session no longer exists", "/")
}

func (server *Server) writeLiveHtml(writer http.ResponseWriter, statusCode int, title string, subtitle string, retryPath string) {
	writer.Header().Set("Content-Type", "text/html; charset=utf-8")
	writer.WriteHeader(statusCode)

	sessionId := ""
	trimmed := strings.TrimPrefix(retryPath, "/session/")
	if trimmed != retryPath {
		trimmed = strings.TrimLeft(trimmed, "/")

		firstSlashIndex := strings.IndexByte(trimmed, '/')
		if firstSlashIndex == -1 {
			sessionId = trimmed
		} else if firstSlashIndex > 0 {
			sessionId = trimmed[:firstSlashIndex]
		}
	}

	sessionIdJs := strconv.Quote(sessionId)
	retryPathJs := strconv.Quote(retryPath)

	titleHtml := html.EscapeString(title)
	subtitleHtml := html.EscapeString(subtitle)

	page := fmt.Sprintf(`<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8"/>
  <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover"/>
  <title>%s</title>
  <style>
    :root {
      --bg-color: #0f0b1e;
      --card-bg: #1a162d;
      --card-border: #4c2f7a;
      --text-main: #e9d5ff;
      --text-muted: #a39eb5;
      --accent: #d946ef; /* Neon Purple/Pink */
      --accent-glow: rgba(217, 70, 239, 0.4);
      --pill-bg: #281f3f;
    }

    html, body {
      height: 100%%;
      margin: 0;
      background-color: var(--bg-color);
      /* Subtle purple gradient background */
      background-image: radial-gradient(circle at 50%% 0%%, #2e1065 0%%, #0f0b1e 75%%);
      color: var(--text-main);
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif;
      -webkit-font-smoothing: antialiased;
    }

    .wrap {
      min-height: 100%%;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 20px;
      box-sizing: border-box;
    }

    .card {
      width: 100%%;
      max-width: 680px;
      background: var(--card-bg);
      border: 1px solid var(--card-border);
      border-radius: 20px;
      padding: 32px;
      /* Modern purple shadow */
      box-shadow: 0 10px 40px -10px rgba(0, 0, 0, 0.5), 0 0 20px -5px rgba(124, 58, 237, 0.2);
      backdrop-filter: blur(10px);
    }

    h1 {
      margin: 0 0 16px 0;
      font-size: 26px;
      font-weight: 700;
      color: #fff;
      letter-spacing: -0.5px;
    }

    p {
      margin: 0 0 20px 0;
      line-height: 1.6;
      color: var(--text-muted);
      font-size: 16px;
    }

    .row {
      display: flex;
      gap: 12px;
      flex-wrap: wrap;
      align-items: center;
    }

    .pill {
      display: inline-flex;
      align-items: center;
      gap: 12px;
      padding: 10px 16px;
      border-radius: 999px;
      background: var(--pill-bg);
      border: 1px solid var(--card-border);
      font-weight: 500;
      font-size: 14px;
      color: var(--text-main);
      transition: border-color 0.2s;
    }

    .dot {
      width: 10px;
      height: 10px;
      border-radius: 50%%;
      background: var(--accent);
      box-shadow: 0 0 10px var(--accent);
      animation: pulse 2s infinite cubic-bezier(0.4, 0, 0.6, 1);
    }

    @keyframes pulse {
      0%% { opacity: 1; transform: scale(1); box-shadow: 0 0 0 0 var(--accent-glow); }
      50%% { opacity: .7; transform: scale(1.1); box-shadow: 0 0 0 6px rgba(0,0,0,0); }
      100%% { opacity: 1; transform: scale(1); box-shadow: 0 0 0 0 rgba(0,0,0,0); }
    }

    .footer-text {
      margin-top: 24px;
      font-size: 13px;
      color: #6b7280;
      border-top: 1px solid rgba(255,255,255,0.05);
      padding-top: 16px;
    }

    /* Mobile Tweaks */
    @media (max-width: 480px) {
      .card { padding: 24px; }
      h1 { font-size: 22px; }
      p { font-size: 15px; }
    }
  </style>
</head>
<body>
  <div class="wrap">
    <div class="card">
      <h1>%s</h1>
      <p>%s</p>
      
      <div class="row">
        <div class="pill">
          <span class="dot"></span>
          <span id="statusText">Checking status…</span>
        </div>
      </div>

      <p class="footer-text">
        System is auto-checking session connectivity. You will be redirected automatically once the live session is reachable.
      </p>
    </div>
  </div>

<script>
(function() {
  // Go variables injected here
  const retryPath = %s;
  const sessionId = %s;
  
  const statusTextElement = document.getElementById("statusText");

  if (!statusTextElement) {
    console.error("Critical: Status text element missing.");
    return;
  }

  // Direct redirection if no session ID implies immediate retry/login needed
  if (!sessionId && retryPath) {
    location.replace(retryPath);
    return;
  }

  async function tick() {
    try {
      // Added timestamp to prevent aggressive browser caching issues
      const timestamp = new Date().getTime();
      const response = await fetch("/status/" + sessionId + "?t=" + timestamp, { 
        cache: "no-store",
        headers: { 'Cache-Control': 'no-cache' }
      });

      if (!response.ok) return;

      const status = await response.json();

      // Session no longer exists -> go home
      if (!status.exists) {
        location.replace("/");
        return;
      }

      // Session is ready -> go to application
      if (status.ready) {
        statusTextElement.textContent = "Session Ready! Redirecting...";
        statusTextElement.style.color = "#a7f3d0"; // Little visual green hint
        location.replace(retryPath);
        return;
      }

      statusTextElement.textContent = "Starting environment...";
      
    } catch(error) {
      console.warn("Status check failed, retrying...", error);
    } finally {
      setTimeout(tick, 1000);
    }
  }

  tick();
})();
</script>
</body>
</html>`, titleHtml, titleHtml, subtitleHtml, retryPathJs, sessionIdJs)

	_, _ = writer.Write([]byte(page))
}

func (server *Server) deleteSession(sessionId string) error {
	session, ok := server.Sessions[sessionId]
	if ok {
		delete(server.Sessions, sessionId)
	}

	if !ok {
		return nil
	}

	log.Printf("Deleting session %s", session.Id)

	err := server.stopSession(session)
	if err != nil {
		return err
	}

	log.Printf("Session %s cleanup completed", session.Id)
	return nil
}

func (server *Server) stopSession(session *Session) error {
	stopOutput, stopErr := dockerCommand("compose", "--project-name", session.SanitizedId, "--file", "session.yml", "down", "--remove-orphans", "--volumes").CombinedOutput()
	if stopErr != nil {
		return fmt.Errorf("docker compose down failed: %v: %s", stopErr, string(stopOutput))
	}
	return nil
}

func (server *Server) ensureSessionImagesAvailable() error {
	build := dockerCommand("compose", "--file", "session.yml", "build")

	build.Stdout = os.Stdout
	build.Stderr = os.Stderr
	build.Stdin = os.Stdin

	err := build.Run()
	if err != nil {
		return fmt.Errorf("docker compose build failed: %v", err)
	}
	return nil
}

func (server *Server) startSessionContainers(session *Session) (returnedError error) {
	log.Printf("Session %s: Starting containers", session.Id)

	defer func() {
		if returnedError != nil {
			server.stopSession(session)
		}
	}()

	startOutputBytes, startError := dockerCommand("compose", "--project-name", session.SanitizedId, "--file", "session.yml", "up", "--build", "--detach").CombinedOutput()
	if startError != nil {
		log.Printf("Session %s: Failed to start containers with docker compose: %v, output: %s", session.Id, startError, string(startOutputBytes))
		return fmt.Errorf("failed to start containers with docker compose: %v: %s", startError, string(startOutputBytes))
	}

	log.Printf("Session %s: Containers started with docker compose", session.Id)

	containerIdBytes, listContainersError := dockerCommand("compose", "--project-name", session.SanitizedId, "--file", "session.yml", "ps", "-q").Output()
	if listContainersError != nil {
		log.Printf("Session %s: Failed to list compose containers: %v", session.Id, listContainersError)
		return fmt.Errorf("failed to list compose containers: %v", listContainersError)
	}

	containerIds := strings.Fields(string(containerIdBytes))
	if len(containerIds) == 0 {
		log.Printf("Session %s: No containers returned by docker compose ps -q", session.Id)
		return fmt.Errorf("no containers returned by docker compose ps -q")
	}

	inspectArguments := []string{
		"inspect",
		"--format",
		"{{.Name}} {{index .Config.Labels \"com.docker.compose.service\"}}",
	}
	inspectArguments = append(inspectArguments, containerIds...)

	inspectOutputBytes, inspectError := dockerCommand(inspectArguments...).CombinedOutput()
	if inspectError != nil {
		return fmt.Errorf("failed to inspect compose containers: %v: %s", inspectError, string(inspectOutputBytes))
	}

	dashboardContainerName := ""
	clientContainerName := ""
	voidContainerName := ""

	inspectLines := strings.SplitSeq(strings.TrimSpace(string(inspectOutputBytes)), "\n")
	for inspectLine := range inspectLines {
		inspectFields := strings.Fields(inspectLine)
		if len(inspectFields) < 2 {
			continue
		}

		containerName := strings.TrimPrefix(strings.TrimSpace(inspectFields[0]), "/")
		composeServiceName := strings.TrimSpace(inspectFields[1])

		switch composeServiceName {
		case "dashboard":
			dashboardContainerName = containerName
		case "client":
			clientContainerName = containerName
		case "void":
			voidContainerName = containerName
		}

		if server.RedirectLogs {
			if containerName == "" {
				continue
			}

			server.streamContainerLogs(containerName)
		}
	}

	if strings.TrimSpace(dashboardContainerName) == "" {
		return fmt.Errorf("dashboard container name not found")
	}

	if strings.TrimSpace(clientContainerName) == "" {
		return fmt.Errorf("client container name not found")
	}

	if strings.TrimSpace(voidContainerName) == "" {
		return fmt.Errorf("void container name not found")
	}

	session.DashboardHost = dashboardContainerName
	session.ClientHost = clientContainerName
	session.VoidHost = voidContainerName

	return nil
}

func createSessionId() (string, error) {
	randomBytes := make([]byte, 32)
	if _, err := rand.Read(randomBytes); err != nil {
		return "", err
	}

	return base64.RawURLEncoding.EncodeToString(randomBytes), nil
}

func sanitizeForDockerName(input string) string {
	lowered := strings.ToLower(input)
	builder := strings.Builder{}
	builder.Grow(len(lowered))

	lastWasSeparator := true

	for _, ch := range lowered {
		if (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9') {
			builder.WriteRune(ch)
			lastWasSeparator = false
			continue
		}

		if !lastWasSeparator {
			builder.WriteByte('-')
			lastWasSeparator = true
		}
	}

	output := builder.String()
	output = strings.Trim(output, "-")
	if output == "" {
		return "session"
	}

	if len(output) > 60 {
		output = strings.Trim(output[:60], "-")
		if output == "" {
			return "session"
		}
	}

	return output
}

func dockerCommand(arguments ...string) *exec.Cmd {
	command := exec.Command("docker", arguments...)
	command.Env = os.Environ()
	return command
}

func (server *Server) streamContainerLogs(containerName string) {
	if !server.RedirectLogs {
		return
	}

	go func() {
		log.Printf("Starting log stream for container: %s", containerName)

		writer := &LogPrefixWriter{Prefix: "[" + containerName + "] "}

		logsCommand := dockerCommand("logs", "--follow", "--tail=0", containerName)

		var stderrBuffer bytes.Buffer
		logsCommand.Stdout = writer
		logsCommand.Stderr = io.MultiWriter(writer, &stderrBuffer)

		err := logsCommand.Run()
		if err == nil {
			return
		}

		stderrText := stderrBuffer.String()
		if strings.Contains(stderrText, "No such container") || strings.Contains(stderrText, "is not running") {
			return
		}

		log.Printf("Log streaming ended for %s: %v", containerName, err)
	}()
}

func (writer *LogPrefixWriter) Write(data []byte) (int, error) {
	writer.Mutex.Lock()
	defer writer.Mutex.Unlock()

	_, _ = writer.Buffer.Write(data)
	content := writer.Buffer.String()

	lines := strings.Split(content, "\n")

	if !strings.HasSuffix(content, "\n") {
		writer.Buffer.Reset()
		_, _ = writer.Buffer.WriteString(lines[len(lines)-1])
		lines = lines[:len(lines)-1]
	} else {
		writer.Buffer.Reset()
	}

	for _, line := range lines {
		if line != "" {
			log.Printf("%s%s", writer.Prefix, line)
		}
	}

	return len(data), nil
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
