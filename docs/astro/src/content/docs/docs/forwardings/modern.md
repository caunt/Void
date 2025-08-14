---
title: Modern (Velocity)
description: Learn how to configure the Modern (Velocity) forwarding.
sidebar:
  order: 2
---

Velocity, also known as Modern forwarding, is a type of forwarding player data developed by [PaperMC](https://docs.papermc.io/velocity/player-information-forwarding/).

:::tip
It is a secure way to pass player information from the proxy to the server, supported by many server implementations.
Modern forwarding was updated multiple times, enabling new features and fixing bugs.
:::

:::note[Mods]
Velocity forwarding is not supported by [**mod loaders**](/docs/getting-started/features/#mod-loaders); however, the community has created mods implementing the protocol, such as [Proxy Compatible Forge](https://github.com/adde0109/Proxy-Compatible-Forge).
:::

:::caution[Limitations]
- Servers have to run in offline mode.
- You have to install a plugin, mod, or server with built-in implementation.
- Proxy must authenticate the Mojang player on proxy side.
:::

## Configuration
To configure the Velocity forwarding, you need to set up a 'Secret' key on both the proxy and server sides.
Check the [**In-file configuration**](/docs/configuration/in-file/#modern-velocity) for more details.
