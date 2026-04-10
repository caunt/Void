# <a id="Void_Minecraft_Nbt_ArrayTag_1"></a> Class ArrayTag<T\>

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Base class for NBT tags that contain a fixed-size array of numeric types.

```csharp
public abstract class ArrayTag<T> : Tag, IEquatable<Tag>, ICloneable, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable where T : unmanaged, INumber<T>
```

#### Type Parameters

`T` 

A value type that implements <xref href="System.Numerics.INumber%601" data-throw-if-not-resolved="false"></xref>.

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Tag](Void.Minecraft.Nbt.Tag.md) ← 
[ArrayTag<T\>](Void.Minecraft.Nbt.ArrayTag\-1.md)

#### Implements

[IEquatable<Tag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICloneable](https://learn.microsoft.com/dotnet/api/system.icloneable), 
[IReadOnlyList<T\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist\-1), 
[IReadOnlyCollection<T\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlycollection\-1), 
[IEnumerable<T\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1), 
[IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.ienumerable)

#### Inherited Members

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

[EnumerableExtensions.Synchronized<T\>\(IEnumerable<T\>, AsyncLock, CancellationToken\)](Void.Proxy.Api.Extensions.EnumerableExtensions.md\#Void\_Proxy\_Api\_Extensions\_EnumerableExtensions\_Synchronized\_\_1\_System\_Collections\_Generic\_IEnumerable\_\_\_0\_\_Nito\_AsyncEx\_AsyncLock\_System\_Threading\_CancellationToken\_), 
[EnumerableExtensions.SynchronizedAsync<T\>\(IEnumerable<T\>, AsyncLock, CancellationToken\)](Void.Proxy.Api.Extensions.EnumerableExtensions.md\#Void\_Proxy\_Api\_Extensions\_EnumerableExtensions\_SynchronizedAsync\_\_1\_System\_Collections\_Generic\_IEnumerable\_\_\_0\_\_Nito\_AsyncEx\_AsyncLock\_System\_Threading\_CancellationToken\_)

## Constructors

### <a id="Void_Minecraft_Nbt_ArrayTag_1__ctor_Void_Minecraft_Nbt_TagType_System_String__0___"></a> ArrayTag\(TagType, string?, T\[\]\)

```csharp
protected ArrayTag(TagType type, string? name, T[] value)
```

#### Parameters

`type` [TagType](Void.Minecraft.Nbt.TagType.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

`value` T\[\]

The value of the tag.

## Properties

### <a id="Void_Minecraft_Nbt_ArrayTag_1_Count"></a> Count

Gets the number of elements in the collection.

```csharp
public int Count { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Nbt_ArrayTag_1_Memory"></a> Memory

Gets a <xref href="System.Memory%601" data-throw-if-not-resolved="false"></xref> over the tag data.

```csharp
public Memory<T> Memory { get; }
```

#### Property Value

 [Memory](https://learn.microsoft.com/dotnet/api/system.memory\-1)<T\>

### <a id="Void_Minecraft_Nbt_ArrayTag_1_Span"></a> Span

Gets a <xref href="System.Span%601" data-throw-if-not-resolved="false"></xref> over the tag data.

```csharp
public Span<T> Span { get; }
```

#### Property Value

 [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<T\>

### <a id="Void_Minecraft_Nbt_ArrayTag_1_Item_System_Int32_"></a> this\[int\]

Gets or sets the element at the specified index.

```csharp
public T this[int index] { get; set; }
```

#### Property Value

 T

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

<code class="paramref">index</code> is not a valid index in the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref>.

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The property is set and the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref> is read-only.

## Methods

### <a id="Void_Minecraft_Nbt_ArrayTag_1_CopyTo__0___System_Int32_"></a> CopyTo\(T\[\], int\)

```csharp
public void CopyTo(T[] array, int arrayIndex)
```

#### Parameters

`array` T\[\]

`arrayIndex` [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Nbt_ArrayTag_1_GetEnumerator"></a> GetEnumerator\(\)

Returns an enumerator that iterates through the collection.

```csharp
public IEnumerator<T> GetEnumerator()
```

#### Returns

 [IEnumerator](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerator\-1)<T\>

An enumerator that can be used to iterate through the collection.

### <a id="Void_Minecraft_Nbt_ArrayTag_1_GetPinnableReference"></a> GetPinnableReference\(\)

Returns a reference to the underlying memory of this object that is be pinned using the <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/statements/fixed">fixed</a>
statement.

```csharp
public ref T GetPinnableReference()
```

#### Returns

 T

A reference to the first value in the underlying array.

### <a id="Void_Minecraft_Nbt_ArrayTag_1_IndexOf__0_"></a> IndexOf\(T\)

Determines the index of a specific item in the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public int IndexOf(T item)
```

#### Parameters

`item` T

The object to locate in the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

The index of <code class="paramref">item</code> if found in the list; otherwise, -1.

### <a id="Void_Minecraft_Nbt_ArrayTag_1_Slice_System_Int32_System_Int32_"></a> Slice\(int, int\)

Forms a slice out of the current span starting at a specified index for a specified length.

```csharp
public Span<T> Slice(int start, int length)
```

#### Parameters

`start` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The zero-based index at which to begin this slice.

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The desired length for the slice.

#### Returns

 [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<T\>

A span that consists of <code class="paramref">length</code> elements from the current span starting at <code class="paramref">start</code>.

#### Remarks

This method being defined provides Range indexers for the class.

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

<code class="paramref">start</code> or <code class="paramref">start</code> + <code class="paramref">length</code> is less than zero or greater than <xref href="System.Span%601.Length" data-throw-if-not-resolved="false"></xref>.

## Operators

### <a id="Void_Minecraft_Nbt_ArrayTag_1_op_Implicit_Void_Minecraft_Nbt_ArrayTag__0____0__"></a> implicit operator T\[\]\(ArrayTag<T\>\)

Implicit conversion of an <xref href="Void.Minecraft.Nbt.ArrayTag%601" data-throw-if-not-resolved="false"></xref> to an array of T.

```csharp
public static implicit operator T[](ArrayTag<T> tag)
```

#### Parameters

`tag` [ArrayTag](Void.Minecraft.Nbt.ArrayTag\-1.md)<T\>

The <xref href="Void.Minecraft.Nbt.ArrayTag%601" data-throw-if-not-resolved="false"></xref> to be converted.

#### Returns

 T\[\]

The value of <code class="paramref">tag</code> as an array of T.

### <a id="Void_Minecraft_Nbt_ArrayTag_1_op_Implicit_Void_Minecraft_Nbt_ArrayTag__0___System_Span__0_"></a> implicit operator Span<T\>\(ArrayTag<T\>\)

Implicit conversion of an <xref href="Void.Minecraft.Nbt.ArrayTag%601" data-throw-if-not-resolved="false"></xref> to a <xref href="System.Span%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public static implicit operator Span<T>(ArrayTag<T> tag)
```

#### Parameters

`tag` [ArrayTag](Void.Minecraft.Nbt.ArrayTag\-1.md)<T\>

The <xref href="Void.Minecraft.Nbt.ArrayTag%601" data-throw-if-not-resolved="false"></xref> to be converted.

#### Returns

 [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<T\>

The value of <code class="paramref">tag</code> as a <xref href="System.Span%601" data-throw-if-not-resolved="false"></xref>.

### <a id="Void_Minecraft_Nbt_ArrayTag_1_op_Implicit_Void_Minecraft_Nbt_ArrayTag__0___System_Memory__0_"></a> implicit operator Memory<T\>\(ArrayTag<T\>\)

Implicit conversion of an <xref href="Void.Minecraft.Nbt.ArrayTag%601" data-throw-if-not-resolved="false"></xref> to a <xref href="System.Memory%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public static implicit operator Memory<T>(ArrayTag<T> tag)
```

#### Parameters

`tag` [ArrayTag](Void.Minecraft.Nbt.ArrayTag\-1.md)<T\>

The <xref href="Void.Minecraft.Nbt.ArrayTag%601" data-throw-if-not-resolved="false"></xref> to be converted.

#### Returns

 [Memory](https://learn.microsoft.com/dotnet/api/system.memory\-1)<T\>

The value of <code class="paramref">tag</code> as a <xref href="System.Memory%601" data-throw-if-not-resolved="false"></xref>.

