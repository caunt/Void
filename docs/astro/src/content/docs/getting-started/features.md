---
title: Features
description: Explore the features supported by the Void proxy.
sidebar:
  order: 1
---

This page provides an overview of the features supported or planned to be implemented by the Void proxy.

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
| Forwarding                                                  | Supported | WIP      |
| :---------------------------------------------------------- | :------:  | :------: |
| [**None (Offline)**](/forwardings/forwarding-overview)      | &#x2705;  | &#x2705; |
| [**Legacy (Bungee)**](/forwardings/legacy)                  | &#x274C;  | &#x2705; |
| [**Modern (Velocity)**](/forwardings/modern)                | &#x2705;  | &#x2705; |
| [**Online (PK)**](/forwardings/online)                      | &#x2705;  | &#x2705; |

:::tip[Online (Private Key)]
Online (Private Key) is a new [**forwarding**](/forwardings/online) method.
It lets you connect to servers using online mode.
The server securely shares its private key so the proxy can expose an API to plugins.
Without the key you can still play, but plugin features are limited.
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
Currently, Minecraft is the only game supported by the Void proxy.
We are planning to add support for other games in the future.
However, they might already work but are limited to the TCP protocol.
