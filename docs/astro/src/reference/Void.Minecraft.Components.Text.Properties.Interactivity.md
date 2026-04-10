# <a id="Void_Minecraft_Components_Text_Properties_Interactivity"></a> Class Interactivity

Namespace: [Void.Minecraft.Components.Text.Properties](Void.Minecraft.Components.Text.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record Interactivity : IEquatable<Interactivity>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Interactivity](Void.Minecraft.Components.Text.Properties.Interactivity.md)

#### Implements

[IEquatable<Interactivity\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Properties_Interactivity__ctor_System_String_Void_Minecraft_Components_Text_Events_ClickEvent_Void_Minecraft_Components_Text_Events_HoverEvent_"></a> Interactivity\(string?, ClickEvent?, HoverEvent?\)

```csharp
public Interactivity(string? Insertion = null, ClickEvent? ClickEvent = null, HoverEvent? HoverEvent = null)
```

#### Parameters

`Insertion` [string](https://learn.microsoft.com/dotnet/api/system.string)?

`ClickEvent` [ClickEvent](Void.Minecraft.Components.Text.Events.ClickEvent.md)?

`HoverEvent` [HoverEvent](Void.Minecraft.Components.Text.Events.HoverEvent.md)?

## Properties

### <a id="Void_Minecraft_Components_Text_Properties_Interactivity_ClickEvent"></a> ClickEvent

```csharp
public ClickEvent? ClickEvent { get; init; }
```

#### Property Value

 [ClickEvent](Void.Minecraft.Components.Text.Events.ClickEvent.md)?

### <a id="Void_Minecraft_Components_Text_Properties_Interactivity_Default"></a> Default

```csharp
public static Interactivity Default { get; }
```

#### Property Value

 [Interactivity](Void.Minecraft.Components.Text.Properties.Interactivity.md)

### <a id="Void_Minecraft_Components_Text_Properties_Interactivity_HoverEvent"></a> HoverEvent

```csharp
public HoverEvent? HoverEvent { get; init; }
```

#### Property Value

 [HoverEvent](Void.Minecraft.Components.Text.Events.HoverEvent.md)?

### <a id="Void_Minecraft_Components_Text_Properties_Interactivity_Insertion"></a> Insertion

```csharp
public string? Insertion { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

