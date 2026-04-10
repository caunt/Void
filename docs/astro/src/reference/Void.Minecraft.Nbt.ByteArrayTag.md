# <a id="Void_Minecraft_Nbt_ByteArrayTag"></a> Class ByteArrayTag

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

A tag whose value is a contiguous sequence of 8-bit integers.

```csharp
public class ByteArrayTag : ArrayTag<byte>, IEquatable<Tag>, ICloneable, IReadOnlyList<byte>, IReadOnlyCollection<byte>, IEnumerable<byte>, IEnumerable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Tag](Void.Minecraft.Nbt.Tag.md) ← 
[ArrayTag<byte\>](Void.Minecraft.Nbt.ArrayTag\-1.md) ← 
[ByteArrayTag](Void.Minecraft.Nbt.ByteArrayTag.md)

#### Implements

[IEquatable<Tag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICloneable](https://learn.microsoft.com/dotnet/api/system.icloneable), 
[IReadOnlyList<byte\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist\-1), 
[IReadOnlyCollection<byte\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlycollection\-1), 
[IEnumerable<byte\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1), 
[IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.ienumerable)

#### Inherited Members

[ArrayTag<byte\>.Span](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_Span), 
[ArrayTag<byte\>.Memory](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_Memory), 
[ArrayTag<byte\>.GetEnumerator\(\)](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_GetEnumerator), 
[ArrayTag<byte\>.CopyTo\(byte\[\], int\)](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_CopyTo\_\_0\_\_\_System\_Int32\_), 
[ArrayTag<byte\>.Count](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_Count), 
[ArrayTag<byte\>.IndexOf\(byte\)](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_IndexOf\_\_0\_), 
[ArrayTag<byte\>.Slice\(int, int\)](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_Slice\_System\_Int32\_System\_Int32\_), 
[ArrayTag<byte\>.this\[int\]](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_Item\_System\_Int32\_), 
[ArrayTag<byte\>.GetPinnableReference\(\)](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_GetPinnableReference), 
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

#### Extension Methods

[EnumerableExtensions.Synchronized<byte\>\(IEnumerable<byte\>, AsyncLock, CancellationToken\)](Void.Proxy.Api.Extensions.EnumerableExtensions.md\#Void\_Proxy\_Api\_Extensions\_EnumerableExtensions\_Synchronized\_\_1\_System\_Collections\_Generic\_IEnumerable\_\_\_0\_\_Nito\_AsyncEx\_AsyncLock\_System\_Threading\_CancellationToken\_), 
[EnumerableExtensions.SynchronizedAsync<byte\>\(IEnumerable<byte\>, AsyncLock, CancellationToken\)](Void.Proxy.Api.Extensions.EnumerableExtensions.md\#Void\_Proxy\_Api\_Extensions\_EnumerableExtensions\_SynchronizedAsync\_\_1\_System\_Collections\_Generic\_IEnumerable\_\_\_0\_\_Nito\_AsyncEx\_AsyncLock\_System\_Threading\_CancellationToken\_)

## Remarks

While this class uses the CLS compliant <xref href="System.Byte" data-throw-if-not-resolved="false"></xref> (0..255), the NBT specification uses a signed value with a range of -128..127, so ensure
the bits are equivalent for your values.

## Constructors

### <a id="Void_Minecraft_Nbt_ByteArrayTag__ctor_System_String_System_Int32_"></a> ByteArrayTag\(string?, int\)

Initializes a new instance of the <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public ByteArrayTag(string? name, int capacity)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The capacity of the array.

### <a id="Void_Minecraft_Nbt_ByteArrayTag__ctor_System_String_System_Byte___"></a> ByteArrayTag\(string?, byte\[\]\)

Initializes a new instance of the <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public ByteArrayTag(string? name, byte[] values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`values` [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

A collection of values to include in this tag.

### <a id="Void_Minecraft_Nbt_ByteArrayTag__ctor_System_String_System_Collections_Generic_IEnumerable_System_Byte__"></a> ByteArrayTag\(string?, IEnumerable<byte\>\)

Initializes a new instance of the <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code>.

```csharp
public ByteArrayTag(string? name, IEnumerable<byte> values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`values` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A collection of values to include in this tag.

### <a id="Void_Minecraft_Nbt_ByteArrayTag__ctor_System_String_System_ReadOnlySpan_System_Byte__"></a> ByteArrayTag\(string?, ReadOnlySpan<byte\>\)

Initializes a new instance of the <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code>.

```csharp
public ByteArrayTag(string? name, ReadOnlySpan<byte> values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`values` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A collection of values to include in this tag.

## Methods

### <a id="Void_Minecraft_Nbt_ByteArrayTag_Stringify_System_Boolean_"></a> Stringify\(bool\)

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

### <a id="Void_Minecraft_Nbt_ByteArrayTag_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

### <a id="Void_Minecraft_Nbt_ByteArrayTag_WriteJson_System_Text_Json_Utf8JsonWriter_System_Boolean_"></a> WriteJson\(Utf8JsonWriter, bool\)

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

