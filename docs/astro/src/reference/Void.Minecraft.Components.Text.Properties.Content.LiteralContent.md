# <a id="Void_Minecraft_Components_Text_Properties_Content_LiteralContent"></a> Class LiteralContent

Namespace: [Void.Minecraft.Components.Text.Properties.Content](Void.Minecraft.Components.Text.Properties.Content.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record LiteralContent : IContent, IEquatable<LiteralContent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[LiteralContent](Void.Minecraft.Components.Text.Properties.Content.LiteralContent.md)

#### Implements

[IContent](Void.Minecraft.Components.Text.Properties.Content.IContent.md), 
[IEquatable<LiteralContent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Properties_Content_LiteralContent__ctor_Void_Minecraft_Nbt_NbtTag_"></a> LiteralContent\(NbtTag\)

```csharp
public LiteralContent(NbtTag Value)
```

#### Parameters

`Value` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

## Properties

### <a id="Void_Minecraft_Components_Text_Properties_Content_LiteralContent_Type"></a> Type

```csharp
public string Type { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Properties_Content_LiteralContent_Value"></a> Value

```csharp
public NbtTag Value { get; init; }
```

#### Property Value

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

