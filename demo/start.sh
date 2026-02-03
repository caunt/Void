#!/bin/sh
set -e

# Start Docker daemon inside this container (dind)
# Requires running this container with: --privileged
/usr/local/bin/dockerd-entrypoint.sh dockerd >/var/log/dockerd.log 2>&1 &

# Wait for Docker to be ready
until docker info >/dev/null 2>&1; do
  sleep 0.2
done

# Copy tcp-proxy binary to userspace for building proxy image
cp /usr/local/bin/tcp-proxy /opt/userspace/tcp-proxy/tcp-proxy

# Build tcp-proxy image
echo "Building tcp-proxy image..."
docker build -t tcp-proxy:latest /opt/userspace/tcp-proxy

# Build client image
echo "Building client image..."
docker build -t client:latest /opt/userspace/client

# Create backend network
echo "Creating backend network..."
docker network create backend 2>/dev/null || true

# Start shared mc-server on backend network
echo "Starting shared mc-server..."
docker run -d --rm \
  --name mc-server \
  --network backend \
  -e EULA=TRUE \
  -e TYPE=PAPER \
  -e VERSION=LATEST \
  -e ONLINE_MODE=FALSE \
  -e OVERRIDE_SERVER_PROPERTIES=TRUE \
  -e SERVER_PORT=25565 \
  itzg/minecraft-server:latest

exec /usr/local/bin/session-proxy
