---
title: Player Events
description: Learn types of Player Events.
---

[**PlayerConnectingEvent**](/reference/Void.Proxy.Api.Events.Player.PlayerConnectingEvent) is the very first event that is triggered when a player connects to the server.
It is used to create a new player instance and set up the player context with [**scoped services**](/docs/developing-plugins/services/scoped).
Do not change the `Result` property of this event until you are very sure what you are doing.
You can use this event to modify the player instance implementation.
```csharp
class PlayerConnectingEvent(TcpClient Client, Func<IPlayer, IServiceProvider> GetServices) : IEventWithResult<IPlayer>;
```

[**PlayerConnectedEvent**](/reference/Void.Proxy.Api.Events.Player.PlayerConnectedEvent) tells when a player is connected to the proxy.
```csharp
class PlayerConnectedEvent(IPlayer Player) : IScopedEvent;
```

[**PlayerDisconnectedEvent**](/reference/Void.Proxy.Api.Events.Player.PlayerDisconnectedEvent) tells when a player is disconnected from the proxy.
```csharp
class PlayerDisconnectedEvent(IPlayer Player) : IScopedEvent;
```

[**PlayerKickEvent**](/reference/Void.Proxy.Api.Events.Player.PlayerKickEvent) triggers protocol plugins to send the `Disconnect` packet to the client.
Property `Result` tells whether the packet was sent successfully.
Do not change the `Result` property of this event until you are very sure what you are doing.
```csharp
class PlayerKickEvent(IPlayer Player, string? Text = null) : IScopedEventWithResult<bool>;
```

[**PlayerSearchServerEvent**](/reference/Void.Proxy.Api.Events.Player.PlayerSearchServerEvent) helps determine the server that the player should connect to.
You can rely on this event to redirect the player to a different server.  
Just close the `ILink.ServerChannel` network channel, and the player instance will start searching for a new server.
```csharp
class PlayerSearchServerEvent(IPlayer Player) : IScopedEventWithResult<IServer>;
```
