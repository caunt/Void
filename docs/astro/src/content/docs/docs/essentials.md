---
title: Essentials
description: Built-in commands for server redirection, moderation, and debugging.
sidebar:
  order: 0
---

The Essentials plugin ships with Void and offers basic utilities for managing the proxy and connected players. You can explore the [**Essentials plugin project**](https://github.com/caunt/Void/tree/main/src/Plugins/Essentials).

## Server Redirection

Use `/server [server-name]` to send yourself to another backend server. If no name is given, one is chosen at random. Review your [**server configuration**](/docs/configuration/in-file/#proxy) to see which names are available, or register new ones with the [**program argument**](/docs/configuration/program-arguments#servers) `--server`.

## Platform Commands

- `/stop` — immediately stops the proxy. When running in [**containers**](/docs/containers/), prefer orchestrator controls for clean restarts.
- `/plugins` — lists currently loaded plugins. Learn more about [**developing plugins**](/docs/developing-plugins/development-kit/).
- `/unload <name>` — unloads a plugin container without restarting.

## Moderation

`/kick <name> [reason]` removes a player from the proxy with an optional message. For broader moderation strategies, see the [**troubleshooting guide**](/docs/troubleshooting/).

## Debugging

Essentials logs every network message at the trace level, helping diagnose issues during development or [**testing**](/docs/getting-started/running/).
