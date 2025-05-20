---
title: Velocity (Modern)
description: Learn how to configure the Velocity (Modern) forwarding.
sidebar:
  order: 2
---

Velocity also known as Modern forwarding is a type of forwarding player data developed by [PaperMC](https://docs.papermc.io/velocity/player-information-forwarding/).

:::tip
It is a secure way to pass player's information from proxy to server, supported by many server implementations.
Modern forwarding was updated multiple times, enabling new features and fixing bugs.
:::

:::note[Mods]
Modern forwarding does not support mods officially, but community has created some mods implementing it's protocol, such as [Proxy Compatible Forge](https://github.com/adde0109/Proxy-Compatible-Forge).
:::

:::caution[Limitations]
- Servers have to run in offline mode
- You have to install a plugin, mod, or server with built-in implementation
- Proxy must authenticate the Mojang player on proxy side
:::

## Configuration
To configure the Velocity forwarding, you need to set up 'Secret' key on both proxy and servers sides.
Check the [**In-file configuration**](/configuration/in-file/#velocity-modern) for more details.