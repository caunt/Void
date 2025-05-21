---
title: Links (Connections)
description: Learn about ILink lifetime and channels.
sidebar:
  order: 0
---

[**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) is a core of Void networking system.
They define a connection between the player and server.

## Lifetime
[**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) is created whenever a player finds a server to connect to.  
Players do not have any link at `PlayerConnectingEvent` event or `PlayerSearchServerEvent` event.

[**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) is disposed when the player disconnects from the server.  
However, the player might be just redirecting to another server, so **new** link will be created for each server connection player makes.

## Channels
Channels are used to send or receive data from **one** side of the link.
So separate `INetworkChannel` is created for `IPlayer` and separate `INetworkChannel` is created for `IServer`.

[**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) always has 2 channels that never change in whole lifetime of the link: `ILink.PlayerChannel` and `ILink.ServerChannel`.

## Sides
[**`ILink`**](https://github.com/caunt/Void/blob/main/src/Api/Links/ILink.cs) saves references to both sides: `ILink.Player` and `ILink.Server`.

## Custom Implementation
Void uses internal implementation of `ILink` if not overriden by plugin.

Plugin can override it by providing custom implementation of `ILink` with `CreateLinkEvent` event.
```csharp
[Subscribe]
public void OnCreateLink(CreateLinkEvent @event)
{
    @event.Result = new FasterLinkImplementation(@event.Player, @event.Server);
}
```

All the job for `ILink` implementation is to forward packets (messages) from player channel to server channel and vice versa.
If you want to replace the default implementation, follow [**this example**](https://github.com/caunt/Void/blob/main/src/Platform/Links/Link.cs).