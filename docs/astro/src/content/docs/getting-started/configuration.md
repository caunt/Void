---
title: Configuration
description: Learn how to configure Void Proxy
sidebar:
  order: 2
---

File configurations and environment variables are primary ways of configuration.
However many options may be rewritten in runtime using the API from Plugins.

## Proxy

```toml
// settings.toml
# Defines the network interface to bind on
Address = "0.0.0.0"

# Defines the port
Port = 25565

# Packet size threshold before compression
CompressionThreshold = 256

# General per player timeout for many operations (in milliseconds)
KickTimeout = 10000

# Logging level (valid values are Trace, Debug, Information, Warning, Error, Critical)
LogLevel = "Information"

# Predefined list of servers. 
# Players will be connected to the first one, if not specified otherwise from plugins.
Servers = [
	{ Name = "lobby", Host = "127.0.0.1", Port = 25566 },
	{ Name = "minigames", Host = "127.0.0.1", Port = 25567 },
	{ Name = "limbo", Host = "127.0.0.1", Port = 25568 },
]
```

## Watchdog

[**Watchdog**](../../watchdog) is a built-in HTTP server that allows you to check status or control the proxy.

```toml
// configs/Watchdog/Settings.toml
# Enables the watchdog
Enabled = false

# Defines the network interface to bind on
Address = *

# Defines the port
Port = 80
```

## Forwarding

Forwarding helps to forward player data (IP, UUID, Skin, etc.) to the backend server.

### Velocity (Modern)
```toml
// configs/Velocity/Settings.toml
# Enables Velocity forwarding
Enabled = true

# The secret key (should be same as on backend server)
Secret = "YourSecretKeyHere"
```

## Plugins Installation

Plugins are compiled with the *.dll extension in any .NET compatible language.
See the [**Plugin Development Kit**](../../developing-plugins/development-kit) section for more details.

Directory `plugins` is the default location for plugins.
Environment variable `VOID_PLUGINS` might be used to include URLs or Local Paths to plugins, separated by coma or semicolon.
Multiple options `--plugin` (short `-p`) might be used to include URLs or Local Paths to plugins.

Examples:
```bash
$ ./void-linux-x64 --plugin "/home/YourPlugin1.dll" --plugin "https://example.org/download/YoutPlugin2.dll"
```
```bash
$ VOID_PLUGINS="https://example.org/download/YoutPlugin1.dll;/home/YourPlugin2.dll" ./void-linux-x64
```

## Plugins Configurations (configs/\<Plugin\>/*.toml)

Each plugin may define one or subset of keyed configuration files in its own directory. 
Plugins are not required to save or load configurations manually. 

All changes on the disk are automatically loaded into existing instance in the memory.
Vice versa, all changes in the memory are automatically saved to the disk.

Currently, only TOML format is supported.