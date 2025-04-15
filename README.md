<h1 align="center">
	<img src="https://github.com/caunt/Void/actions/workflows/dotnet.yml/badge.svg" alt="GitHub Actions" />
	<br>
	VOID
</h1>


**Download:**
<br>
[**Stable Development Builds**](https://github.com/caunt/Void/releases)
<br>
[**Unstable Daily Builds**](https://github.com/caunt/Void/actions)



### Supported / Planned Features:

> [!NOTE]
>
>| Game Version         | Proxying           | Redirects          | API                | WIP                |
>| :------------------- | :----------------: | :----------------: | :----------------: | :----------------: |
>| Java 1.7.2 - 1.21.5  | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
>| Java 1.0.0 - 1.6.4   | :white_check_mark: | :x:                | :x:                | :x:                |
>| Java Beta            | :white_check_mark: | :x:                | :x:                | :x:                |
>| Java Alpha           | :white_check_mark: | :x:                | :x:                | :x:                |
>| Java Classic         | :white_check_mark: | :x:                | :x:                | :x:                |
>| Bedrock Edition      | :x:                | :x:                | :x:                | :x:                |
>| Windows 10 Edition   | :x:                | :x:                | :x:                | :x:                |
>| Pocket Edition       | :x:                | :x:                | :x:                | :x:                |
>| Pocket Edition Alpha | :x:                | :x:                | :x:                | :x:                |

### Mod Loaders
NeoForge / Forge / others are playable since proxy does not manipulate their protocols.
<br>
Redirections definitely won't work with them until full support implemented.

### Forwarding
While it's possible to play in online mode with Void, it is not recommended.
<br>
The proxy allows plugins to choose side of authentication (Server or Proxy).
<br>
Most of the time, you want to authenticate on the proxy side, so you have protocol API working.

| Forwarding        | Supported          | WIP                |
| :---------------- | :----------------: | :----------------: |
| None (Offline)    | :white_check_mark: | :white_check_mark: |
| Legacy (Bungee)   | :x:                | :white_check_mark: |
| Modern (Velocity) | :white_check_mark: | :white_check_mark: |
| Online (PK)       | :white_check_mark: | :white_check_mark: |

#### Online (Private Key) is a new type of forwarding being developed.
It allows to play through proxy on servers that are configured with online mode.
<br>
The only caveat is that you need to install plugin / modification / anything on the server side.
<br>
Server will send it's own private key to the proxy, so proxy can provide protocol API to plugins.


### Plugins
See the [Example Plugin](https://github.com/caunt/Void/blob/main/src/Plugins/ExamplePlugin/ExamplePlugin.cs) on how to create a plugin.
<br>
This is still an early version of the API, so it may change drastically in the future.
<br>
Protocol API, Events, Chat Commands, Text Components, NBT are some of available features.
<br>
Plugin Development Kit and Documentation about plugins are planned.