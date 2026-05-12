# <a id="Void_Minecraft_Components_Text_Events_ClickEvent"></a> Class ClickEvent

Namespace: [Void.Minecraft.Components.Text.Events](Void.Minecraft.Components.Text.Events.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ClickEvent : IEvent, IEquatable<ClickEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ClickEvent](Void.Minecraft.Components.Text.Events.ClickEvent.md)

#### Implements

[IEvent](Void.Minecraft.Components.Text.Events.IEvent.md), 
[IEquatable<ClickEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Events_ClickEvent__ctor_Void_Minecraft_Components_Text_Events_Actions_IClickEventAction_"></a> ClickEvent\(IClickEventAction\)

```csharp
public ClickEvent(IClickEventAction Content)
```

#### Parameters

`Content` [IClickEventAction](Void.Minecraft.Components.Text.Events.Actions.IClickEventAction.md)

## Properties

### <a id="Void_Minecraft_Components_Text_Events_ClickEvent_ActionName"></a> ActionName

```csharp
public string ActionName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Events_ClickEvent_Content"></a> Content

```csharp
public IClickEventAction Content { get; init; }
```

#### Property Value

 [IClickEventAction](Void.Minecraft.Components.Text.Events.Actions.IClickEventAction.md)

