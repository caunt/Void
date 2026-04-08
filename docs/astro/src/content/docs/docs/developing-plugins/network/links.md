---
title: Links (Connections)
description: Learn about ILink lifetime and channels.
sidebar:
  order: 0
---

[**ILink**](/reference/Void.Proxy.Api.Links.ILink) is the core of the Void networking system.
It defines a connection between the player and server.

## Lifetime
An [**ILink**](/reference/Void.Proxy.Api.Links.ILink) is created when a player connects to a server and is destroyed when they disconnect.
Switching to another server replaces the existing link with a new [**ILink**](/reference/Void.Proxy.Api.Links.ILink).
Players do not have an [**ILink**](/reference/Void.Proxy.Api.Links.ILink) during [**PlayerConnectingEvent**](/reference/Void.Proxy.Api.Events.Player.PlayerConnectingEvent) or [**PlayerSearchServerEvent**](/reference/Void.Proxy.Api.Events.Player.PlayerSearchServerEvent).

## Channels
Channels are used to send or receive data from **one** side of the [**ILink**](/reference/Void.Proxy.Api.Links.ILink).
A separate [**INetworkChannel**](/reference/Void.Proxy.Api.Network.Channels.INetworkChannel) is created for [**IPlayer**](/reference/Void.Proxy.Api.Players.IPlayer), and another for [**IServer**](/reference/Void.Proxy.Api.Servers.IServer).

[**ILink**](/reference/Void.Proxy.Api.Links.ILink) always has two channels that remain constant throughout the lifetime of the link: `ILink.PlayerChannel` and `ILink.ServerChannel`.

## Sides
[**ILink**](/reference/Void.Proxy.Api.Links.ILink) saves references to both sides: `ILink.Player` and `ILink.Server`.

## Custom Implementation
Void uses an internal implementation of [**ILink**](/reference/Void.Proxy.Api.Links.ILink) if not overridden by a plugin.

Plugins can override it by providing a custom implementation of [**ILink**](/reference/Void.Proxy.Api.Links.ILink) with [**CreateLinkEvent**](/reference/Void.Proxy.Api.Events.Links.CreateLinkEvent).
```csharp
[Subscribe]
public void OnCreateLink(CreateLinkEvent @event)
{
    @event.Result = new FasterLinkImplementation(@event.Player, @event.Server);
}
```

The job of an [**ILink**](/reference/Void.Proxy.Api.Links.ILink) implementation is to forward [**packets (messages)**](/docs/developing-plugins/network/packets) from the player channel to the server channel and vice versa.
If you want to replace the default implementation, follow [**this example**](https://github.com/caunt/Void/blob/main/src/Platform/Links/Link.cs).
