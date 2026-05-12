# <a id="Void_Minecraft_Nbt_ShortTag"></a> Class ShortTag

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

A tag that contains a single 16-bit integer value.

```csharp
public class ShortTag : NumericTag<short>, IEquatable<Tag>, ICloneable, IEquatable<NumericTag<short>>, IComparable<NumericTag<short>>, IComparable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Tag](Void.Minecraft.Nbt.Tag.md) ← 
[NumericTag<short\>](Void.Minecraft.Nbt.NumericTag\-1.md) ← 
[ShortTag](Void.Minecraft.Nbt.ShortTag.md)

#### Implements

[IEquatable<Tag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICloneable](https://learn.microsoft.com/dotnet/api/system.icloneable), 
[IEquatable<NumericTag<short\>\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IComparable<NumericTag<short\>\>](https://learn.microsoft.com/dotnet/api/system.icomparable\-1), 
[IComparable](https://learn.microsoft.com/dotnet/api/system.icomparable)

#### Inherited Members

[NumericTag<short\>.Value](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_Value), 
[NumericTag<short\>.Equals\(NumericTag<short\>?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_Equals\_Void\_Minecraft\_Nbt\_NumericTag\_\_0\_\_), 
[NumericTag<short\>.Equals\(object?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_Equals\_System\_Object\_), 
[NumericTag<short\>.GetHashCode\(\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_GetHashCode), 
[NumericTag<short\>.CompareTo\(NumericTag<short\>?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_CompareTo\_Void\_Minecraft\_Nbt\_NumericTag\_\_0\_\_), 
[NumericTag<short\>.CompareTo\(object?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_CompareTo\_System\_Object\_), 
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

## Constructors

### <a id="Void_Minecraft_Nbt_ShortTag__ctor_System_String_System_Int16_"></a> ShortTag\(string?, short\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> class with the specified <code class="paramref">value</code>.

```csharp
public ShortTag(string? name, short value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`value` [short](https://learn.microsoft.com/dotnet/api/system.int16)

The value to assign to this tag.

### <a id="Void_Minecraft_Nbt_ShortTag__ctor_System_String_System_Int32_"></a> ShortTag\(string?, int\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> class with the specified <code class="paramref">value</code>.

```csharp
public ShortTag(string? name, int value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The value to assign to this tag.

#### Remarks

The use of <xref href="System.Int32" data-throw-if-not-resolved="false"></xref> is for convenience only.

### <a id="Void_Minecraft_Nbt_ShortTag__ctor_System_String_System_UInt16_"></a> ShortTag\(string?, ushort\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> class with the specified <code class="paramref">value</code>.

```csharp
public ShortTag(string? name, ushort value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`value` [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

The value to assign to this tag.

## Properties

### <a id="Void_Minecraft_Nbt_ShortTag_UnsignedValue"></a> UnsignedValue

Gets or sets the value of this tag as an unsigned value.

```csharp
public ushort UnsignedValue { get; set; }
```

#### Property Value

 [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

#### Remarks

This is only a reinterpretation of the bytes, no actual conversion is performed.

## Methods

### <a id="Void_Minecraft_Nbt_ShortTag_Stringify_System_Boolean_"></a> Stringify\(bool\)

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

### <a id="Void_Minecraft_Nbt_ShortTag_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

### <a id="Void_Minecraft_Nbt_ShortTag_WriteJson_System_Text_Json_Utf8JsonWriter_System_Boolean_"></a> WriteJson\(Utf8JsonWriter, bool\)

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

### <a id="Void_Minecraft_Nbt_ShortTag_op_Implicit_Void_Minecraft_Nbt_ShortTag__System_Int16"></a> implicit operator short\(ShortTag\)

Implicit conversion of this tag to a <xref href="System.Int16" data-throw-if-not-resolved="false"></xref>.

```csharp
public static implicit operator short(ShortTag tag)
```

#### Parameters

`tag` [ShortTag](Void.Minecraft.Nbt.ShortTag.md)

The tag to convert.

#### Returns

 [short](https://learn.microsoft.com/dotnet/api/system.int16)

The tag represented as a <xref href="System.Int16" data-throw-if-not-resolved="false"></xref>.

### <a id="Void_Minecraft_Nbt_ShortTag_op_Implicit_Void_Minecraft_Nbt_ShortTag__System_UInt16"></a> implicit operator ushort\(ShortTag\)

Implicit conversion of this tag to a <xref href="System.UInt16" data-throw-if-not-resolved="false"></xref>.

```csharp
public static implicit operator ushort(ShortTag tag)
```

#### Parameters

`tag` [ShortTag](Void.Minecraft.Nbt.ShortTag.md)

The tag to convert.

#### Returns

 [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

The tag represented as a <xref href="System.UInt16" data-throw-if-not-resolved="false"></xref>.

