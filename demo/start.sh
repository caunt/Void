#!/bin/sh
set -e

# Start Docker daemon inside this container (dind)
# Requires running this container with: --privileged
/usr/local/bin/dockerd-entrypoint.sh dockerd >/var/log/dockerd.log 2>&1 &

# Wait for Docker to be ready
until docker info >/dev/null 2>&1; do
  sleep 0.2
done

exec /usr/local/bin/session-proxy
