---
title: How to Set Up Void Minecraft Proxy in Docker for Modded & Vanilla Servers
description: Learn how to run the Void Minecraft Proxy in Docker with a clean, fast setup for both modded and vanilla servers, plus tips for plugins, forwarding, and smooth production use.
template: splash
pagefind: false
editUrl: false
---

# Setting up Minecraft Proxy — Void (In Docker)

When I first pointed my server traffic through **Void**, I expected a weekend of tinkering and a long night of regrets. What I got instead was a clean, predictable setup that took minutes, felt modern, and frankly made me a little smug. If you’ve been juggling Velocity/Bungee-era rituals, or you’re running a modded network and want something that speaks fluent .NET and plain English, this is the path I wish I’d taken sooner.

Let me walk you through exactly how I run Void in Docker, why I chose the flags I did, and a few real‑world notes from putting it behind production traffic.

---

## Why Void for a Docker-friendly proxy

I like proxies that don’t fight me. Void boots fast, has sensible flags, and plays nicely with both legacy and modern [**forwarding models**](/docs/forwardings/forwarding-overview). It’s comfortable in a container, which means fewer moving parts on the host and easier rollbacks. Most importantly, it behaves the same on my laptop and on the node in the rack.

If you’re running modded servers, you know the routine: weird handshakes, changing headers, and that one forge build that decides it’s special. Void handles the chaos without turning your launch script into a novella.

---

## My mental model: one clean container, one clear port

I keep Void as a single ephemeral container that owns the public port and forwards to whatever backend I declare. That’s it. No sidecars. No mystery env sprawl. If I want to swap configurations or try a different plugin, I kill and run again.

A few ground rules I follow: stateless by default, no mounting config volumes unless I must, and host networking when I want port handling to be predictable.

---

## Building the actual run command

Once I’ve decided my backend target and public entry port, I start Void with the essential flags in a single `docker run` line. I skip clutter and only include what matters: backend address, interface, port, and any relevant toggles like `--ignore-file-servers`.

Example with one backend:

```bash
docker run --network host --rm caunt/void:dev \
  --ignore-file-servers \
  --server mc.example.com:25566 \
  --port 25565 \
  --interface 0.0.0.0
```

---

## Adding plugins without ceremony

Void’s `-p, --plugin <plugin>` flag takes a file path, directory, or even a URL. I point it to the tools I need — maybe a rate limiter or branding overlay — without special packaging.

Example:

```bash
docker run --network host --rm caunt/void:dev \
  --server 10.0.0.5:25566 \
  --interface 0.0.0.0 \
  --port 25565 \
  --plugin https://example.org/download/RateLimiter.dll \
  --plugin /home/me/plugins/BrandingOverlay.dll
```

---

## Online vs offline mode

For public servers, I stay online. In isolated tests, I add `--offline` for quick joins without Mojang auth:

```bash
docker run --network host --rm caunt/void:dev \
  --server 127.0.0.1:25566 \
  --interface 0.0.0.0 \
  --port 25565 \
  --offline
```

---

## Forwarding models and common pitfalls

Void supports both legacy and modern forwarding. Just make sure your backend’s forwarding setting matches the proxy. Mismatches are the main cause of timeouts right after login.

---

## Logging levels I use

I default to Information, switch to Debug when verifying changes, and occasionally use Trace for packet-level checks. Example:

```bash
docker run --network host --rm caunt/void:dev \
  --server 10.0.0.5:25566 \
  --logging Debug
```

---

## IPv6 note

For IPv6 backends, wrap addresses in brackets:

```bash
--server [2001:db8::1]:25565
```

---

## My quick recipes

Single backend:

```bash
docker run --network host --rm caunt/void:dev \
  --server 127.0.0.1:25566 \
  --interface 0.0.0.0 \
  --port 25565
```

Two backends:

```bash
docker run --network host --rm caunt/void:dev \
  --server 10.0.0.5:25566 \
  --server 10.0.0.8:25568 \
  --interface 0.0.0.0 \
  --port 25565
```

---

## Final thoughts

The best thing about running Void in Docker is how unremarkable it becomes. A single clean container, a few flags, and it just works so I can focus on the parts my players notice.
