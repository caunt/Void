# <a id="Void_Minecraft_Components_Text_Colors_TextColor"></a> Class TextColor

Namespace: [Void.Minecraft.Components.Text.Colors](Void.Minecraft.Components.Text.Colors.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record TextColor : IEquatable<TextColor>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TextColor](Void.Minecraft.Components.Text.Colors.TextColor.md)

#### Implements

[IEquatable<TextColor\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Colors_TextColor__ctor_System_Byte_System_Byte_System_Byte_"></a> TextColor\(byte, byte, byte\)

```csharp
public TextColor(byte Red, byte Green, byte Blue)
```

#### Parameters

`Red` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

`Green` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

`Blue` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

## Properties

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_Blue"></a> Blue

```csharp
public byte Blue { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_Green"></a> Green

```csharp
public byte Green { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_Name"></a> Name

```csharp
public string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_Red"></a> Red

```csharp
public byte Red { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

## Methods

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_Downsample"></a> Downsample\(\)

```csharp
public TextColor Downsample()
```

#### Returns

 [TextColor](Void.Minecraft.Components.Text.Colors.TextColor.md)

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_FromString_System_String_"></a> FromString\(string\)

```csharp
public static TextColor FromString(string value)
```

#### Parameters

`value` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [TextColor](Void.Minecraft.Components.Text.Colors.TextColor.md)

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

## Operators

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_op_Implicit_System_ValueTuple_System_Byte_System_Byte_System_Byte___Void_Minecraft_Components_Text_Colors_TextColor"></a> implicit operator TextColor\(\(byte Red, byte Green, byte Blue\)\)

```csharp
public static implicit operator TextColor((byte Red, byte Green, byte Blue) color)
```

#### Parameters

`color` \([byte](https://learn.microsoft.com/dotnet/api/system.byte) [Red](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.byte,system.byte,system.byte\-.red), [byte](https://learn.microsoft.com/dotnet/api/system.byte) [Green](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.byte,system.byte,system.byte\-.green), [byte](https://learn.microsoft.com/dotnet/api/system.byte) [Blue](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.byte,system.byte,system.byte\-.blue)\)

#### Returns

 [TextColor](Void.Minecraft.Components.Text.Colors.TextColor.md)

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_op_Implicit_System_Drawing_Color__Void_Minecraft_Components_Text_Colors_TextColor"></a> implicit operator TextColor\(Color\)

```csharp
public static implicit operator TextColor(Color color)
```

#### Parameters

`color` [Color](https://learn.microsoft.com/dotnet/api/system.drawing.color)

#### Returns

 [TextColor](Void.Minecraft.Components.Text.Colors.TextColor.md)

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_op_Implicit_System_String__Void_Minecraft_Components_Text_Colors_TextColor"></a> implicit operator TextColor\(string\)

```csharp
public static implicit operator TextColor(string color)
```

#### Parameters

`color` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [TextColor](Void.Minecraft.Components.Text.Colors.TextColor.md)

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_op_Implicit_Void_Minecraft_Components_Text_Colors_TextColor__System_Drawing_Color"></a> implicit operator Color\(TextColor\)

```csharp
public static implicit operator Color(TextColor color)
```

#### Parameters

`color` [TextColor](Void.Minecraft.Components.Text.Colors.TextColor.md)

#### Returns

 [Color](https://learn.microsoft.com/dotnet/api/system.drawing.color)

### <a id="Void_Minecraft_Components_Text_Colors_TextColor_op_Implicit_Void_Minecraft_Components_Text_Colors_TextColor__System_String"></a> implicit operator string\(TextColor\)

```csharp
public static implicit operator string(TextColor color)
```

#### Parameters

`color` [TextColor](Void.Minecraft.Components.Text.Colors.TextColor.md)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

