---
title: Containers
description: Learn how to run Void in a Kubernetes cluster or Docker container.
sidebar:
  order: 1
---

Void provides docker images for running in a container.  
See the [Void Docker Hub](https://hub.docker.com/r/caunt/void/tags) for all available images.

## Running Void in a Docker
To run Void in a Docker container, use following example command:
```bash
docker run --name void --network host --rm caunt/void:dev
```

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
```

## Configuring Void in Containers
Use [**environment variables**](/configuration/environment-variables/) or [**mount volumes**](/configuration/in-file/) to configure Void.


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
Versions below are historical and are not longer maintained
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