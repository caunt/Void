---
title: Features
description: Explore Void proxy supported and features.
sidebar:
  order: 1
---

This page provides an overview of the features supported or planned to implement by Void proxy.

## Minecraft

#### Terms
**Proxying** is a support for playing through proxy.  
**Redirects** is a support for player redirections between servers.  
**API** is a support for Services API in Plugins (Chat, Commands, Events, Content, Packets)  
**WIP** is a Work In Progress.

### Game Versions
| Game Version         | Proxying | Redirects | API      | WIP      |
| :------------------- | :------: | :------:  | :------: | :------: |
| Java 1.7.2 - 1.21.5  | &#x2705; | &#x2705;  | &#x2705; | &#x2705; |
| Java 1.0.0 - 1.7.1   | &#x2705; | &#x274C;  | &#x274C; | &#x274C; |
| Java Beta            | &#x2705; | &#x274C;  | &#x274C; | &#x274C; |
| Java Alpha           | &#x2705; | &#x274C;  | &#x274C; | &#x274C; |
| Java Classic         | &#x2705; | &#x274C;  | &#x274C; | &#x274C; |
| Bedrock Edition      | &#x274C; | &#x274C;  | &#x274C; | &#x274C; |
| Windows 10 Edition   | &#x274C; | &#x274C;  | &#x274C; | &#x274C; |
| Pocket Edition       | &#x274C; | &#x274C;  | &#x274C; | &#x274C; |
| Pocket Edition Alpha | &#x274C; | &#x274C;  | &#x274C; | &#x274C; |

### Forwardings
| Forwarding                                          | Supported | WIP      |
| :-------------------------------------------------- | :------:  | :------: |
| [**None (Offline)**](../../forwardings/comparison)  | &#x2705;  | &#x2705; |
| [**Legacy (Bungee)**](../../forwardings/bungee)     | &#x274C;  | &#x2705; |
| [**Modern (Velocity)**](../../forwardings/velocity) | &#x2705;  | &#x2705; |
| [**Online (PK)**](../../forwardings/online)         | &#x2705;  | &#x2705; |

:::tip[Online (Private Key)]
A new type of [**forwarding**](../../forwardings/online) being developed.  
It allows to play through the proxy on servers that are configured with online mode.  
Server must safely share its own private key, so proxy can provide API to plugins.  
If no key provided, you still can play through proxy, but API will be very limited.
:::

### Mod Loaders
| Type       | Proxying | Redirects | WIP      |
| :--------- | :------: | :------:  | :------: |
| NeoForge   | &#x2705; | &#x274C;  | &#x2705; |
| Forge      | &#x2705; | &#x274C;  | &#x2705; |
| Fabric     | &#x2705; | &#x274C;  | &#x274C; |
| Quilt      | &#x2705; | &#x274C;  | &#x274C; |
| LiteLoader | &#x2705; | &#x274C;  | &#x274C; |

## Other Games
Currently Minecraft is the only game supported by Void proxy.  
We are planning to add support for other games in the future.  
However, they might work already, but are limited to TCP protocol.