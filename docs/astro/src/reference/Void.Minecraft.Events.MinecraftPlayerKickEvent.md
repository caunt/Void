# <a id="Void_Minecraft_Events_MinecraftPlayerKickEvent"></a> Class MinecraftPlayerKickEvent

Namespace: [Void.Minecraft.Events](Void.Minecraft.Events.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record MinecraftPlayerKickEvent : PlayerKickEvent, IScopedEventWithResult<bool>, IScopedEvent, IEventWithResult<bool>, IEvent, IEquatable<PlayerKickEvent>, IEquatable<MinecraftPlayerKickEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerKickEvent](Void.Proxy.Api.Events.Player.PlayerKickEvent.md) ← 
[MinecraftPlayerKickEvent](Void.Minecraft.Events.MinecraftPlayerKickEvent.md)

#### Implements

[IScopedEventWithResult<bool\>](Void.Proxy.Api.Events.IScopedEventWithResult\-1.md), 
[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEventWithResult<bool\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<PlayerKickEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IEquatable<MinecraftPlayerKickEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[PlayerKickEvent.Player](Void.Proxy.Api.Events.Player.PlayerKickEvent.md\#Void\_Proxy\_Api\_Events\_Player\_PlayerKickEvent\_Player), 
[PlayerKickEvent.Text](Void.Proxy.Api.Events.Player.PlayerKickEvent.md\#Void\_Proxy\_Api\_Events\_Player\_PlayerKickEvent\_Text), 
[PlayerKickEvent.Result](Void.Proxy.Api.Events.Player.PlayerKickEvent.md\#Void\_Proxy\_Api\_Events\_Player\_PlayerKickEvent\_Result), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Events_MinecraftPlayerKickEvent__ctor_Void_Proxy_Api_Players_IPlayer_Void_Minecraft_Components_Text_Component_"></a> MinecraftPlayerKickEvent\(IPlayer, Component?\)

```csharp
public MinecraftPlayerKickEvent(IPlayer Player, Component? Reason = null)
```

#### Parameters

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Reason` [Component](Void.Minecraft.Components.Text.Component.md)?

## Properties

### <a id="Void_Minecraft_Events_MinecraftPlayerKickEvent_Reason"></a> Reason

```csharp
public Component? Reason { get; init; }
```

#### Property Value

 [Component](Void.Minecraft.Components.Text.Component.md)?

