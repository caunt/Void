---
title: In-file
description: Learn how to configure Void Proxy from files.
sidebar:
  order: 0
---

File configurations are the main way of configuration.
However, many options can be overridden at runtime using the API in plugins.

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

# Allow players to connect without Mojang authentication
Offline = false

# Logging level (valid values are Trace, Debug, Information, Warning, Error, Critical)
LogLevel = "Information"

# Predefined list of servers. 
# Players will be connected to the first one, if not specified otherwise from plugins.
Servers = [
	{ Name = "lobby", Host = "127.0.0.1", Port = 25566 },
	{ Name = "minigames", Host = "127.0.0.1", Port = 25567 },
	{ Name = "limbo", Host = "127.0.0.1", Port = 25568 }
]
```

## Watchdog
[**Watchdog**](/docs/watchdog) is a built-in HTTP server that allows you to check status or control the proxy.

```toml
// configs/Watchdog/Settings.toml
# Enables the watchdog
Enabled = false

# Defines the network interface to bind on
Address = "*"

# Defines the port
Port = 80
```

## Forwarding
[**Forwarding**](/docs/forwardings/forwarding-overview) helps to forward player data (IP, UUID, Skin, etc.) to the backend server.

### Modern (Velocity)
Read more in [**Modern (Velocity)**](/docs/forwardings/modern) forwarding.

```toml
// configs/Velocity/Settings.toml
# Enables Velocity forwarding
Enabled = true

# The secret key (should be the same as on the backend server)
Secret = "YourSecretKeyHere"
```

## Plugins Installation

Plugins are compiled with the `.dll` extension in any .NET compatible language.
See the [**Plugin Development Kit**](/docs/developing-plugins/development-kit) section for more details.

- Directory `plugins` is the default location to install plugins.
- [**Environment variable**](/docs/configuration/environment-variables/#plugins) `VOID_PLUGINS` might be used to add URLs or Local Paths to run plugins, separated by comma or semicolon.
- [**Program argument**](/docs/configuration/program-arguments#plugins) `--plugin` (alias `-p`) might be used to add URL or Local Path to a plugin.

```bash title="Program Argument Example"
$ ./void-linux-x64 --plugin "/home/YourPlugin1.dll" --plugin "https://example.org/download/YourPlugin2.dll"
```
```bash title="Environment Variable Example"
$ VOID_PLUGINS="https://example.org/download/YourPlugin1.dll;/home/YourPlugin2.dll" ./void-linux-x64
```

## Plugins Configurations (configs/\<Plugin\>/*.toml)

Each plugin may [**define one or a subset of keyed configuration files**](/docs/developing-plugins/configuration/) in its own directory. 
Plugins are not required to save or load configurations manually. 

All changes on the disk are automatically loaded into an existing instance in memory.
Vice versa, all changes in the memory are automatically saved to the disk. This automatic saving can be disabled via [**program argument**](/docs/configuration/program-arguments/#file-configuration).

Currently, only TOML format is supported.
