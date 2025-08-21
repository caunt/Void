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
  void-linux-x64 [options]

Options:
  -r, --repository <repository>                                    Provides a URI to NuGet repository [--repository https://nuget.example.com/v3/index.json or --repository https://username:password@nuget.example.com/v3/index.json].
  -p, --plugin <plugin>                                            Provides a path to the file, directory or URL to load plugin.
  --ignore-file-servers                                            Ignore servers specified in configuration files
  --server <server>                                                Registers an additional server in format <host>:<port>
  --interface <interface>                                          Sets the listening network interface
  --port <port>                                                    Sets the listening port
  --offline                                                        Allows players to connect without Mojang authorization
  --logging <Critical|Debug|Error|Information|None|Trace|Warning>  Sets the logging level
  --version                                                        Show version information
  -?, -h, --help                                                   Show help and usage information
```

## Authentication
- `--offline`  
  Allows players to connect without Mojang authentication.

  ```bash title="Example Usage"
  ./void-linux-x64 --offline
  ```

## Network
- `--interface`  
  Overrides the listening network interface.
- `--port`  
  Overrides the listening port for the proxy.

  ```bash title="Example Usage"
  ./void-linux-x64 \
    --interface 0.0.0.0 \
    --port 25565
  ```

## Servers
- `--server`  
  Registers a server in format `<host>:<port>` where the port is between `1` and `65535`. IPv6 addresses must be enclosed in square brackets.
- `--ignore-file-servers`
  Ignore servers specified in [**configuration files**](/docs/configuration/in-file).

  ```bash title="Example Usage"
  ./void-linux-x64 \
    --ignore-file-servers \
    --server 127.0.0.1:25565 \
    --server [2001:db8::1]:25565
  ```

## Plugins
- `--plugin`  
  Allows you to specify plugins to load.
- `--repository`  
  Allows you to specify NuGet repositories to resolve plugin dependencies.

  ```bash title="Example Usage"
  ./void-linux-x64 \
    --plugin https://example.org/download/YourPlugin1.dll \
    --plugin /home/YourPlugin2.dll \
    --repository https://nuget.example.com/v3/index.json
  ```

## Logging
- `--logging`
  Sets the logging level. Valid values are Trace, Debug, Information, Warning, Error and Critical.

  ```bash title="Example Usage"
  ./void-linux-x64 --logging Debug
  ```

## Version
- `--version`  
  Displays the current version of Void Proxy.

  ```bash title="Example"
  ./void-linux-x64 --version
  ```
