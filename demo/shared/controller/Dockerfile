FROM golang@sha256:0d1d3a794be25f809dd2cb3160d8c73276c4056a9f8242a138e908ddeee7b6b6 AS build

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
