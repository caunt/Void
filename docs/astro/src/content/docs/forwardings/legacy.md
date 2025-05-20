---
title: Legacy (Bungee)
description: Learn how to configure the Legacy (Bungee) forwarding.
sidebar:
  order: 1
---

:::danger[IN DEVELOPMENT]
This forwarding currently not implemented in Void.
:::

BungeeCord also known as Legacy forwarding is a type of forwarding player data developed by [SpigotMC](https://github.com/SpigotMC/BungeeCord).

:::tip
It is an easiest way to pass player's information from proxy to server in Handshake packet, supported by many server implementations.
Legacy forwarding may include additional data, such as forge FML markers.
:::

:::note[Mods]
BungeeCord forwarding is not supported by mod loaders, however community has created modded server implementations with BungeeCord forwarding, such as [Mohist](https://github.com/MohistMC/Mohist).  

If you are using official Forge server, you can install [**BungeeForge**](https://github.com/caunt/BungeeForge) mod, which adds BungeeCord forwarding support to Forge server.
:::

:::caution[Limitations]
- Servers have to run in offline mode.
- You have to install a plugin, mod, or server with built-in implementation.
- Proxy must authenticate the Mojang player on proxy side.
:::

## Configuration
Currently BungeeCord forwarding does not require configuration.