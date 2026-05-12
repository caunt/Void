# <a id="Void_Minecraft_Commands_Brigadier_StringReader"></a> Class StringReader

Namespace: [Void.Minecraft.Commands.Brigadier](Void.Minecraft.Commands.Brigadier.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class StringReader : IImmutableStringReader
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

#### Implements

[IImmutableStringReader](Void.Minecraft.Commands.Brigadier.IImmutableStringReader.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_StringReader__ctor_System_String_System_Int32_"></a> StringReader\(string, int\)

```csharp
public StringReader(string source, int cursor = 0)
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

`cursor` [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader__ctor_Void_Minecraft_Commands_Brigadier_StringReader_"></a> StringReader\(StringReader\)

```csharp
public StringReader(StringReader reader)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_CanRead"></a> CanRead

```csharp
public bool CanRead { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_Cursor"></a> Cursor

```csharp
public int Cursor { get; set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_Peek"></a> Peek

```csharp
public char Peek { get; }
```

#### Property Value

 [char](https://learn.microsoft.com/dotnet/api/system.char)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_Read"></a> Read

```csharp
public string Read { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_Remaining"></a> Remaining

```csharp
public string Remaining { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_RemainingLength"></a> RemainingLength

```csharp
public int RemainingLength { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_Source"></a> Source

```csharp
public string Source { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_TotalLength"></a> TotalLength

```csharp
public int TotalLength { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_CanReadLength_System_Int32_"></a> CanReadLength\(int\)

```csharp
public bool CanReadLength(int length)
```

#### Parameters

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_Expect_System_Char_"></a> Expect\(char\)

```csharp
public void Expect(char character)
```

#### Parameters

`character` [char](https://learn.microsoft.com/dotnet/api/system.char)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_IsAllowedInUnquotedString_System_Char_"></a> IsAllowedInUnquotedString\(char\)

```csharp
public static bool IsAllowedInUnquotedString(char character)
```

#### Parameters

`character` [char](https://learn.microsoft.com/dotnet/api/system.char)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_IsAllowedNumber_System_Char_"></a> IsAllowedNumber\(char\)

```csharp
public static bool IsAllowedNumber(char digit)
```

#### Parameters

`digit` [char](https://learn.microsoft.com/dotnet/api/system.char)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_IsQuotedStringStart_System_Char_"></a> IsQuotedStringStart\(char\)

```csharp
public static bool IsQuotedStringStart(char character)
```

#### Parameters

`character` [char](https://learn.microsoft.com/dotnet/api/system.char)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_PeekAt_System_Int32_"></a> PeekAt\(int\)

```csharp
public char PeekAt(int offset)
```

#### Parameters

`offset` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [char](https://learn.microsoft.com/dotnet/api/system.char)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_ReadBoolean"></a> ReadBoolean\(\)

```csharp
public bool ReadBoolean()
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_ReadDouble"></a> ReadDouble\(\)

```csharp
public double ReadDouble()
```

#### Returns

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_ReadFloat"></a> ReadFloat\(\)

```csharp
public float ReadFloat()
```

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_ReadInt"></a> ReadInt\(\)

```csharp
public int ReadInt()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_ReadLong"></a> ReadLong\(\)

```csharp
public long ReadLong()
```

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_ReadNext"></a> ReadNext\(\)

```csharp
public char ReadNext()
```

#### Returns

 [char](https://learn.microsoft.com/dotnet/api/system.char)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_ReadQuotedString"></a> ReadQuotedString\(\)

```csharp
public string ReadQuotedString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_ReadString"></a> ReadString\(\)

```csharp
public string ReadString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_ReadStringUntil_System_Char_"></a> ReadStringUntil\(char\)

```csharp
public string ReadStringUntil(char terminator)
```

#### Parameters

`terminator` [char](https://learn.microsoft.com/dotnet/api/system.char)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_ReadUnquotedString"></a> ReadUnquotedString\(\)

```csharp
public string ReadUnquotedString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_Skip"></a> Skip\(\)

```csharp
public void Skip()
```

### <a id="Void_Minecraft_Commands_Brigadier_StringReader_SkipWhitespace"></a> SkipWhitespace\(\)

```csharp
public void SkipWhitespace()
```

