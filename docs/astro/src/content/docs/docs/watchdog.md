---
title: Watchdog
description: Learn how to enable and use the Watchdog feature in Void.
---

The Watchdog feature in Void is designed to monitor the health of the Void Proxy and schedule a restart if required. 
This is particularly useful for long-running processes or when running Void in a production environment.

## Enable Watchdog
Watchdog is disabled by default. To enable it, you need to set the `Enabled` setting in the [**configuration file**](/docs/configuration/in-file#watchdog) to `true`.

## Health Check
`/health` endpoint is used to check the health of the Void Proxy.

```bash
$ curl http://localhost:80/health
OK
```

### Status Codes
- `200 OK`: The Void Proxy is running and healthy.
- `503 Service Unavailable`: The Void Proxy is not running or unhealthy.

### Response
- OK: The Void Proxy is running and healthy.
- PAUSED: The Void Proxy is not accepting connections.
- STOPPING: The Void Proxy is shutting down.

Status Code `200 OK` may be in any combination of above responses.   
It just means that the Void Proxy is still running and healthy.

## Bound Check
`/bound` endpoint is used to check if the Void Proxy is bound and accepting incoming connections.
```bash
$ curl http://localhost:80/bound
OK
```

### Status Codes
- `200 OK`: The Void Proxy is bound and accepting connections.
- `503 Service Unavailable`: The Void Proxy is not accepting connections.

### Response
Responses are the same as for the `/health` endpoint.

:::tip
While `/bound` may look similar to `/health`, it is important to note that the Void Proxy may be healthy but not accepting connections.
In such a case, the `/bound` endpoint will return `503 Service Unavailable`, while the `/health` endpoint will return `200 OK`.
:::

## Pause
`/pause` endpoint is used to pause the Void Proxy. This will stop accepting new connections but connected players will still be able to play.
```bash
$ curl http://localhost:80/pause
```

## Continue
`/continue` endpoint is used to continue the Void Proxy. This will allow new connections to be accepted again.
```bash
$ curl http://localhost:80/continue
```

## Stop
`/stop` endpoint is used to stop the Void Proxy. This will kick all players and shut down the Void Proxy immediately.
```bash
$ curl http://localhost:80/stop
```

## Slow Stop
`/slow-stop` endpoint is used to pause accepting new connections and wait for existing players to leave, then shut down the Void Proxy.
```bash
$ curl http://localhost:80/slow-stop
```

Once slow-stop is called, you cannot call `/continue`.  
Proxy will wait indefinitely for players to leave, but you can call `/stop` to forcefully shut down the Void Proxy.