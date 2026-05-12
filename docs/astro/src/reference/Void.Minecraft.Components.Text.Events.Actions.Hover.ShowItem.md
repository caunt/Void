# <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowItem"></a> Class ShowItem

Namespace: [Void.Minecraft.Components.Text.Events.Actions.Hover](Void.Minecraft.Components.Text.Events.Actions.Hover.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ShowItem : IHoverEventAction, IEquatable<ShowItem>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ShowItem](Void.Minecraft.Components.Text.Events.Actions.Hover.ShowItem.md)

#### Implements

[IHoverEventAction](Void.Minecraft.Components.Text.Events.Actions.IHoverEventAction.md), 
[IEquatable<ShowItem\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowItem__ctor_System_String_System_Nullable_System_Int32__Void_Minecraft_Nbt_Tags_NbtCompound_"></a> ShowItem\(string, int?, NbtCompound?\)

```csharp
public ShowItem(string Id, int? Count = null, NbtCompound? ItemComponents = null)
```

#### Parameters

`Id` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Count` [int](https://learn.microsoft.com/dotnet/api/system.int32)?

`ItemComponents` [NbtCompound](Void.Minecraft.Nbt.Tags.NbtCompound.md)?

## Properties

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowItem_ActionName"></a> ActionName

```csharp
public string ActionName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowItem_Count"></a> Count

```csharp
public int? Count { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)?

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowItem_Id"></a> Id

```csharp
public string Id { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowItem_ItemComponents"></a> ItemComponents

```csharp
public NbtCompound? ItemComponents { get; init; }
```

#### Property Value

 [NbtCompound](Void.Minecraft.Nbt.Tags.NbtCompound.md)?

