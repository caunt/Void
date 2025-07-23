---
title: Program Arguments
description: Learn how to configure Void Proxy from program arguments.
sidebar:
  order: 2
---

Program arguments make configuration in terminal easier.

## Help
Provides a list of all available program arguments.
```bash
./void-linux-x64 --help
Description:
  Runs the proxy

Usage:
  void-win-x64 [options]

Options:
  -r, --repository <repository>  Provides a URI to NuGet repository [--repository
                                 https://nuget.example.com/v3/index.json or --repository
                                 https://username:password@nuget.example.com/v3/index.json].
  -p, --plugin <plugin>          Provides a path to the file, directory or url to load plugin.
  --server <server>              Registers an additional server in format <host>:<port>
  --interface <address>          Overrides the listening network interface
  --port <port>                  Overrides the listening port
  --ignore-file-servers          Ignore servers specified in configuration files
  --offline                      Allows players to connect without Mojang authorization
  --version                      Show version information
  -?, -h, --help                 Show help and usage information
```

## Plugins
- `--plugin`  
  Allows you to specify additional plugins to load.  
  Example: `./void-linux-x64 --plugin https://example.org/download/YourPlugin1.dll --plugin /home/YourPlugin2.dll`

## NuGet
- `--repository`
  Allows you to specify additional NuGet repositories to use.
  Example: `./void-linux-x64 --repository https://nuget.example.com/v3/index.json`

## Servers
- `--server`
  Registers an additional server in format `<host>:<port>` where port is between `1` and `65535`. IPv6 addresses must be enclosed in square brackets. This option can be used multiple times.
- `--ignore-file-servers`
  Ignore servers specified in configuration files.
  Example: `./void-linux-x64 --server 127.0.0.1:25565 --server [2001:db8::1]:25565`

## Network
- `--interface`
  Overrides the listening network interface.
  Example: `./void-linux-x64 --interface 0.0.0.0`
- `--port`
  Overrides the listening port for the proxy.
  Example: `./void-linux-x64 --port 25570`

## Authentication
- `--offline`
  Allows players to connect without Mojang authorization.
  Example: `./void-linux-x64 --offline`

## Version
- `--version`
  Displays the current version of Void Proxy.
  Example: `./void-linux-x64 --version`
