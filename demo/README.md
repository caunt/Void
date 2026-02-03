# caunt/void-demo (dind single-container)

This image runs **one visible container** on the host, and internally runs Docker (dind) to create per-session isolated environments.

## Architecture

The demo uses a Docker-in-Docker architecture with network isolation:

### Always-on containers (inside dind):
- **mc-server** (itzg/minecraft-server): Shared Minecraft server on backend network only
- **session-proxy**: HTTP reverse proxy and session manager

### Per-session containers (spawned on demand):
- **client-<sid>**: Minecraft client + noVNC on session network
- **proxy-<sid>**: TCP proxy forwarding traffic from client to mc-server (on both session and backend networks)

### Networks:
- **backend**: Shared private network for mc-server and all proxy-<sid> containers
- **sess-<sid>**: Per-session private network for client-<sid> and proxy-<sid>

### Traffic flow:
- Browser → session-proxy (HTTP) → client-<sid> noVNC endpoint
- client-<sid> → proxy-<sid> (Minecraft protocol on sess network)
- proxy-<sid> → mc-server (Minecraft protocol on backend network)

**Key security feature**: Clients cannot directly reach mc-server; all traffic flows through the isolated proxy.

## Requirements

Run the root container with **--privileged** (required for dind).

## Build

```bash
docker build -t caunt/void-demo:latest .
```

## Run (single container visible)

```bash
docker run --rm --privileged -p 8080:8080 caunt/void-demo:latest
```

Open:

- [**http://localhost:8080/**](http://localhost:8080/)

You will be redirected to:

- /s/<sessionId>/

## Behavior

- Sessions expire after **300 seconds** (configurable via SESSION_TTL_SECONDS env var)
- Containers are stopped with `docker stop --time 0`
- If a session container is still starting, you'll see a live "Starting session" page that polls status (~1 request / 1.5s) and only redirects when the client port is actually reachable
- If a session is expired, you'll see a live "Session expired" page that redirects to a new session

## Environment variables

- `LISTEN_ADDR`: HTTP listen address (default: 0.0.0.0:8080)
- `SESSION_TTL_SECONDS`: Session time-to-live in seconds (default: 300)
- `BACKEND_NETWORK`: Backend network name (default: backend)
- `CLIENT_IMAGE`: Client container image (default: client:latest)
- `PROXY_IMAGE`: TCP proxy container image (default: tcp-proxy:latest)
- `CLIENT_PORT`: Client noVNC port (default: 6080)

## Networking note

Because this is true dind, the root process is not a member of the child bridge networks. Each client container's port 6080 is published to the root container loopback as `127.0.0.1:<randomPort>`, and the session-proxy targets that.

### Boot waiting traffic

When the client is still starting, only the main HTML navigation gets the live waiting page. Non-HTML asset requests return a simple 503 text response so the browser doesn't spawn multiple waiting pages in parallel.
