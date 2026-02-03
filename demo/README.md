# caunt/void-demo (dind single-container)

This image runs **one visible container** on the host, and internally runs Docker (dind) to create per-session child containers.

## Requirements

Run the root container with **--privileged** (required for dind).

## Put your child build context

Replace `userspace/` with your real child Dockerfile + files.
The child container must start an HTTP server listening on **0.0.0.0:8080**.

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

- Child containers are started **privileged**.
- Sessions expire after **300 seconds** and containers are stopped with `docker stop --time 0`.
- If a session container is still starting, you’ll see a live “Starting session” page that polls status (~1 request / 1.5s) and only redirects when the child port is actually reachable and switches to the real page when ready.
- If a session is expired, you’ll see a live “Session expired” page that redirects to a new session.


## Networking note

Because this is true dind, the root process is not a member of the child bridge network. Each child container’s port 8080 is published to the root container loopback as `127.0.0.1:<randomPort>`, and the proxy targets that.


### Boot waiting traffic

When the child is still starting, only the main HTML navigation gets the live waiting page. Non-HTML asset requests return a simple 503 text response so the browser doesn't spawn multiple waiting pages in parallel.
