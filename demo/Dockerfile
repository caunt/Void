FROM docker:dind@sha256:cebc379a615d8dec408ac36f8854bb5002dc83e64fc8bc6f35312130f898d0e0

WORKDIR /demo
COPY . /demo

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

cd /demo
echo "Starting demo stack ..."
exec docker compose --file shared.yml up --build
SH

RUN tr -d '\r' < /run-compose.sh > /run-compose.fixed.sh && mv /run-compose.fixed.sh /run-compose.sh && chmod +x /run-compose.sh

ENTRYPOINT ["/run-compose.sh"]
