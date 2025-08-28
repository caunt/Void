---
title: Essentials
description: Built-in commands for server redirection, moderation, and debugging.
sidebar:
  order: 0
---

The [**Essentials plugin**](https://github.com/caunt/Void/tree/main/src/Plugins/Essentials) ships with Void and offers basic utilities for managing the proxy and connected players.

## Server Redirection

Use `/server [server-name]` to send yourself to another backend server. If no name is given, one is chosen at random. Register servers in the [**server configuration**](/docs/configuration/in-file/#proxy) or with the [**program argument**](/docs/configuration/program-arguments#servers) `--server`.

## Platform Commands

- `/stop` — immediately stops the proxy.
- `/plugins` — lists currently loaded plugins. Learn more about [**developing plugins**](/docs/developing-plugins/development-kit/).
- `/unload <name>` — unloads a plugin container without restarting.

## Moderation

`/kick <name> [reason]` removes a player from the proxy with an optional message.

## Debugging

Essentials logs every network message at the trace level, helping diagnose issues during development or testing.
