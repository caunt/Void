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
| `GET` | `/api/game/players` | `200` | Read the live local player and client-tracked players. |
| `PUT` | `/api/game/options` | `204` | Replace Minecraft `options.txt`. |
| `POST` | `/api/game/start/vanilla` | `202` | Start a Mojang vanilla release. |
| `POST` | `/api/game/start/neoforge` | `202` | Start a NeoForge release, latest stable by default. |
| `POST` | `/api/game/start/curseforge` | `202` | Start a CurseForge modpack file. |
| `POST` | `/api/game/connect` | `200` | Converge the running game on a server connection. |
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
  "failure": null,
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
| `failure` | Structured runtime failure with `code`, `operation`, `stage`, `message`, `exceptionType`, and full `stackTrace`; otherwise `null`. |
| `warnings` | Non-fatal messages associated with the state. |
| `updatedAt` | ISO 8601 timestamp of the latest status change. |

Start operations return this status with `202 Accepted` and a `Location: /api/game/status` header.
Save its `operationId`, then poll until the same operation reports `ready` and `succeeded`.
Stop polling with a failure if the identifier changes or the operation becomes `failed` or `canceled`.

Runtime failures are also returned in the `failure` extension of HTTP Problem Details responses. An unexpected Minecraft process exit uses `client.process.exited`; a cgroup-confirmed OOM kill uses `client.process.out_of_memory` and reports the exit code and configured heap. Stack traces intentionally expose container implementation details and should not be forwarded to untrusted consumers.

## Live Players

`GET /api/game/players` reads players from the running Minecraft client. It does not require a mod, server plugin, or protocol-specific configuration.

```json
{
  "local": {
    "uuid": "f84c6a79-7f8b-4b7c-a02a-7bc7e3f99574",
    "name": "LocalPlayer",
    "position": { "x": 0.5, "y": 64.0, "z": 0.5 },
    "body": { "yaw": 15.0 },
    "head": { "yaw": 20.0, "pitch": -5.0 }
  },
  "remote": [
    {
      "uuid": "62ca1f73-b4d1-47bb-b176-052905a08b35",
      "name": "Neighbour",
      "position": { "x": 2.5, "y": 64.0, "z": 0.5 },
      "body": { "yaw": -10.0 },
      "head": { "yaw": -5.0, "pitch": 2.5 }
    }
  ]
}
```

Every returned player always contains finite `x`, `y`, and `z` coordinates plus finite body yaw, head yaw, and head pitch values in degrees. Profile identity is `null` when unavailable. A client without a current player world returns `409 Conflict`; tracker initialization or complete position or rotation failures return `503 Service Unavailable` rather than partial player data.

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

Only one game can run at a time. Every start request accepts an optional `arguments` array containing PortableMC start arguments and an optional positive `memoryMb` integer. When provided, `memoryMb` sets the Minecraft JVM maximum heap through `-Xmx`; it does not limit total container memory. Do not also provide a raw `-Xmx` argument.

Mojang used a 2 GiB default before 26.1 and changed it to 4 GiB for 26.1. See [**Minecraft Java Edition memory allocation**](https://help.minecraft.net/hc/en-us/articles/39083573916941) and the [**Minecraft Java Edition 26.1 release notes**](https://www.minecraft.net/en-us/article/minecraft-java-edition-26-1).

### Vanilla

`version` is a Mojang release identifier:

```bash
curl --fail-with-body \
  --request POST \
  --header 'Content-Type: application/json' \
  --data '{"version":"1.21.8","memoryMb":2048,"arguments":["--username","TestPlayer"]}' \
  http://localhost:8080/api/game/start/vanilla
```

### NeoForge

`version` is an optional Minecraft release identifier, the same form the vanilla endpoint accepts. Omit it, or send a
blank value, to resolve and start the latest stable NeoForge release:

```bash
curl --fail-with-body \
  --request POST \
  --header 'Content-Type: application/json' \
  --data '{"memoryMb":4096,"arguments":["--username","TestPlayer"]}' \
  http://localhost:8080/api/game/start/neoforge
```

Supply it to start NeoForge for a specific Minecraft version instead:

```bash
curl --fail-with-body \
  --request POST \
  --header 'Content-Type: application/json' \
  --data '{"version":"1.21.1","memoryMb":2048,"arguments":["--username","TestPlayer"]}' \
  http://localhost:8080/api/game/start/neoforge
```

Sodium is installed automatically when Modrinth provides a compatible NeoForge build for the resolved Minecraft version.

### CurseForge

Provide a project `slug` and positive CurseForge `fileId`. The container must have `CURSEFORGE_API_KEY` configured.

```bash
curl --fail-with-body \
  --request POST \
  --header 'Content-Type: application/json' \
  --data '{"slug":"all-the-mods-10","fileId":1234567,"memoryMb":4096,"arguments":["--username","TestPlayer"]}' \
  http://localhost:8080/api/game/start/curseforge
```

:::note
Replace the example CurseForge file identifier with the exact file you want to launch.
:::

## Connecting

The game must be running. `port` must be between `1` and `65535`. Connection navigation is idempotent for a given `host` and `port`: repeated callers join the same background operation, and an already-connected game returns its original result. A different target is rejected with `409 Conflict` while connecting or connected.

Canceling an HTTP caller stops only that caller's wait. The background navigation continues from whichever Minecraft screen is currently visible until the game connects, exits, or is stopped through the API.

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
