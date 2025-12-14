---
title: Environment Variables
description: Learn how to configure Void Proxy from environment variables.
sidebar:
  order: 1
---

Environment variables make configuration in [**container environments**](/docs/containers/) easier.
However, you can also use them directly from the terminal.

## Plugins
- `VOID_PLUGINS`
  Defines the list of URLs or paths to load plugins from.
  Example: `https://example.org/download/YourPlugin1.dll;/home/YourPlugin2.dll`
 
## NuGet
- `VOID_NUGET_REPOSITORIES`  
  Defines the NuGet repositories to use.  
  Example: `https://api.nuget.org/v3/index.json;https://nuget.void.dev/v3/index.json`

## Watchdog
- `VOID_WATCHDOG_ENABLE`
  Enables the [**watchdog**](/docs/watchdog).
  Example: `true`

## Proxy
- `VOID_OFFLINE`
  Allows players to connect without Mojang authentication.
  Example: `true`

## Mojang
- `VOID_MOJANG_SESSIONSERVER`  
  Defines the Mojang session server to use.  
  Example: `https://sessionserver.mojang.com/session/minecraft/hasJoined`

- `VOID_MOJANG_PREVENT_PROXY_CONNECTIONS`
  Tells Mojang to disallow player proxy connections.
  Example: `true`
