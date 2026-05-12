# <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor"></a> Class TextShadowColor

Namespace: [Void.Minecraft.Components.Text.Colors](Void.Minecraft.Components.Text.Colors.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record TextShadowColor : IEquatable<TextShadowColor>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)

#### Implements

[IEquatable<TextShadowColor\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor__ctor_System_Byte_System_Byte_System_Byte_System_Byte_"></a> TextShadowColor\(byte, byte, byte, byte\)

```csharp
public TextShadowColor(byte Alpha, byte Red, byte Green, byte Blue)
```

#### Parameters

`Alpha` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

`Red` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

`Green` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

`Blue` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

## Properties

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_Alpha"></a> Alpha

```csharp
public byte Alpha { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_Blue"></a> Blue

```csharp
public byte Blue { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_Green"></a> Green

```csharp
public byte Green { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_Name"></a> Name

```csharp
public string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_Red"></a> Red

```csharp
public byte Red { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

## Methods

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_FromString_System_String_"></a> FromString\(string\)

```csharp
public static TextShadowColor FromString(string value)
```

#### Parameters

`value` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

## Operators

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_op_Implicit_System_ValueTuple_System_Byte_System_Byte_System_Byte_System_Byte___Void_Minecraft_Components_Text_Colors_TextShadowColor"></a> implicit operator TextShadowColor\(\(byte Alpha, byte Red, byte Green, byte Blue\)\)

```csharp
public static implicit operator TextShadowColor((byte Alpha, byte Red, byte Green, byte Blue) color)
```

#### Parameters

`color` \([byte](https://learn.microsoft.com/dotnet/api/system.byte) [Alpha](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.byte,system.byte,system.byte,system.byte\-.alpha), [byte](https://learn.microsoft.com/dotnet/api/system.byte) [Red](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.byte,system.byte,system.byte,system.byte\-.red), [byte](https://learn.microsoft.com/dotnet/api/system.byte) [Green](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.byte,system.byte,system.byte,system.byte\-.green), [byte](https://learn.microsoft.com/dotnet/api/system.byte) [Blue](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.byte,system.byte,system.byte,system.byte\-.blue)\)

#### Returns

 [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_op_Implicit_System_Drawing_Color__Void_Minecraft_Components_Text_Colors_TextShadowColor"></a> implicit operator TextShadowColor\(Color\)

```csharp
public static implicit operator TextShadowColor(Color color)
```

#### Parameters

`color` [Color](https://learn.microsoft.com/dotnet/api/system.drawing.color)

#### Returns

 [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_op_Implicit_System_String__Void_Minecraft_Components_Text_Colors_TextShadowColor"></a> implicit operator TextShadowColor\(string\)

```csharp
public static implicit operator TextShadowColor(string color)
```

#### Parameters

`color` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_op_Implicit_Void_Minecraft_Components_Text_Colors_TextShadowColor__System_Drawing_Color"></a> implicit operator Color\(TextShadowColor\)

```csharp
public static implicit operator Color(TextShadowColor color)
```

#### Parameters

`color` [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)

#### Returns

 [Color](https://learn.microsoft.com/dotnet/api/system.drawing.color)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_op_Implicit_Void_Minecraft_Components_Text_Colors_TextShadowColor__System_String"></a> implicit operator string\(TextShadowColor\)

```csharp
public static implicit operator string(TextShadowColor color)
```

#### Parameters

`color` [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_op_Implicit_Void_Minecraft_Components_Text_Colors_TextShadowColor__System_Int32"></a> implicit operator int\(TextShadowColor\)

```csharp
public static implicit operator int(TextShadowColor color)
```

#### Parameters

`color` [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_op_Implicit_System_Int32__Void_Minecraft_Components_Text_Colors_TextShadowColor"></a> implicit operator TextShadowColor\(int\)

```csharp
public static implicit operator TextShadowColor(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_op_Implicit_Void_Minecraft_Components_Text_Colors_TextShadowColor__System_Single__"></a> implicit operator float\[\]\(TextShadowColor\)

```csharp
public static implicit operator float[](TextShadowColor color)
```

#### Parameters

`color` [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)\[\]

### <a id="Void_Minecraft_Components_Text_Colors_TextShadowColor_op_Implicit_System_Single____Void_Minecraft_Components_Text_Colors_TextShadowColor"></a> implicit operator TextShadowColor\(float\[\]\)

```csharp
public static implicit operator TextShadowColor(float[] components)
```

#### Parameters

`components` [float](https://learn.microsoft.com/dotnet/api/system.single)\[\]

#### Returns

 [TextShadowColor](Void.Minecraft.Components.Text.Colors.TextShadowColor.md)

