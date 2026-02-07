# Void Demo

This directory contains a Docker-based demo of the Void Proxy that runs a complete Minecraft server and client environment in the browser.

Try the live demo at [**https://void-demo.caunt.world**](https://void-demo.caunt.world).

## Running the Demo

### Run Pre-built Image

```bash
docker run --rm --privileged -v demo-dind:/var/lib/docker -p 8080:80 -e REDIRECT_LOGS=true caunt/void-demo:latest
```

Then open your browser to `http://localhost:8080`

### Build and Run Locally

```bash
docker build -t caunt/void-demo:latest . && docker run --rm --privileged -v demo-dind:/var/lib/docker -p 8080:80 -e REDIRECT_LOGS=true caunt/void-demo:latest
```

### Cleanup

```bash
docker volume rm demo-dind && docker rmi caunt/void-demo:latest
```

## What's Included

The demo environment includes:
- Void Proxy running in a container
- Minecraft server running in a container
- Web-based Minecraft client accessible from your browser

## Learn More

- [**Main Documentation**](https://void.caunt.world/docs/)
- [**Getting Started Guide**](https://void.caunt.world/docs/getting-started/running/)
- [**Container Deployment Guide**](https://void.caunt.world/docs/containers/)
