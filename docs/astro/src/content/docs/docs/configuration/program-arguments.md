---
title: Program Arguments
description: Learn how to configure Void Proxy from program arguments.
sidebar:
  order: 2
---

Program arguments make configuration in the terminal easier.

## Help
Provides a list of all available program arguments.
```bash
./void-linux-x64 --help
Description:
  Runs the proxy

Usage:
  void-linux-x64 [options]

Options:
  -?, -h, --help                                         Show help and usage information
  -o, --override                                         Register an additional server override to redirect players based on
                                                         hostname they are connecting with.
  -p, --plugin                                           Provides a path to the file, directory or URL to load plugin.
  -r, --repository                                       Provides a URI to NuGet repository.
  --forwarding-modern-key                                Sets the secret key for modern forwarding
  --ignore-file-servers                                  Ignore servers specified in configuration files
  --interface                                            Sets the listening network interface
  --logging                                              Sets the logging level
  <Critical|Debug|Error|Information|None|Trace|Warning>
  --offline                                              Allows players to connect without Mojang authorization
  --port                                                 Sets the listening port
  --read-only                                            Disables saving changes to the configuration files
  --server                                               Registers an additional server in format <host>:<port>
  --version                                              Show version information
```

## Authentication
- `--offline`  
  Allows players to connect without Mojang authentication.

  ```bash title="Example Usage"
  ./void-linux-x64 --offline
  ```

## File Configuration
- `--read-only`  
  Disables automatically saving changes to the [**configuration files**](/docs/configuration/in-file).

  ```bash title="Example Usage"
  ./void-linux-x64 --read-only
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
  Registers a server in the format `<host>:<port>` or `<host>` (port defaults to 25565). The port must be between `1` and `65535`. IPv6 addresses must be enclosed in square brackets.
- `--ignore-file-servers`
  Ignore servers specified in [**configuration files**](/docs/configuration/in-file).
- `--override` (short: `-o`)  
  Register an additional server override to redirect players based on the hostname they are connecting with. The format is `<hostname>=<server-name>`. This allows you to route players to different servers depending on the domain or subdomain they use to connect.

  ```bash title="Example Usage"
  ./void-linux-x64 \
    --ignore-file-servers \
    --server 127.0.0.1:25565 \
    --server [2001:db8::1] \
    --server [2001:db8::1]:25566 \
    --server paper-server.default.svc.cluster.local
  ```

  ```bash title="Example with Override"
  # Redirect players connecting via vanilla.example.org to a specific server
  ./void-linux-x64 \
    --ignore-file-servers \
    --server 127.0.0.1:25565 \
    --override vanilla.example.org=args-server-1
  ```

  ```bash title="Example with File-configured Server"
  # If you have a server named 'lobby' configured in file
  ./void-linux-x64 \
    --override vanilla.example.org=lobby
  ```

## Forwarding
- `--forwarding-modern-key`
  Sets the secret key for [**Modern (Velocity)**](/docs/forwardings/modern) forwarding.

  ```bash title="Example Usage"
  ./void-linux-x64 --forwarding-modern-key YourSecretKeyHere
  ```

## Plugins
- `--plugin`
  Allows you to specify plugins to load.
- `--repository`
  Provides a URI to NuGet repository. You can specify multiple repositories and include credentials in the URI using the format `https://username:password@nuget.example.com/v3/index.json`.

  ```bash title="Example Usage"
  ./void-linux-x64 \
    --plugin https://example.org/download/YourPlugin1.dll \
    --plugin /home/YourPlugin2.dll \
    --repository https://nuget.example.com/v3/index.json
  ```

  ```bash title="Example with Authenticated Repository"
  ./void-linux-x64 \
    --plugin /home/YourPlugin.dll \
    --repository https://username:password@nuget.example.com/v3/index.json
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
