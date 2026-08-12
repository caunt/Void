---
title: API Reference
description: Control the Docker Minecraft client over HTTP.
---

The client API is available under `/api` on container port `80`.
JSON property names and enum values use camel case.

## Endpoints

| Method | Path | Success | Description |
| :----- | :--- | :------ | :---------- |
| `GET` | `/api/health` | `200` | Check whether the client service is ready. |
| `GET` | `/api/game/status` | `200` | Read the game state and latest operation. |
| `PUT` | `/api/game/options` | `204` | Replace Minecraft `options.txt`. |
| `POST` | `/api/game/start/vanilla` | `202` | Start a Mojang vanilla release. |
| `POST` | `/api/game/start/neoforge` | `202` | Start the latest stable NeoForge release. |
| `POST` | `/api/game/start/curseforge` | `202` | Start a CurseForge modpack file. |
| `POST` | `/api/game/connect` | `200` | Connect the ready game to a server. |
| `POST` | `/api/game/send-chat` | `204` | Send chat text or a command. |
| `GET` | `/api/game/screenshot` | `200` | Return the Minecraft window as `image/png`. |
| `POST` | `/api/game/stop` | `200` | Stop the current game. |

## Status and Operations

`GET /api/game/status` returns the current state and the most recently accepted operation:

```json
{
  "state": "ready",
  "operationId": 1,
  "operation": "start-vanilla",
  "operationState": "succeeded",
  "processId": 432,
  "exitCode": null,
  "server": null,
  "message": "Game window is ready",
  "error": null,
  "warnings": [],
  "updatedAt": "2026-08-12T12:00:00+00:00"
}
```

| Field | Description |
| :---- | :---------- |
| `state` | Game lifecycle: `idle`, `starting`, `ready`, `connected`, `stopping`, or `failed`. |
| `operationId` | Increasing identifier for the latest accepted operation. |
| `operation` | Latest operation: `start-vanilla`, `start-neoforge`, `start-curseforge`, `options`, `connect`, `send-chat`, `screenshot`, or `stop`; otherwise `null`. |
| `operationState` | `none`, `running`, `succeeded`, `failed`, or `canceled`. |
| `processId` | Minecraft process identifier while it is running. |
| `exitCode` | Minecraft exit code after it exits, when available. |
| `server` | Connected `host` and `port`, otherwise `null`. |
| `message` | Short description of the current result. |
| `error` | Failure message, otherwise `null`. |
| `warnings` | Non-fatal messages associated with the state. |
| `updatedAt` | ISO 8601 timestamp of the latest status change. |

Start operations return this status with `202 Accepted` and a `Location: /api/game/status` header.
Save its `operationId`, then poll until the same operation reports `ready` and `succeeded`.
Stop polling with a failure if the identifier changes or the operation becomes `failed` or `canceled`.

## Health

```bash
curl --fail http://localhost:8080/api/health
```

A ready API returns `200 OK` with `ok` as plain text. An unavailable client service returns `503 Service Unavailable`.

## Game Options

Send the complete `options.txt` contents as `text/plain`:

```bash
curl --fail-with-body \
  --request PUT \
  --header 'Content-Type: text/plain' \
  --data-binary $'fullscreen:false\nrenderDistance:2\nenableVsync:false' \
  http://localhost:8080/api/game/options
```

The options are retained in the configured Minecraft directory and used by later launches.

## Starting a Game

Only one game can run at a time. Every start request accepts an optional `arguments` array containing PortableMC start arguments.

### Vanilla

`version` is a Mojang release identifier:

```bash
curl --fail-with-body \
  --request POST \
  --header 'Content-Type: application/json' \
  --data '{"version":"1.21.8","arguments":["--username","TestPlayer"]}' \
  http://localhost:8080/api/game/start/vanilla
```

### NeoForge

This endpoint resolves and starts the latest stable NeoForge release:

```bash
curl --fail-with-body \
  --request POST \
  --header 'Content-Type: application/json' \
  --data '{"arguments":["--username","TestPlayer"]}' \
  http://localhost:8080/api/game/start/neoforge
```

Sodium is installed automatically when Modrinth provides a compatible NeoForge build for the resolved Minecraft version.

### CurseForge

Provide a project `slug` and positive CurseForge `fileId`. The container must have `CURSEFORGE_API_KEY` configured.

```bash
curl --fail-with-body \
  --request POST \
  --header 'Content-Type: application/json' \
  --data '{"slug":"all-the-mods-10","fileId":1234567,"arguments":["--username","TestPlayer"]}' \
  http://localhost:8080/api/game/start/curseforge
```

:::note
Replace the example CurseForge file identifier with the exact file you want to launch.
:::

## Connecting

The game must be `ready`. `port` must be between `1` and `65535`.

```bash
curl --fail-with-body \
  --request POST \
  --header 'Content-Type: application/json' \
  --data '{"host":"minecraft-server","port":25565}' \
  http://localhost:8080/api/game/connect
```

The confirmed connection returns:

```json
{
  "server": {
    "host": "minecraft-server",
    "port": 25565
  },
  "connectedAt": "2026-08-12T12:01:00+00:00"
}
```

## Sending Chat

The game must be `connected`. A leading `/` sends a Minecraft command.

```bash
curl --fail-with-body \
  --request POST \
  --header 'Content-Type: application/json' \
  --data '{"message":"Hello from the client"}' \
  http://localhost:8080/api/game/send-chat
```

## Taking a Screenshot

The game must be `ready` or `connected`.

```bash
curl --fail \
  --output minecraft.png \
  http://localhost:8080/api/game/screenshot
```

## Stopping

```bash
curl --fail-with-body \
  --request POST \
  http://localhost:8080/api/game/stop
```

The response contains `mode` and the final `status`:

```json
{
  "mode": "graceful",
  "status": {
    "state": "idle",
    "operationId": 5,
    "operation": "stop",
    "operationState": "succeeded",
    "processId": null,
    "exitCode": 0,
    "server": null,
    "message": "Game stopped",
    "error": null,
    "warnings": [],
    "updatedAt": "2026-08-12T12:02:00+00:00"
  }
}
```

`mode` is `alreadyStopped`, `graceful`, or `forced`.

## Errors

Errors use the standard Problem Details JSON format.

| Status | Meaning |
| :----- | :------ |
| `400 Bad Request` | A required value is missing or invalid. |
| `408 Request Timeout` | The request was canceled. |
| `409 Conflict` | The requested action is not valid for the current game state or another operation is running. |
| `500 Internal Server Error` | The operation failed unexpectedly. |
| `503 Service Unavailable` | The client service is not ready. |
| `504 Gateway Timeout` | A client operation exceeded its timeout. |
