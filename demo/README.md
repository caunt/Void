# caunt/void-demo (dind single-container)

This image runs **one visible container** on the host, and internally runs Docker (dind) to create per-session child containers.

## Architecture

The demo uses a **shared Minecraft server** architecture:

### Always-on (shared across all sessions)
- **itzg-server**: Single Minecraft server container on `backend` network
  - Shared by all concurrent demo sessions
  - Never stopped between sessions

### Per-session (created for each visitor)
- **dashboard-<sid>**: Dashboard HTTP service showing 3 iframes
  - Connected to both `sess-<sid>` and `backend` networks
- **void-proxy-<sid>**: Void proxy instance
  - Connected to both `sess-<sid>` (as alias "proxy") and `backend` networks
  - Routes Minecraft traffic from client to shared itzg-server
- **client-<sid>**: noVNC Minecraft client
  - Only connected to `sess-<sid>` network
  - **Cannot** reach itzg-server directly (network isolation)

### Network topology
```
client-<sid> ──sess-<sid>──> void-proxy-<sid> ──backend──> itzg-server (shared)
                                    ↓
                            dashboard-<sid> ──backend──> itzg-server (shared)
```

### Key isolation rules
- Clients can only connect to their session's proxy (via "proxy" alias)
- Clients cannot reach the shared Minecraft server directly
- All traffic flows through the per-session Void proxy

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

- http://localhost:8080/

You will be redirected to:

- /s/<sessionId>/

## Behavior

- Sessions expire after **300 seconds** and per-session containers are stopped with `docker stop --time 0`.
- The shared itzg-server container persists between sessions.
- If a session container is still starting, you'll see a live "Starting session" page that polls status (~1 request / 1.5s) and only redirects when the dashboard port is actually reachable.
- If a session is expired, you'll see a live "Session expired" page that redirects to a new session.


## Networking note

Because this is true dind, the root process is not a member of the child bridge networks. Each dashboard container's port 8080 is published to the root container loopback as `127.0.0.1:<randomPort>`, and the session proxy targets that.

The dashboard uses nginx template substitution to dynamically route:
- `/void/` → session-specific void-proxy-<sid> terminal
- `/itzg/` → shared itzg-server terminal (same for everyone)
- `/vnc/` → session-specific client-<sid> noVNC


### Boot waiting traffic

When the child is still starting, only the main HTML navigation gets the live waiting page. Non-HTML asset requests return a simple 503 text response so the browser doesn't spawn multiple waiting pages in parallel.
