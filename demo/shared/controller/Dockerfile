FROM golang@sha256:c5b07c17f54c5f22230c4b4da6e90249165cf55368e01a52808cb92064e18836 AS build

WORKDIR /demo
COPY . /demo

RUN CGO_ENABLED=0 go build -trimpath -ldflags "-s -w" -o /out/controller /demo/shared/controller/main.go

FROM alpine@sha256:28bd5fe8b56d1bd048e5babf5b10710ebe0bae67db86916198a6eec434943f8b

RUN apk add --no-cache docker-cli docker-cli-buildx docker-cli-compose

WORKDIR /demo
COPY . /demo

COPY --from=build /out/controller /demo/controller
EXPOSE 80

ENTRYPOINT ["/demo/controller"]
