<h1 align="center">
  <a href="https://github.com/caunt/Void/actions">
    <img src="https://github.com/caunt/Void/actions/workflows/main.yaml/badge.svg" alt="GitHub Actions">
  </a>
  <br>
  VOID
</h1>

**Download:**
<br>
[**Stable Development Builds**](https://github.com/caunt/Void/releases)
<br>
[**Unstable Daily Builds**](https://github.com/caunt/Void/actions)

**Getting Started:**
<br>
[**Open Documentation**](https://void.caunt.world)

### Supported / Planned Features:

> [!NOTE]
>
>| Game Version         | Proxying           | Redirects          | API                | WIP                |
>| :------------------- | :----------------: | :----------------: | :----------------: | :----------------: |
>| Java 1.7.2 - 1.21.5  | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
>| Java 1.0.0 - 1.7.1   | :white_check_mark: | :x:                | :x:                | :x:                |
>| Java Beta            | :white_check_mark: | :x:                | :x:                | :x:                |
>| Java Alpha           | :white_check_mark: | :x:                | :x:                | :x:                |
>| Java Classic         | :white_check_mark: | :x:                | :x:                | :x:                |
>| Bedrock Edition      | :x:                | :x:                | :x:                | :x:                |
>| Windows 10 Edition   | :x:                | :x:                | :x:                | :x:                |
>| Pocket Edition       | :x:                | :x:                | :x:                | :x:                |
>| Pocket Edition Alpha | :x:                | :x:                | :x:                | :x:                |

### Mod Loaders
NeoForge / Forge / others are playable since the proxy does not manipulate their protocols.
<br>
Redirections definitely won't work with them until full support is implemented.

### Forwarding
The proxy allows plugins to choose the side of authentication (Server or Proxy).
<br>
Most of the time, you want to authenticate on the proxy side, so you have protocol API working.

| Forwarding        | Supported          | WIP                |
| :---------------- | :----------------: | :----------------: |
| None (Offline)    | :white_check_mark: | :white_check_mark: |
| Legacy (Bungee)   | :x:                | :white_check_mark: |
| Modern (Velocity) | :x:                | :white_check_mark: |
| Online (PK)       | :white_check_mark: | :white_check_mark: |

#### Online (Private Key) is a new type of forwarding being developed.
It allows to play through the proxy on servers that are configured with online mode.
<br>
The only caveat is that you need to install plugin / modification / anything on the server side.
<br>
Server will send its own private key to the proxy, so proxy can provide protocol API to plugins.
<br>
If no key provided, you still can play through proxy, but protocol API will be very limited.


### Developing Plugins
Download [**Plugin DevKit**](https://github.com/caunt/Void/releases/latest/download/plugin-devkit.zip) and open it with your IDE.
<br>
<br>
See the [Example Plugin](https://github.com/caunt/Void/blob/main/src/Plugins/ExamplePlugin/ExamplePlugin.cs) to learn how to create a plugin.
<br>
This is still an early version of the API, so it may change drastically in the future.
<br>
Protocol API, Events, Chat Commands, Text Components, NBT are some of the available features.
<br>
Plugin Development Kit and Documentation about plugins are planned.
