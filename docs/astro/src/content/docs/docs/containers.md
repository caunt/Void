---
title: Containers
description: Learn how to run Void in a Kubernetes cluster or Docker container.
sidebar:
  order: 1
---

Void provides docker images for running in a container.
See the [Void Docker Hub](https://hub.docker.com/r/caunt/void/tags) for all available images.

For running Void directly on your host, refer to the [Running](/docs/getting-started/running/) guide.

## Running Void in Docker
To run Void in a Docker container, use the following example command:
```bash
docker run --name void --network host --pull=always --rm caunt/void:dev
```

You can pass additional [program arguments](/docs/configuration/program-arguments/) to customize servers and network settings:

```bash
docker run --name void --network host --pull=always --rm caunt/void:dev \
  --ignore-file-servers \
  --server mc.example.com:25566 \
  --port 25565 \
  --interface 0.0.0.0
```

:::tip[Offline Mode]
Add `--offline` to allow players to connect without Mojang authorization.
:::

## Running Void in Kubernetes
To run Void in a Kubernetes cluster, follow this example `Deployment` manifest:
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: void-deployment
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
        ports:
        - name: watchdog
          containerPort: 80
        - name: proxy
          containerPort: 25565
        env:
        - name: VOID_PLUGINS
          value: "https://example.org/download/YourPlugin1.dll"
        - name: VOID_WATCHDOG_ENABLE
          value: "true"
        - name: VOID_OFFLINE
          value: "true"
```

## Configuring Void in Containers
Use [**environment variables**](/docs/configuration/environment-variables/) or [**mount volumes**](/docs/configuration/in-file/) to configure Void.


## Image Tags
:::tip[Latest Stable Version]
- **caunt/void:latest** - Latest stable version of Void.
:::

Other `latest` versions:
- caunt/void:latest-windows
- caunt/void:latest-windows-win-x64
- caunt/void:latest-linux-x64
- caunt/void:latest-linux-arm
- caunt/void:latest-linux-arm64
- caunt/void:latest-alpine
- caunt/void:latest-alpine-linux-musl-x64
- caunt/void:latest-alpine-linux-musl-arm
- caunt/void:latest-alpine-linux-musl-arm64
- caunt/void:latest-android
- caunt/void:latest-android-linux-bionic-x64

:::tip[Latest Development Version]
- **caunt/void:dev** - Latest development version of Void.
:::

Other `dev` versions:
- caunt/void:dev-windows
- caunt/void:dev-windows-win-x64
- caunt/void:dev-linux-x64
- caunt/void:dev-linux-arm
- caunt/void:dev-linux-arm64
- caunt/void:dev-alpine
- caunt/void:dev-alpine-linux-musl-x64
- caunt/void:dev-alpine-linux-musl-arm
- caunt/void:dev-alpine-linux-musl-arm64
- caunt/void:dev-android
- caunt/void:dev-android-linux-bionic-x64

:::caution
Versions below are historical and are no longer maintained
:::

- caunt/void:**`<version>`** - Specific version of Void release, e.g. **`caunt/void:0.5.1`**.

Other specific versions:
- caunt/void:**`<version>`**-windows
- caunt/void:**`<version>`**-windows-win-x64
- caunt/void:**`<version>`**-linux-x64
- caunt/void:**`<version>`**-linux-arm
- caunt/void:**`<version>`**-linux-arm64
- caunt/void:**`<version>`**-alpine
- caunt/void:**`<version>`**-alpine-linux-musl-x64
- caunt/void:**`<version>`**-alpine-linux-musl-arm
- caunt/void:**`<version>`**-alpine-linux-musl-arm64
- caunt/void:**`<version>`**-android
- caunt/void:**`<version>`**-android-linux-bionic-x64