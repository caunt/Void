---
title: Player Events
description: Learn types of Player Events.
---

Very first event that is triggered when a player connects to the server.  
It is used to create a new player instance and set up the player context with scoped services.  
Do not change `Result` property of this event, until you are very sure what you are doing.  
You can use this event to modify the player instance implementation.
```csharp
class PlayerConnectingEvent(TcpClient Client, Func<IPlayer, IServiceProvider> GetServices) : IEventWithResult<IPlayer>;
```

Tells when a player is connected to the proxy.
```csharp
class PlayerConnectedEvent(IPlayer Player) : IScopedEvent;
```

Tells when a player is disconnected from the proxy.
```csharp
class PlayerDisconnectedEvent(IPlayer Player) : IScopedEvent;
```

Triggers protocol plugins to send `Disconnect` packet to the client.  
Property `Result` tells whether the packet was sent successfully.  
Do not change `Result` property of this event, until you are very sure what you are doing.
```csharp
class PlayerKickEvent(IPlayer Player, string? Text = null) : IScopedEventWithResult<bool>;
```

Helps to determine the server that the player should connect to.  
You can rely on this event to redirect the player to a different server.  
Just close the `ILink.ServerChannel` network channel, and the player instance will start searching for a new server.
```csharp
class PlayerSearchServerEvent(IPlayer Player) : IScopedEventWithResult<IServer>;
```
