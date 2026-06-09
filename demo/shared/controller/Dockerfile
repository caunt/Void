FROM golang@sha256:68cb6d68bed024785b69195b89af7ac7a444f27791435f98647edff595aa0479 AS build

WORKDIR /demo
COPY . /demo

RUN CGO_ENABLED=0 go build -trimpath -ldflags "-s -w" -o /out/controller /demo/shared/controller/main.go

FROM alpine@sha256:fa1b3b8cd12d2b2ded5ef366f99b5a7556884646af680404989d626535a3ac14

RUN apk add --no-cache docker-cli docker-cli-compose

WORKDIR /demo
COPY . /demo

COPY --from=build /out/controller /demo/controller
EXPOSE 80

ENTRYPOINT ["/demo/controller"]
