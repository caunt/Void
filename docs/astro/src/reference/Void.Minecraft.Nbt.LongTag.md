# <a id="Void_Minecraft_Nbt_LongTag"></a> Class LongTag

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

A tag that contains a single 64-bit integer value.

```csharp
public class LongTag : NumericTag<long>, IEquatable<Tag>, ICloneable, IEquatable<NumericTag<long>>, IComparable<NumericTag<long>>, IComparable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Tag](Void.Minecraft.Nbt.Tag.md) ← 
[NumericTag<long\>](Void.Minecraft.Nbt.NumericTag\-1.md) ← 
[LongTag](Void.Minecraft.Nbt.LongTag.md)

#### Implements

[IEquatable<Tag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICloneable](https://learn.microsoft.com/dotnet/api/system.icloneable), 
[IEquatable<NumericTag<long\>\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IComparable<NumericTag<long\>\>](https://learn.microsoft.com/dotnet/api/system.icomparable\-1), 
[IComparable](https://learn.microsoft.com/dotnet/api/system.icomparable)

#### Inherited Members

[NumericTag<long\>.Value](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_Value), 
[NumericTag<long\>.Equals\(NumericTag<long\>?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_Equals\_Void\_Minecraft\_Nbt\_NumericTag\_\_0\_\_), 
[NumericTag<long\>.Equals\(object?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_Equals\_System\_Object\_), 
[NumericTag<long\>.GetHashCode\(\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_GetHashCode), 
[NumericTag<long\>.CompareTo\(NumericTag<long\>?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_CompareTo\_Void\_Minecraft\_Nbt\_NumericTag\_\_0\_\_), 
[NumericTag<long\>.CompareTo\(object?\)](Void.Minecraft.Nbt.NumericTag\-1.md\#Void\_Minecraft\_Nbt\_NumericTag\_1\_CompareTo\_System\_Object\_), 
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

### <a id="Void_Minecraft_Nbt_LongTag__ctor_System_String_System_Int64_"></a> LongTag\(string?, long\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.LongTag" data-throw-if-not-resolved="false"></xref> class with the specified <code class="paramref">value</code>.

```csharp
public LongTag(string? name, long value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The value to assign to this tag.

### <a id="Void_Minecraft_Nbt_LongTag__ctor_System_String_System_UInt64_"></a> LongTag\(string?, ulong\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.LongTag" data-throw-if-not-resolved="false"></xref> class with the specified <code class="paramref">value</code>.

```csharp
public LongTag(string? name, ulong value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`value` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The value to assign to this tag.

## Properties

### <a id="Void_Minecraft_Nbt_LongTag_UnsignedValue"></a> UnsignedValue

Gets or sets the value of this tag as an unsigned value.

```csharp
public ulong UnsignedValue { get; set; }
```

#### Property Value

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

#### Remarks

This is only a reinterpretation of the bytes, no actual conversion is performed.

## Methods

### <a id="Void_Minecraft_Nbt_LongTag_Stringify_System_Boolean_"></a> Stringify\(bool\)

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

### <a id="Void_Minecraft_Nbt_LongTag_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

### <a id="Void_Minecraft_Nbt_LongTag_WriteJson_System_Text_Json_Utf8JsonWriter_System_Boolean_"></a> WriteJson\(Utf8JsonWriter, bool\)

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

### <a id="Void_Minecraft_Nbt_LongTag_op_Implicit_Void_Minecraft_Nbt_LongTag__System_Int64"></a> implicit operator long\(LongTag\)

Implicit conversion of this tag to a <xref href="System.Int64" data-throw-if-not-resolved="false"></xref>.

```csharp
public static implicit operator long(LongTag tag)
```

#### Parameters

`tag` [LongTag](Void.Minecraft.Nbt.LongTag.md)

The tag to convert.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

The tag represented as a <xref href="System.Int64" data-throw-if-not-resolved="false"></xref>.

### <a id="Void_Minecraft_Nbt_LongTag_op_Implicit_Void_Minecraft_Nbt_LongTag__System_UInt64"></a> implicit operator ulong\(LongTag\)

Implicit conversion of this tag to a <xref href="System.UInt64" data-throw-if-not-resolved="false"></xref>.

```csharp
public static implicit operator ulong(LongTag tag)
```

#### Parameters

`tag` [LongTag](Void.Minecraft.Nbt.LongTag.md)

The tag to convert.

#### Returns

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The tag represented as a <xref href="System.UInt64" data-throw-if-not-resolved="false"></xref>.

