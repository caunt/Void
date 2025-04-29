# Configuration

File configurations and environment variables are primary ways of configuration.
However many options may be rewritten in runtime using the API from Plugins.

## Proxy (settings.toml)

```ini
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

## Plugins Installation

Plugins are compiled with the *.dll extension in any .NET compatible language.
See the [Plugins overview](../developing-plugins/overview.md) section for more details.

Directory `plugins` is the default location for plugins.
Environment variable `PLUGINS` might be used to include URLs or Local Paths to plugins, separated by coma or semicolon.
Same applies to `--plugins` command line argument.

Examples:
```bash
$ ./void-linux-x64 --plugins "/home/YourPlugin1.dll;/home/YourPlugin2.dll"

$ PLUGINS="/home/YourPlugin1.dll;/home/YourPlugin2.dll" ./void-linux-x64
```

## Plugins Configurations (configs/\<Plugin\>/*.toml)

Each plugin may define one or subset of keyed configuration files in its own directory. 
Plugins are not required to save or load configurations manually. 

All changes on the disk are automatically loaded into existing instance in the memory.
Vice versa, all changes in the memory are automatically saved to the disk.

Currently, only TOML format is supported.