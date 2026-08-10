FROM docker:dind@sha256:e8faad5a8dc5279dff929afc5449f2791736912fff9f99351d742db2fad01b4c

WORKDIR /demo
COPY demo /demo
COPY src/Client /src/Client

RUN cat > /run-compose.sh <<'SH'
#!/bin/sh
set -e

dockerd-entrypoint.sh dockerd >/dev/null 2>&1 &

echo "Waiting for Docker to start ..."
until docker info >/dev/null 2>&1
do
  sleep 0.1
done

echo "Removing existing containers"
docker rm -f $(docker ps -aq) 2>/dev/null || true
echo "Removing existing networks"
docker network rm $(docker network ls -q) 2>/dev/null || true
echo "Removing existing volumes"
docker volume rm $(docker volume ls -q) 2>/dev/null || true

echo "Building Portable Minecraft client"
docker build --target latest --build-arg PREINSTALL_PORTABLEMC_DEMO=true --tag portableminecraftclient:latest /src/Client

cd /demo
echo "Starting demo stack ..."
exec docker compose --file shared.yml up --build
SH

RUN tr -d '\r' < /run-compose.sh > /run-compose.fixed.sh && mv /run-compose.fixed.sh /run-compose.sh && chmod +x /run-compose.sh

ENTRYPOINT ["/run-compose.sh"]
