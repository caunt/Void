---
title: Links (Connections)
description: Learn about ILink lifetime and channels.
sidebar:
  order: 0
---

[**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) is the core of the Void networking system.
They define a connection between the player and server.

## Lifetime
An [**ILink**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) is created when a player connects to a server and destroyed when they disconnect.
Switching to another server replaces the existing link with a new [**ILink**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs).
Players do not have an [**ILink**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) during `PlayerConnectingEvent` or `PlayerSearchServerEvent`.

## Channels
Channels are used to send or receive data from **one** side of the [**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs).
A separate `INetworkChannel` is created for `IPlayer`, and another for `IServer`.

[**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) always has two channels that remain constant throughout the lifetime of the link: `ILink.PlayerChannel` and `ILink.ServerChannel`.

## Sides
[**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) saves references to both sides: `ILink.Player` and `ILink.Server`.

## Custom Implementation
Void uses an internal implementation of [**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) if not overridden by a plugin.

Plugins can override it by providing a custom implementation of [**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) with `CreateLinkEvent`.
```csharp
[Subscribe]
public void OnCreateLink(CreateLinkEvent @event)
{
    @event.Result = new FasterLinkImplementation(@event.Player, @event.Server);
}
```

The job of an [**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) implementation is to forward [**packets (messages)**](/docs/developing-plugins/network/packets) from the player channel to the server channel and vice versa.
If you want to replace the default implementation, follow [**this example**](https://github.com/caunt/Void/blob/main/src/Platform/Links/Link.cs).
