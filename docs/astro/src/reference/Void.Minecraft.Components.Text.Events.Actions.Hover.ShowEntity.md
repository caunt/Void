# <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowEntity"></a> Class ShowEntity

Namespace: [Void.Minecraft.Components.Text.Events.Actions.Hover](Void.Minecraft.Components.Text.Events.Actions.Hover.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ShowEntity : IHoverEventAction, IEquatable<ShowEntity>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ShowEntity](Void.Minecraft.Components.Text.Events.Actions.Hover.ShowEntity.md)

#### Implements

[IHoverEventAction](Void.Minecraft.Components.Text.Events.Actions.IHoverEventAction.md), 
[IEquatable<ShowEntity\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowEntity__ctor_Void_Minecraft_Profiles_Uuid_System_String_Void_Minecraft_Components_Text_Component_"></a> ShowEntity\(Uuid, string?, Component?\)

```csharp
public ShowEntity(Uuid Id, string? Type = null, Component? Name = null)
```

#### Parameters

`Id` [Uuid](Void.Minecraft.Profiles.Uuid.md)

`Type` [string](https://learn.microsoft.com/dotnet/api/system.string)?

`Name` [Component](Void.Minecraft.Components.Text.Component.md)?

## Properties

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowEntity_ActionName"></a> ActionName

```csharp
public string ActionName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowEntity_Id"></a> Id

```csharp
public Uuid Id { get; init; }
```

#### Property Value

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowEntity_Name"></a> Name

```csharp
public Component? Name { get; init; }
```

#### Property Value

 [Component](Void.Minecraft.Components.Text.Component.md)?

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowEntity_Type"></a> Type

```csharp
public string? Type { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

