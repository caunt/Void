# <a id="Void_Minecraft_Nbt_ByteTag"></a> Class ByteTag

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

A tag that contains a single 8-bit integer value.

```csharp
[Serializable]
public class ByteTag : NumericTag<byte>, IEquatable<Tag>, ICloneable, IEquatable<NumericTag<byte>>, IComparable<NumericTag<byte>>, IComparable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Tag](Void.Minecraft.Nbt.Tag.md) ← 
[NumericTag<byte\>](Void.Minecraft.Nbt.NumericTag\-1.md) ← 
[ByteTag](Void.Minecraft.Nbt.ByteTag.md)

#### Implements

[IEquatable<Tag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICloneable](https://learn.microsoft.com/dotnet/api/system.icloneable), 
[IEquatable<NumericTag<byte\>\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IComparable<NumericTag<byte\>\>](https://learn.microsoft.com/dotnet/api/system.icomparable\-1), 
[IComparable](https://learn.microsoft.com/dotnet/api/system.icomparable)

#### Inherited Members

[NumericTag<byte\>.Value](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_Value), 
[NumericTag<byte\>.Equals\(NumericTag<byte\>?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_Equals\_Void\_Minecraft\_Nbt\_NumericTag\_\_0\_\_), 
[NumericTag<byte\>.Equals\(object?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_Equals\_System\_Object\_), 
[NumericTag<byte\>.GetHashCode\(\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_GetHashCode), 
[NumericTag<byte\>.CompareTo\(NumericTag<byte\>?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_CompareTo\_Void\_Minecraft\_Nbt\_NumericTag\_\_0\_\_), 
[NumericTag<byte\>.CompareTo\(object?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_CompareTo\_System\_Object\_), 
[Tag.NoName](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_NoName), 
[Tag.Type](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Type), 
[Tag.Parent](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Parent), 
[Tag.Name](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Name), 
[Tag.PrettyPrinted\(StringBuilder, int, string\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_PrettyPrinted\_System\_Text\_StringBuilder\_System\_Int32\_System\_String\_), 
[Tag.PrettyName](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_PrettyName), 
[Tag.WriteJson\(Utf8JsonWriter, bool\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_WriteJson\_System\_Text\_Json\_Utf8JsonWriter\_System\_Boolean\_), 
[Tag.WriteJson\(Stream, JsonWriterOptions?\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_WriteJson\_System\_IO\_Stream\_System\_Nullable\_System\_Text\_Json\_JsonWriterOptions\_\_), 
[Tag.WriteJsonAsync\(Stream, JsonWriterOptions?\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_WriteJsonAsync\_System\_IO\_Stream\_System\_Nullable\_System\_Text\_Json\_JsonWriterOptions\_\_), 
[Tag.ToJson\(JsonWriterOptions?\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_ToJson\_System\_Nullable\_System\_Text\_Json\_JsonWriterOptions\_\_), 
[Tag.ToJsonString\(bool, string\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_ToJsonString\_System\_Boolean\_System\_String\_), 
[Tag.Equals\(Tag?\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Equals\_Void\_Minecraft\_Nbt\_Tag\_), 
[Tag.Equals\(object?\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Equals\_System\_Object\_), 
[Tag.GetHashCode\(\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_GetHashCode), 
[Tag.Clone\(\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Clone), 
[Tag.Stringify\(bool\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Stringify\_System\_Boolean\_), 
[Tag.StringifyName](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_StringifyName), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Remarks

While this class uses the CLS compliant <xref href="System.Byte" data-throw-if-not-resolved="false"></xref> (0..255), the NBT specification uses a signed value with a range of -128..127. It is
recommended to use the <xref href="Void.Minecraft.Nbt.ByteTag.SignedValue" data-throw-if-not-resolved="false"></xref> property if your language supports a signed 8-bit value, otherwise simply ensure the bits are
equivalent.

## Constructors

### <a id="Void_Minecraft_Nbt_ByteTag__ctor_System_String_System_Byte_"></a> ByteTag\(string?, byte\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> class with the specified <code class="paramref">value</code>.

```csharp
public ByteTag(string? name, byte value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`value` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

The value to assign to this tag.

### <a id="Void_Minecraft_Nbt_ByteTag__ctor_System_String_System_Int32_"></a> ByteTag\(string?, int\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> class with the specified <code class="paramref">value</code>.

```csharp
public ByteTag(string? name, int value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The value to assign to this tag.

#### Remarks

The use of <xref href="System.Int32" data-throw-if-not-resolved="false"></xref> is for convenience only.

### <a id="Void_Minecraft_Nbt_ByteTag__ctor_System_String_System_Boolean_"></a> ByteTag\(string?, bool\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> class with the specified <code class="paramref">value</code>.

```csharp
public ByteTag(string? name, bool value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`value` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

The value to assign to this tag.

### <a id="Void_Minecraft_Nbt_ByteTag__ctor_System_String_System_SByte_"></a> ByteTag\(string?, sbyte\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> class with the specified <code class="paramref">value</code>.

```csharp
public ByteTag(string? name, sbyte value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`value` [sbyte](https://learn.microsoft.com/dotnet/api/system.sbyte)

The value to assign to this tag.

## Properties

### <a id="Void_Minecraft_Nbt_ByteTag_Bool"></a> Bool

Gets or sets the value of this tag as a boolean value.

```csharp
public bool Bool { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Nbt_ByteTag_IsBool"></a> IsBool

Gets a flag indicating if this <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> was assigned a <xref href="System.Boolean" data-throw-if-not-resolved="false"></xref> value.

```csharp
public bool IsBool { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Nbt_ByteTag_SignedValue"></a> SignedValue

Gets or sets the value of this tag as an unsigned value.

```csharp
public sbyte SignedValue { get; set; }
```

#### Property Value

 [sbyte](https://learn.microsoft.com/dotnet/api/system.sbyte)

#### Remarks

This is only a reinterpretation of the bytes, no actual conversion is performed.

### <a id="Void_Minecraft_Nbt_ByteTag_Value"></a> Value

```csharp
public byte Value { get; set; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

## Methods

### <a id="Void_Minecraft_Nbt_ByteTag_Stringify_System_Boolean_"></a> Stringify\(bool\)

Gets the <i>string</i> representation of this NBT tag (SNBT).

```csharp
public override string Stringify(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if the name of the tag should be written.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

This NBT tag in SNBT format.

#### See Also

[https://minecraft.fandom.com/wiki/NBT\_format\#SNBT\_format](https://minecraft.fandom.com/wiki/NBT\_format\#SNBT\_format)

### <a id="Void_Minecraft_Nbt_ByteTag_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

### <a id="Void_Minecraft_Nbt_ByteTag_WriteJson_System_Text_Json_Utf8JsonWriter_System_Boolean_"></a> WriteJson\(Utf8JsonWriter, bool\)

Uses the provided <code class="paramref">writer</code> to write the NBT tag in JSON format.

```csharp
protected override void WriteJson(Utf8JsonWriter writer, bool named = true)
```

#### Parameters

`writer` [Utf8JsonWriter](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonwriter)

A JSON writer instance.

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this object's name should be written as a property name, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when it
is a child of <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>, in which case it should be written as a JSON array element.

## Operators

### <a id="Void_Minecraft_Nbt_ByteTag_op_Implicit_Void_Minecraft_Nbt_ByteTag__System_Byte"></a> implicit operator byte\(ByteTag\)

Implicit conversion of this tag to a <xref href="System.Byte" data-throw-if-not-resolved="false"></xref>.

```csharp
public static implicit operator byte(ByteTag tag)
```

#### Parameters

`tag` [ByteTag](Void.Minecraft.Nbt.ByteTag.md)

The tag to convert.

#### Returns

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

The tag represented as a <xref href="System.Byte" data-throw-if-not-resolved="false"></xref>.

### <a id="Void_Minecraft_Nbt_ByteTag_op_Implicit_Void_Minecraft_Nbt_ByteTag__System_Boolean"></a> implicit operator bool\(ByteTag\)

Implicit conversion of this tag to a <xref href="System.Boolean" data-throw-if-not-resolved="false"></xref>.

```csharp
public static implicit operator bool(ByteTag tag)
```

#### Parameters

`tag` [ByteTag](Void.Minecraft.Nbt.ByteTag.md)

The tag to convert.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

The tag represented as a <xref href="System.Byte" data-throw-if-not-resolved="false"></xref>.

### <a id="Void_Minecraft_Nbt_ByteTag_op_Implicit_Void_Minecraft_Nbt_ByteTag__System_SByte"></a> implicit operator sbyte\(ByteTag\)

Implicit conversion of this tag to a <xref href="System.SByte" data-throw-if-not-resolved="false"></xref>.

```csharp
public static implicit operator sbyte(ByteTag tag)
```

#### Parameters

`tag` [ByteTag](Void.Minecraft.Nbt.ByteTag.md)

The tag to convert.

#### Returns

 [sbyte](https://learn.microsoft.com/dotnet/api/system.sbyte)

The tag represented as a <xref href="System.SByte" data-throw-if-not-resolved="false"></xref>.

