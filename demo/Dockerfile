FROM golang:1.22-alpine AS build
WORKDIR /src
RUN apk add --no-cache ca-certificates
COPY go.mod go.sum ./
RUN go mod download
COPY . .
RUN CGO_ENABLED=0 go build -trimpath -ldflags "-s -w" -o /out/session-proxy ./cmd/session-proxy

FROM docker:27-dind
RUN apk add --no-cache ca-certificates
ENV LISTEN_ADDR=0.0.0.0:8080
ENV SESSION_TTL_SECONDS=7200
ENV BACKEND_NETWORK=backend
ENV DASHBOARD_IMAGE=dashboard:latest
ENV VOID_IMAGE=terminal-void:latest
ENV CLIENT_IMAGE=client:latest
ENV ITZG_IMAGE=terminal-itzg:latest

COPY --from=build /out/session-proxy /usr/local/bin/session-proxy
COPY userspace /opt/userspace
COPY start.sh /start.sh
RUN chmod +x /start.sh

EXPOSE 8080
ENTRYPOINT ["/start.sh"]
