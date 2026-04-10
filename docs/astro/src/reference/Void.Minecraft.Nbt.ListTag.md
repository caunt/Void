# <a id="Void_Minecraft_Nbt_ListTag"></a> Class ListTag

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Represents a collection of a tags.

```csharp
public class ListTag : Tag, IEquatable<Tag>, ICloneable, IList<Tag>, ICollection<Tag>, IEnumerable<Tag>, IEnumerable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Tag](Void.Minecraft.Nbt.Tag.md) ← 
[ListTag](Void.Minecraft.Nbt.ListTag.md)

#### Implements

[IEquatable<Tag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICloneable](https://learn.microsoft.com/dotnet/api/system.icloneable), 
[IList<Tag\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ilist\-1), 
[ICollection<Tag\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.icollection\-1), 
[IEnumerable<Tag\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1), 
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

[EnumerableExtensions.Synchronized<Tag\>\(IEnumerable<Tag\>, AsyncLock, CancellationToken\)](Void.Proxy.Api.Extensions.EnumerableExtensions.md\#Void\_Proxy\_Api\_Extensions\_EnumerableExtensions\_Synchronized\_\_1\_System\_Collections\_Generic\_IEnumerable\_\_\_0\_\_Nito\_AsyncEx\_AsyncLock\_System\_Threading\_CancellationToken\_), 
[EnumerableExtensions.SynchronizedAsync<Tag\>\(IEnumerable<Tag\>, AsyncLock, CancellationToken\)](Void.Proxy.Api.Extensions.EnumerableExtensions.md\#Void\_Proxy\_Api\_Extensions\_EnumerableExtensions\_SynchronizedAsync\_\_1\_System\_Collections\_Generic\_IEnumerable\_\_\_0\_\_Nito\_AsyncEx\_AsyncLock\_System\_Threading\_CancellationToken\_)

## Remarks

All child tags <b>must</b> be have the same <xref href="Void.Minecraft.Nbt.Tag.Type" data-throw-if-not-resolved="false"></xref> value, and their <xref href="Void.Minecraft.Nbt.Tag.Name" data-throw-if-not-resolved="false"></xref> value will be omitted during serialization.

## Constructors

### <a id="Void_Minecraft_Nbt_ListTag__ctor_System_String_Void_Minecraft_Nbt_TagType_"></a> ListTag\(string?, TagType\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> class.

```csharp
public ListTag(string? name, TagType childType)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`childType` [TagType](Void.Minecraft.Nbt.TagType.md)

A constant describing the NBT type for children in this tag.

### <a id="Void_Minecraft_Nbt_ListTag__ctor_System_String_Void_Minecraft_Nbt_TagType_System_Int32_"></a> ListTag\(string?, TagType, int\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> class.

```csharp
public ListTag(string? name, TagType childType, int capacity)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`childType` [TagType](Void.Minecraft.Nbt.TagType.md)

A constant describing the NBT type for children in this tag.

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The initial capacity of the list.

### <a id="Void_Minecraft_Nbt_ListTag__ctor_System_String_Void_Minecraft_Nbt_TagType_System_Collections_Generic_IEnumerable_Void_Minecraft_Nbt_Tag__"></a> ListTag\(string?, TagType, IEnumerable<Tag\>\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">children</code>.

```csharp
public ListTag(string? name, TagType childType, IEnumerable<Tag> children)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`childType` [TagType](Void.Minecraft.Nbt.TagType.md)

A constant describing the NBT type for children in this tag.

`children` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Tag](Void.Minecraft.Nbt.Tag.md)\>

A collection of values to include in this tag.

## Properties

### <a id="Void_Minecraft_Nbt_ListTag_ChildType"></a> ChildType

Gets the NBT type of this tag's children.

```csharp
public TagType ChildType { get; }
```

#### Property Value

 [TagType](Void.Minecraft.Nbt.TagType.md)

### <a id="Void_Minecraft_Nbt_ListTag_Count"></a> Count

Gets the number of elements contained in the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public int Count { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Nbt_ListTag_Item_System_Int32_"></a> this\[int\]

Gets or sets the element at the specified index.

```csharp
public Tag this[int index] { get; set; }
```

#### Property Value

 [Tag](Void.Minecraft.Nbt.Tag.md)

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

<code class="paramref">index</code> is not a valid index in the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref>.

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The property is set and the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref> is read-only.

## Methods

### <a id="Void_Minecraft_Nbt_ListTag_Add_Void_Minecraft_Nbt_Tag_"></a> Add\(Tag\)

Adds an item to the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public void Add(Tag item)
```

#### Parameters

`item` [Tag](Void.Minecraft.Nbt.Tag.md)

The object to add to the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>.

#### Exceptions

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> is read-only.

### <a id="Void_Minecraft_Nbt_ListTag_AddRange_System_Collections_Generic_IEnumerable_Void_Minecraft_Nbt_Tag__"></a> AddRange\(IEnumerable<Tag\>\)

```csharp
public void AddRange(IEnumerable<Tag> items)
```

#### Parameters

`items` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Tag](Void.Minecraft.Nbt.Tag.md)\>

### <a id="Void_Minecraft_Nbt_ListTag_Clear"></a> Clear\(\)

Removes all items from the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public void Clear()
```

#### Exceptions

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> is read-only.

### <a id="Void_Minecraft_Nbt_ListTag_Contains_Void_Minecraft_Nbt_Tag_"></a> Contains\(Tag\)

Determines whether the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> contains a specific value.

```csharp
public bool Contains(Tag item)
```

#### Parameters

`item` [Tag](Void.Minecraft.Nbt.Tag.md)

The object to locate in the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">item</code> is found in the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Nbt_ListTag_CopyTo_Void_Minecraft_Nbt_Tag___System_Int32_"></a> CopyTo\(Tag\[\], int\)

Copies the elements of the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> to an <xref href="System.Array" data-throw-if-not-resolved="false"></xref>, starting at a particular <xref href="System.Array" data-throw-if-not-resolved="false"></xref> index.

```csharp
public void CopyTo(Tag[] array, int arrayIndex)
```

#### Parameters

`array` [Tag](Void.Minecraft.Nbt.Tag.md)\[\]

The one-dimensional <xref href="System.Array" data-throw-if-not-resolved="false"></xref> that is the destination of the elements copied from <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>. The <xref href="System.Array" data-throw-if-not-resolved="false"></xref> must have zero-based indexing.

`arrayIndex` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The zero-based index in <code class="paramref">array</code> at which copying begins.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

<code class="paramref">array</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

<code class="paramref">arrayIndex</code> is less than 0.

 [ArgumentException](https://learn.microsoft.com/dotnet/api/system.argumentexception)

The number of elements in the source <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> is greater than the available space from <code class="paramref">arrayIndex</code> to the end of the destination <code class="paramref">array</code>.

### <a id="Void_Minecraft_Nbt_ListTag_GetEnumerator"></a> GetEnumerator\(\)

Returns an enumerator that iterates through the collection.

```csharp
public IEnumerator<Tag> GetEnumerator()
```

#### Returns

 [IEnumerator](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerator\-1)<[Tag](Void.Minecraft.Nbt.Tag.md)\>

An enumerator that can be used to iterate through the collection.

### <a id="Void_Minecraft_Nbt_ListTag_IndexOf_Void_Minecraft_Nbt_Tag_"></a> IndexOf\(Tag\)

Determines the index of a specific item in the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public int IndexOf(Tag item)
```

#### Parameters

`item` [Tag](Void.Minecraft.Nbt.Tag.md)

The object to locate in the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

The index of <code class="paramref">item</code> if found in the list; otherwise, -1.

### <a id="Void_Minecraft_Nbt_ListTag_Insert_System_Int32_Void_Minecraft_Nbt_Tag_"></a> Insert\(int, Tag\)

Inserts an item to the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref> at the specified index.

```csharp
public void Insert(int index, Tag item)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The zero-based index at which <code class="paramref">item</code> should be inserted.

`item` [Tag](Void.Minecraft.Nbt.Tag.md)

The object to insert into the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref>.

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

<code class="paramref">index</code> is not a valid index in the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref>.

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref> is read-only.

### <a id="Void_Minecraft_Nbt_ListTag_PrettyPrinted_System_Text_StringBuilder_System_Int32_System_String_"></a> PrettyPrinted\(StringBuilder, int, string\)

Writes this tag as a formatted string to the given <code class="paramref">buffer</code>.

```csharp
protected override void PrettyPrinted(StringBuilder buffer, int level, string indent)
```

#### Parameters

`buffer` [StringBuilder](https://learn.microsoft.com/dotnet/api/system.text.stringbuilder)

A <xref href="System.Text.StringBuilder" data-throw-if-not-resolved="false"></xref> instance to write to.

`level` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The current indent depth to write at.

`indent` [string](https://learn.microsoft.com/dotnet/api/system.string)

The string to use for indents.

### <a id="Void_Minecraft_Nbt_ListTag_PrettyPrinted_System_String_"></a> PrettyPrinted\(string\)

Retrieves a "pretty-printed" multiline string representing the complete tree structure of the tag.

```csharp
public string PrettyPrinted(string indent = "    ")
```

#### Parameters

`indent` [string](https://learn.microsoft.com/dotnet/api/system.string)

The prefix that will be applied to each indent-level of nested nodes in the tree structure.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

The pretty-printed string.

### <a id="Void_Minecraft_Nbt_ListTag_Remove_Void_Minecraft_Nbt_Tag_"></a> Remove\(Tag\)

Removes the first occurrence of a specific object from the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public bool Remove(Tag item)
```

#### Parameters

`item` [Tag](Void.Minecraft.Nbt.Tag.md)

The object to remove from the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">item</code> was successfully removed from the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>. This method also returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> if <code class="paramref">item</code> is not found in the original <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>.

#### Exceptions

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> is read-only.

### <a id="Void_Minecraft_Nbt_ListTag_RemoveAt_System_Int32_"></a> RemoveAt\(int\)

Removes the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref> item at the specified index.

```csharp
public void RemoveAt(int index)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The zero-based index of the item to remove.

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

<code class="paramref">index</code> is not a valid index in the <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref>.

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The <xref href="System.Collections.Generic.IList%601" data-throw-if-not-resolved="false"></xref> is read-only.

### <a id="Void_Minecraft_Nbt_ListTag_Stringify_System_Boolean_"></a> Stringify\(bool\)

Gets the <i>string</i> representation of this NBT tag (SNBT).

```csharp
public override string Stringify(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

This NBT tag in SNBT format.

#### See Also

[https://minecraft.fandom.com/wiki/NBT\_format\#SNBT\_format](https://minecraft.fandom.com/wiki/NBT\_format\#SNBT\_format)

### <a id="Void_Minecraft_Nbt_ListTag_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

### <a id="Void_Minecraft_Nbt_ListTag_WriteJson_System_Text_Json_Utf8JsonWriter_System_Boolean_"></a> WriteJson\(Utf8JsonWriter, bool\)

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

