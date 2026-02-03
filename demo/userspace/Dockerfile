FROM docker:27-dind

WORKDIR /project
COPY . /project

RUN cat > /run.sh <<'SH'
#!/bin/sh
set -e

dockerd --host=unix:///var/run/docker.sock > /var/log/dockerd.log 2>&1 &

while ! docker info >/dev/null 2>&1
do
  sleep 0.2
done

exec docker compose --project-directory /project -f /project/docker-compose.yml up --pull always --remove-orphans
SH

RUN chmod +x /run.sh
ENTRYPOINT ["/run.sh"]
