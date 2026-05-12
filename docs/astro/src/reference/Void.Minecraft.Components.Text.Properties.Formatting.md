# <a id="Void_Minecraft_Components_Text_Properties_Formatting"></a> Class Formatting

Namespace: [Void.Minecraft.Components.Text.Properties](Void.Minecraft.Components.Text.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record Formatting : IEquatable<Formatting>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Formatting](Void.Minecraft.Components.Text.Properties.Formatting.md)

#### Implements

[IEquatable<Formatting\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Properties_Formatting__ctor_Void_Minecraft_Components_Text_Colors_TextColor_Void_Minecraft_Components_Text_Colors_TextShadowColor_System_String_System_Nullable_System_Boolean__System_Nullable_System_Boolean__System_Nullable_System_Boolean__System_Nullable_System_Boolean__System_Nullable_System_Boolean__"></a> Formatting\(TextColor?, TextShadowColor?, string?, bool?, bool?, bool?, bool?, bool?\)

```csharp
public Formatting(TextColor? Color = null, TextShadowColor? ShadowColor = null, string? Font = null, bool? IsBold = null, bool? IsItalic = null, bool? IsUnderlined = null, bool? IsStrikethrough = null, bool? IsObfuscated = null)
```

#### Parameters

`Color` [TextColor](Void.Minecraft.Components.Text.Colors.TextColor.md)?

`ShadowColor` [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)?

`Font` [string](https://learn.microsoft.com/dotnet/api/system.string)?

`IsBold` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

`IsItalic` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

`IsUnderlined` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

`IsStrikethrough` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

`IsObfuscated` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

## Properties

### <a id="Void_Minecraft_Components_Text_Properties_Formatting_Color"></a> Color

```csharp
public TextColor? Color { get; init; }
```

#### Property Value

 [TextColor](Void.Minecraft.Components.Text.Colors.TextColor.md)?

### <a id="Void_Minecraft_Components_Text_Properties_Formatting_Default"></a> Default

```csharp
public static Formatting Default { get; }
```

#### Property Value

 [Formatting](Void.Minecraft.Components.Text.Properties.Formatting.md)

### <a id="Void_Minecraft_Components_Text_Properties_Formatting_Font"></a> Font

```csharp
public string? Font { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Minecraft_Components_Text_Properties_Formatting_IsBold"></a> IsBold

```csharp
public bool? IsBold { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

### <a id="Void_Minecraft_Components_Text_Properties_Formatting_IsItalic"></a> IsItalic

```csharp
public bool? IsItalic { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

### <a id="Void_Minecraft_Components_Text_Properties_Formatting_IsObfuscated"></a> IsObfuscated

```csharp
public bool? IsObfuscated { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

### <a id="Void_Minecraft_Components_Text_Properties_Formatting_IsStrikethrough"></a> IsStrikethrough

```csharp
public bool? IsStrikethrough { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

### <a id="Void_Minecraft_Components_Text_Properties_Formatting_IsUnderlined"></a> IsUnderlined

```csharp
public bool? IsUnderlined { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

### <a id="Void_Minecraft_Components_Text_Properties_Formatting_ShadowColor"></a> ShadowColor

```csharp
public TextShadowColor? ShadowColor { get; init; }
```

#### Property Value

 [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)?

