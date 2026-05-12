# <a id="Void_Minecraft_Events_Chat_AvailableCommandsEvent"></a> Class AvailableCommandsEvent

Namespace: [Void.Minecraft.Events.Chat](Void.Minecraft.Events.Chat.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record AvailableCommandsEvent : IScopedEvent, IEvent, IEquatable<AvailableCommandsEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[AvailableCommandsEvent](Void.Minecraft.Events.Chat.AvailableCommandsEvent.md)

#### Implements

[IScopedEvent](Void.Proxy.Api.Events.IScopedEvent.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<AvailableCommandsEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Events_Chat_AvailableCommandsEvent__ctor_Void_Proxy_Api_Links_ILink_Void_Proxy_Api_Players_IPlayer_Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode_"></a> AvailableCommandsEvent\(ILink, IPlayer, RootCommandNode\)

```csharp
public AvailableCommandsEvent(ILink Link, IPlayer Player, RootCommandNode Node)
```

#### Parameters

`Link` [ILink](Void.Proxy.Api.Links.ILink.md)

`Player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`Node` [RootCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.RootCommandNode.md)

## Properties

### <a id="Void_Minecraft_Events_Chat_AvailableCommandsEvent_Link"></a> Link

```csharp
public ILink Link { get; init; }
```

#### Property Value

 [ILink](Void.Proxy.Api.Links.ILink.md)

### <a id="Void_Minecraft_Events_Chat_AvailableCommandsEvent_Node"></a> Node

```csharp
public RootCommandNode Node { get; init; }
```

#### Property Value

 [RootCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.RootCommandNode.md)

### <a id="Void_Minecraft_Events_Chat_AvailableCommandsEvent_Player"></a> Player

```csharp
public IPlayer Player { get; init; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

