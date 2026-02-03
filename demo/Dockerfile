FROM golang:1.22-alpine AS build
WORKDIR /src
RUN apk add --no-cache ca-certificates
COPY go.mod go.sum ./
RUN go mod download
COPY . .
RUN CGO_ENABLED=0 go build -trimpath -ldflags "-s -w" -o /out/session-proxy ./cmd/session-proxy
RUN CGO_ENABLED=0 go build -trimpath -ldflags "-s -w" -o /out/tcp-proxy ./cmd/tcp-proxy

FROM docker:27-dind
RUN apk add --no-cache ca-certificates
ENV LISTEN_ADDR=0.0.0.0:8080
ENV SESSION_TTL_SECONDS=300
ENV BACKEND_NETWORK=backend
ENV CLIENT_IMAGE=client:latest
ENV PROXY_IMAGE=tcp-proxy:latest
ENV CLIENT_PORT=6080

COPY --from=build /out/session-proxy /usr/local/bin/session-proxy
COPY --from=build /out/tcp-proxy /usr/local/bin/tcp-proxy
COPY userspace /opt/userspace
COPY start.sh /start.sh
RUN chmod +x /start.sh

EXPOSE 8080
ENTRYPOINT ["/start.sh"]
