# <a id="Void_Minecraft_Components_Text_Events_HoverEvent"></a> Class HoverEvent

Namespace: [Void.Minecraft.Components.Text.Events](Void.Minecraft.Components.Text.Events.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record HoverEvent : IEvent, IEquatable<HoverEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[HoverEvent](Void.Minecraft.Components.Text.Events.HoverEvent.md)

#### Implements

[IEvent](Void.Minecraft.Components.Text.Events.IEvent.md), 
[IEquatable<HoverEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Events_HoverEvent__ctor_Void_Minecraft_Components_Text_Events_Actions_IHoverEventAction_"></a> HoverEvent\(IHoverEventAction\)

```csharp
public HoverEvent(IHoverEventAction Content)
```

#### Parameters

`Content` [IHoverEventAction](Void.Minecraft.Components.Text.Events.Actions.IHoverEventAction.md)

## Properties

### <a id="Void_Minecraft_Components_Text_Events_HoverEvent_ActionName"></a> ActionName

```csharp
public string ActionName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Events_HoverEvent_Content"></a> Content

```csharp
public IHoverEventAction Content { get; init; }
```

#### Property Value

 [IHoverEventAction](Void.Minecraft.Components.Text.Events.Actions.IHoverEventAction.md)

