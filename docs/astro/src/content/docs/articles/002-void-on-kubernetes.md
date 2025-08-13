---
title: Void on Kubernetes: the setup I wish I’d had from day one
description: Production-ready guide for running Void proxy on Kubernetes with health checks, graceful rollouts, and plugin management.
template: splash
pagefind: false
---

# Void on Kubernetes: the setup I wish I’d had from day one

I moved my Minecraft network to Kubernetes for the same reason most of us do: fewer snowflake boxes, easier rollbacks, and boringly reliable updates. The missing piece was a proxy that didn’t need hand-holding. Void got there for me. It’s lightweight, fast to boot, and it exposes a tiny HTTP control plane that plays nicely with probes and scripts. Once I wired it into a Deployment, the rest felt… calm.

What follows is the exact layout I run in production. It’s simple, repeatable, and friendly to clusters of any size. If you’ve been juggling sidecars, shell hacks, or awkward restarts, this will feel refreshingly straightforward.

---

## What we’re actually running

Void is a .NET proxy that speaks to your backend Paper/Purpur/Fabric servers and presents a single TCP entry point for players. It exposes two important ports:

* **25565/TCP** for Minecraft traffic.
* **80/TCP** for a tiny HTTP control surface I’ll call the *watchdog*. It answers health, bound-state, and graceful control requests.

There are a few knobs I use every day:

* Program arguments like `--server`, `--port`, `--interface`, and `--plugin`.
* Environment variables for container-friendly config, including `VOID_PLUGINS`, `VOID_WATCHDOG_ENABLE`, and `VOID_OFFLINE`.
* File config remains available if you prefer, but I keep the container immutable and do overrides via args and env.

That’s the gist. Now let’s put it into a Deployment that ships.

---

## Namespace and basic assumptions

I’m assuming you have a working cluster with a default StorageClass, but we won’t rely on a filesystem layout inside the pod. DNS inside the cluster should resolve your backend server pods or Services like `minecraft-backend.default.svc.cluster.local`. For simplicity I’ll refer to it as `minecraft-backend` below.

---

## The Deployment I keep coming back to

This is the minimal, sane spec that behaves well under load, rotates cleanly, and answers probes correctly.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: void
  labels:
    app: void
spec:
  replicas: 1
  selector:
    matchLabels:
      app: void
  template:
    metadata:
      labels:
        app: void
    spec:
      containers:
        - name: void
          image: caunt/void:latest
          imagePullPolicy: Always
          args:
            - "--ignore-file-servers"
            - "--server"
            - "minecraft-backend:25566"
            - "--port"
            - "25565"
            - "--interface"
            - "0.0.0.0"
          env:
            - name: VOID_WATCHDOG_ENABLE
              value: "true"
            - name: VOID_OFFLINE
              value: "true"   # set to "false" if you require Mojang auth
            - name: VOID_PLUGINS
              value: "https://example.org/download/YourPlugin1.dll"
          ports:
            - name: watchdog
              containerPort: 80
              protocol: TCP
            - name: proxy
              containerPort: 25565
              protocol: TCP
          readinessProbe:
            httpGet:
              path: /bound
              port: watchdog
            initialDelaySeconds: 2
            periodSeconds: 3
          livenessProbe:
            httpGet:
              path: /health
              port: watchdog
            initialDelaySeconds: 5
            periodSeconds: 10
          lifecycle:
            preStop:
              httpGet:
                path: /slow-stop
                port: watchdog
          resources:
            requests:
              cpu: "100m"
              memory: "128Mi"
            limits:
              cpu: "2"
              memory: "1Gi"
```

A few notes that matter more than they look:

* **Readiness uses `/bound`**. It flips to ready only when Void has bound the player port and is accepting new connections. That prevents premature traffic during startup.
* **Liveness uses `/health`**. If the process wedges, Kubernetes restarts it quickly. I keep the probe gentle; Void itself is fast.
* **`preStop` calls `/slow-stop`**. That tells Void to stop accepting new connections and wait for current players to leave cleanly before the pod is terminated. It’s a small touch that prevents “everyone got kicked” incidents during rolling updates.

---

## Services that do the right thing

We expose the game port and keep the control port inside the cluster. If you’re on bare-metal with MetalLB or a cloud LB, this stays identical.

```yaml
---
apiVersion: v1
kind: Service
metadata:
  name: void-proxy
  labels:
    app: void
spec:
  type: LoadBalancer   # or NodePort if you prefer
  selector:
    app: void
  ports:
    - name: minecraft
      port: 25565
      targetPort: proxy
      protocol: TCP

---
apiVersion: v1
kind: Service
metadata:
  name: void-watchdog
  labels:
    app: void
spec:
  type: ClusterIP
  selector:
    app: void
  ports:
    - name: http
      port: 80
      targetPort: watchdog
      protocol: TCP
```

With that in place, any player hitting the load balancer’s external IP (or your DNS A record) lands on the proxy. Inside the cluster, ops and controllers can reach `void-watchdog.default.svc` for health and control actions.

---

## Quick sanity checks I always run

Make sure the app is actually listening and reporting the right state.

```bash
kubectl get pods -l app=void
kubectl get svc void-proxy void-watchdog
kubectl run -it --rm test --image=curlimages/curl --restart=Never -- \
  curl -s http://void-watchdog.default.svc/health && echo
kubectl run -it --rm test --image=curlimages/curl --restart=Never -- \
  curl -s http://void-watchdog.default.svc/bound && echo
```

You’ll see `OK` when the proxy is up, `PAUSED` if you’ve paused it, or `STOPPING` during a controlled shutdown. The readiness gate follows bound state, so traffic only flows when it should.

---

## Plugins, but keep the image clean

I keep the container immutable and let Void fetch plugins at boot. The trick is the `VOID_PLUGINS` variable. It accepts a semicolon- or comma‑separated list of URLs or local paths. I point it at a stable, versioned URL (an object store bucket, a Git release asset, or a simple HTTPS endpoint). When I ship a new plugin build, I update the URL in the Deployment and roll.

If your plugin pulls dependencies from NuGet, you can also set `VOID_NUGET_REPOSITORIES` so the resolver knows where to look. If your feed requires credentials, publish via a preauthenticated URL or an internal feed, then rotate credentials upstream. Keep the pod surface small.

Want to test a single plugin quickly? Pass `--plugin` as an arg alongside the env var. I often do both: keep a stable baseline via `VOID_PLUGINS`, then layer a temporary `--plugin https://…/feature-build.dll` while I validate a change under real traffic.

---

## Forwarding to backends without drama

Void can register servers from config files or via `--server`. In clusters, using `--server` keeps boot deterministic because you don’t wait on config materialization. I usually point it at a Service that fronts a Paper or Purpur Deployment.

If you use Velocity-style modern forwarding, enable it in the configuration file once and match the secret on backends. I keep the file inside the image during build time when I need it, but it’s equally fine to set that once in a golden image and forget about it—there’s nothing dynamic there.

If you run in offline mode for testing, `VOID_OFFLINE=true` makes that a one-line toggle. In public, flip it off. Simple switches save mistakes.

---

## Rolling updates that don’t kick everyone

Here’s what actually happens during `kubectl rollout restart` with the spec above:

* The new pod starts and stays unready until `/bound` returns OK.
* The Service keeps sending players to the old pod.
* When the new pod is ready, traffic equalizes.
* Kubernetes sends a SIGTERM to the old pod, and `preStop` hits `/slow-stop`.
* Old pod pauses new joins, waits for players to exit naturally, then shuts down.

If you need to force the cutover, you can call `/stop` against `void-watchdog` and it will kick sessions immediately. I reach for that only when I must.

---

## Horizontal scale: when to run more than one replica

For a single entry point you can absolutely run two or more pods behind the same Service. Each join is a fresh TCP connection, and once a player is on one pod, the session sticks to that pod. There’s no shared in‑memory state to coordinate. The only thing to watch is resource sizing per replica—don’t under‑provision CPU and then confuse lag with “network issues.”

If you want to steer certain regions to specific pods, add multiple `Service` objects with different labels and publish them under different DNS names (e.g., `play-eu.example.com`, `play-us.example.com`). Same Deployment template, multiple Services, clear routing.

---

## Ingress or not?

Most HTTP ingress controllers won’t help for raw TCP Minecraft traffic unless they support TCP services. If you already run something like NGINX Ingress or Traefik with TCP routing enabled, point it at `void-proxy`. Otherwise, a direct `LoadBalancer` on the Service keeps the path short and avoids head‑of‑line quirks.

For the control plane, I keep the watchdog on a ClusterIP Service and reach it from inside the cluster. If you want remote control for a small team, put an authentication layer in front of it and publish it over HTTPS with your reverse proxy of choice. Keep the attack surface tiny.

---

## Observability that fits in your head

The watchdog answers a handful of HTTP calls:

* `GET /health` tells you if the process is healthy.
* `GET /bound` tells you if player sockets are being accepted.
* `POST /pause` stops accepting new connections (current players keep playing).
* `POST /continue` resumes accepting new connections.
* `POST /stop` kicks everyone and shuts down.
* `POST /slow-stop` drains new joins and waits for players to leave, then exits.

That’s enough for probes, runbooks, and safe restarts. You don’t need a dozen sidecars. If you already run Prometheus, scrape the HTTP endpoints via a blackbox exporter and alert if `bound` flips for too long.

---

## A tiny runbook I actually use

Here are the commands I keep close when poking the proxy during an incident:

```bash
# See what version is live
kubectl -n default describe deploy/void | grep Image:

# Pause new joins during a backend migration
kubectl run -it --rm tmp --image=curlimages/curl --restart=Never -- \
  curl -X POST http://void-watchdog.default.svc/pause

# Resume when ready
kubectl run -it --rm tmp --image=curlimages/curl --restart=Never -- \
  curl -X POST http://void-watchdog.default.svc/continue

# Nudge a stuck pod (rare)
kubectl run -it --rm tmp --image=curlimages/curl --restart=Never -- \
  curl -X POST http://void-watchdog.default.svc/stop
```

It’s simple muscle memory: pause, adjust, continue. Less stress for you and your players.

---

## Resource sizing and headroom

Void doesn’t need much to run, but players are bursty. I start with 100m CPU and 128 MiB memory requested, with limits at 2 cores and 1 GiB. On real traffic, watch CPU throttling during join spikes. If you see liveness flaps while the host is busy, scale limits before you start changing probe intervals. Capacity problems masquerade as “random” restarts.

For big events, bump replicas to two and sleep better. The Deployment scales in seconds, and the slow‑stop drain keeps the handover smooth.

---

## Upgrades with zero fuss

I tag images carefully (`latest` for stable, separate channels for development). On update, the only moving part is the image tag. With the probes and lifecycle hook above, the rollout is a non‑event. If your plugin URL points at a versioned release, you can test canary pods by applying a label selector and a separate Service, then cut traffic over DNS. Keep it boring.

---

## Troubleshooting that pays rent

* **Pod is “ready” but players can’t join**: check `/bound`. If it’s `503`, the socket isn’t accepting yet—investigate port collisions or security policies. Your Service might also be pointing at the wrong `targetPort`.
* **Liveness keeps restarting the pod**: if `/health` flaps under load, look at CPU limits first. Then check GC pressure. Increase memory limit by a notch and retest.
* **Rolling restart still kicked players**: confirm that `preStop` executed. If your node was evicted hard, the lifecycle hook may not run; consider PodDisruptionBudgets to avoid simultaneous evictions.
* **Plugins didn’t load**: verify URLs and that the container has egress. If dependencies are private, set `VOID_NUGET_REPOSITORIES` to reachable feeds.

---

## Closing thoughts

The longer I run this, the more I appreciate not needing extra scaffolding. One Deployment. One public Service. One internal control plane. Health, readiness, and graceful shutdown handled by the app itself. The mental model stays small, which leaves you time for the work that actually moves the server forward—tuning backends, building events, and making your players happy.

When you want to scale, you scale. When you want to pause, you pause. And when you upgrade, nobody notices. That’s what good infrastructure should feel like.

