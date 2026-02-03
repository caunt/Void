# Testing Guide for Shared itzg Server Architecture

## Architecture Summary

The demo now uses a **shared Minecraft server** that all sessions connect to, while maintaining session isolation for security.

### What Changed

**Before:**
- Each visitor got their own: dashboard, void proxy, client, AND itzg server
- All 4 containers created per session via docker-compose

**After:**
- Each visitor gets their own: dashboard, void proxy, client (per-session)
- All visitors share: ONE itzg-server container (shared)
- Containers created individually with proper network isolation

### Network Architecture

```
Session 1:
  client-<sid1> ──[sess-<sid1>]──> void-proxy-<sid1> ──┐
                                                        │
Session 2:                                              ├─[backend]─> itzg-server (shared)
  client-<sid2> ──[sess-<sid2>]──> void-proxy-<sid2> ──┘
```

**Key Isolation:**
- Each client runs in its own `sess-<sid>` network
- Client can ONLY reach its own void-proxy (via "proxy" alias)
- Client CANNOT reach itzg-server directly
- Void proxies connect to shared itzg-server via `backend` network

## How to Build and Test

### 1. Build the demo image

```bash
cd demo
docker build -t caunt/void-demo:latest .
```

### 2. Run the demo

```bash
docker run --rm --privileged -p 8080:8080 caunt/void-demo:latest
```

### 3. Open in browser

Navigate to: http://localhost:8080/

You will be redirected to: http://localhost:8080/s/<sessionId>/

### 4. Verify the dashboard

The dashboard should show 3 iframes:
- **Left top**: Void proxy terminal (per-session)
- **Left bottom**: itzg server terminal (SHARED - same for all users)
- **Right**: Minecraft client via noVNC (per-session)

### 5. Test with multiple sessions

Open the demo in multiple browser windows/tabs:
- Each will get a different session ID
- Each will see its own void proxy terminal (different)
- Each will see the SAME itzg server terminal (shared)
- Each will have its own Minecraft client

### 6. Verify network isolation

To verify that clients cannot reach itzg-server directly:

```bash
# Get session ID from URL: /s/<sessionId>/
SESSION_ID="<your-session-id>"

# Exec into the client container
docker exec -it client-<sanitized-session-id> sh

# Try to ping itzg-server (should fail - not in same network)
ping itzg-server

# Try to reach proxy (should work - in same network)
ping proxy
```

### 7. Verify shared itzg server

To verify the itzg server is shared:

```bash
# List all containers
docker ps

# You should see:
# - ONE itzg-server container
# - Multiple void-proxy-* containers (one per session)
# - Multiple client-* containers (one per session)
# - Multiple dashboard-* containers (one per session)

# Check itzg-server network
docker inspect itzg-server | grep -A 10 Networks

# Should show it's connected to "backend" network only
```

## Expected Behavior

1. **First session start:**
   - Creates `backend` network
   - Starts `itzg-server` container
   - Creates `sess-<sid>` network
   - Starts dashboard, void-proxy, client containers

2. **Second session start:**
   - Reuses existing `backend` network
   - Reuses existing `itzg-server` container (no restart)
   - Creates new `sess-<sid>` network
   - Starts new dashboard, void-proxy, client containers

3. **Session expiry:**
   - Stops and removes dashboard, void-proxy, client containers
   - Removes `sess-<sid>` network
   - KEEPS `itzg-server` running (shared)
   - KEEPS `backend` network (shared)

## Troubleshooting

### Dashboard shows blank iframes

Check container names match nginx config expectations:
```bash
# Dashboard expects these containers to exist:
# - VOID_CONTAINER_NAME (passed as env var)
# - CLIENT_CONTAINER_NAME (passed as env var)
# - itzg-server (hardcoded)

docker logs dashboard-<sanitized-session-id>
```

### Client cannot connect to proxy

Check network configuration:
```bash
# Verify client is in sess-<sid> network
docker inspect client-<sanitized-session-id> | grep -A 5 Networks

# Verify void-proxy has alias "proxy" in sess-<sid> network
docker inspect void-proxy-<sanitized-session-id> | grep -A 10 Networks
```

### Void proxy cannot reach itzg-server

Check backend network:
```bash
# Both should be in "backend" network
docker network inspect backend
```

## Files Modified

1. **cmd/session-proxy/main.go** - Complete rewrite of container orchestration
2. **userspace/dashboard/nginx.conf** - Dynamic routing to session containers
3. **userspace/dashboard/Dockerfile** - Enable nginx template substitution
4. **Dockerfile** - Update environment variables
5. **README.md** - Document new architecture
6. **userspace/docker-compose.yml** - Reference documentation only
