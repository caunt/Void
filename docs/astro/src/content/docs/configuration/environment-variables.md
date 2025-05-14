---
title: Environment Variables
description: Learn how to configure Void Proxy from environment variables
sidebar:
  order: 1
---

Environment variables make configuration in container environments easier.
However, they might be used terminal directly as well.

## Plugins
- `VOID_PLUGINS`
  Defines the list of URL's or Path's to load plugins from.  
  Example: `https://example.org/download/YoutPlugin1.dll;/home/YourPlugin2.dll` 
 
## NuGet
- `VOID_NUGET_REPOSITORIES`  
  Defines the NuGet repositories to use.  
  Example: `https://api.nuget.org/v3/index.json;https://nuget.void.dev/v3/index.json`

## Watchdog
- `VOID_WATCHDOG_ENABLE`  
  Enables the watchdog.
  Example: `true`

## Mojang
- `VOID_MOJANG_SESSIONSERVER`  
  Defines the Mojang session server to use.  
  Example: `https://sessionserver.mojang.com/session/minecraft/hasJoined`

- `VOID_MOJANG_PREVENT_PROXY_CONNECTIONS`  
  Tells Mojang to disallow player proxy connections.  
  Example: `true`