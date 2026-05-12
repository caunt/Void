# <a id="Void_Minecraft_Nbt_LongArrayTag"></a> Class LongArrayTag

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

A tag whose value is a contiguous sequence of 64-bit integers.

```csharp
public class LongArrayTag : ArrayTag<long>, IEquatable<Tag>, ICloneable, IReadOnlyList<long>, IReadOnlyCollection<long>, IEnumerable<long>, IEnumerable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Tag](Void.Minecraft.Nbt.Tag.md) ← 
[ArrayTag<long\>](Void.Minecraft.Nbt.ArrayTag\-1.md) ← 
[LongArrayTag](Void.Minecraft.Nbt.LongArrayTag.md)

#### Implements

[IEquatable<Tag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICloneable](https://learn.microsoft.com/dotnet/api/system.icloneable), 
[IReadOnlyList<long\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist\-1), 
[IReadOnlyCollection<long\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlycollection\-1), 
[IEnumerable<long\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1), 
[IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.ienumerable)

#### Inherited Members

[ArrayTag<long\>.Span](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_Span), 
[ArrayTag<long\>.Memory](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_Memory), 
[ArrayTag<long\>.GetEnumerator\(\)](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_GetEnumerator), 
[ArrayTag<long\>.CopyTo\(long\[\], int\)](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_CopyTo\_\_0\_\_\_System\_Int32\_), 
[ArrayTag<long\>.Count](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_Count), 
[ArrayTag<long\>.IndexOf\(long\)](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_IndexOf\_\_0\_), 
[ArrayTag<long\>.Slice\(int, int\)](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_Slice\_System\_Int32\_System\_Int32\_), 
[ArrayTag<long\>.this\[int\]](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_Item\_System\_Int32\_), 
[ArrayTag<long\>.GetPinnableReference\(\)](Void.Minecraft.Nbt.ArrayTag\-1.md\#Void\_Minecraft\_Nbt\_ArrayTag\_1\_GetPinnableReference), 
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

[EnumerableExtensions.Synchronized<long\>\(IEnumerable<long\>, AsyncLock, CancellationToken\)](Void.Proxy.Api.Extensions.EnumerableExtensions.md\#Void\_Proxy\_Api\_Extensions\_EnumerableExtensions\_Synchronized\_\_1\_System\_Collections\_Generic\_IEnumerable\_\_\_0\_\_Nito\_AsyncEx\_AsyncLock\_System\_Threading\_CancellationToken\_), 
[EnumerableExtensions.SynchronizedAsync<long\>\(IEnumerable<long\>, AsyncLock, CancellationToken\)](Void.Proxy.Api.Extensions.EnumerableExtensions.md\#Void\_Proxy\_Api\_Extensions\_EnumerableExtensions\_SynchronizedAsync\_\_1\_System\_Collections\_Generic\_IEnumerable\_\_\_0\_\_Nito\_AsyncEx\_AsyncLock\_System\_Threading\_CancellationToken\_)

## Constructors

### <a id="Void_Minecraft_Nbt_LongArrayTag__ctor_System_String_System_Int32_"></a> LongArrayTag\(string?, int\)

Initializes a new instance of the <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public LongArrayTag(string? name, int capacity)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The capacity of the array.

### <a id="Void_Minecraft_Nbt_LongArrayTag__ctor_System_String_System_Int64___"></a> LongArrayTag\(string?, long\[\]\)

Initializes a new instance of the <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code>.

```csharp
public LongArrayTag(string? name, long[] values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`values` [long](https://learn.microsoft.com/dotnet/api/system.int64)\[\]

A collection of values to include in this tag.

### <a id="Void_Minecraft_Nbt_LongArrayTag__ctor_System_String_System_Collections_Generic_IEnumerable_System_Int64__"></a> LongArrayTag\(string?, IEnumerable<long\>\)

Initializes a new instance of the <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code>.

```csharp
public LongArrayTag(string? name, IEnumerable<long> values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`values` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[long](https://learn.microsoft.com/dotnet/api/system.int64)\>

A collection of values to include in this tag.

### <a id="Void_Minecraft_Nbt_LongArrayTag__ctor_System_String_System_ReadOnlySpan_System_Int64__"></a> LongArrayTag\(string?, ReadOnlySpan<long\>\)

Initializes a new instance of the <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public LongArrayTag(string? name, ReadOnlySpan<long> values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`values` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[long](https://learn.microsoft.com/dotnet/api/system.int64)\>

A collection of values to include in this tag.

## Methods

### <a id="Void_Minecraft_Nbt_LongArrayTag_Stringify_System_Boolean_"></a> Stringify\(bool\)

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

### <a id="Void_Minecraft_Nbt_LongArrayTag_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

### <a id="Void_Minecraft_Nbt_LongArrayTag_WriteJson_System_Text_Json_Utf8JsonWriter_System_Boolean_"></a> WriteJson\(Utf8JsonWriter, bool\)

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

