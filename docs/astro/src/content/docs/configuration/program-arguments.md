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
  --ignore-file-servers         Ignore servers specified in configuration files
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
- `--ignore-file-servers`
  Ignore servers specified in configuration files.

## Version
- `--version`
  Displays the current version of Void Proxy.
  Example: `./void-linux-x64 --version`
