# <a id="Void_Minecraft_Components_Text_Properties_Content_NbtContent"></a> Class NbtContent

Namespace: [Void.Minecraft.Components.Text.Properties.Content](Void.Minecraft.Components.Text.Properties.Content.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record NbtContent : IContent, IEquatable<NbtContent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtContent](Void.Minecraft.Components.Text.Properties.Content.NbtContent.md)

#### Implements

[IContent](Void.Minecraft.Components.Text.Properties.Content.IContent.md), 
[IEquatable<NbtContent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Properties_Content_NbtContent__ctor_System_String_System_String_System_Nullable_System_Boolean__Void_Minecraft_Components_Text_Component_System_String_System_String_System_String_"></a> NbtContent\(string, string?, bool?, Component?, string?, string?, string?\)

```csharp
public NbtContent(string Path, string? Source = null, bool? Interpret = null, Component? Separator = null, string? Block = null, string? Entity = null, string? Storage = null)
```

#### Parameters

`Path` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Source` [string](https://learn.microsoft.com/dotnet/api/system.string)?

`Interpret` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

`Separator` [Component](Void.Minecraft.Components.Text.Component.md)?

`Block` [string](https://learn.microsoft.com/dotnet/api/system.string)?

`Entity` [string](https://learn.microsoft.com/dotnet/api/system.string)?

`Storage` [string](https://learn.microsoft.com/dotnet/api/system.string)?

## Properties

### <a id="Void_Minecraft_Components_Text_Properties_Content_NbtContent_Block"></a> Block

```csharp
public string? Block { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Minecraft_Components_Text_Properties_Content_NbtContent_Entity"></a> Entity

```csharp
public string? Entity { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Minecraft_Components_Text_Properties_Content_NbtContent_Interpret"></a> Interpret

```csharp
public bool? Interpret { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

### <a id="Void_Minecraft_Components_Text_Properties_Content_NbtContent_Path"></a> Path

```csharp
public string Path { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Properties_Content_NbtContent_Separator"></a> Separator

```csharp
public Component? Separator { get; init; }
```

#### Property Value

 [Component](Void.Minecraft.Components.Text.Component.md)?

### <a id="Void_Minecraft_Components_Text_Properties_Content_NbtContent_Source"></a> Source

```csharp
public string? Source { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Minecraft_Components_Text_Properties_Content_NbtContent_Storage"></a> Storage

```csharp
public string? Storage { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Minecraft_Components_Text_Properties_Content_NbtContent_Type"></a> Type

```csharp
public string Type { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

