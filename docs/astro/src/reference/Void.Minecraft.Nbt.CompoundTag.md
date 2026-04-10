# <a id="Void_Minecraft_Nbt_CompoundTag"></a> Class CompoundTag

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Top-level tag that acts as a container for other <b>named</b> tags.

```csharp
public class CompoundTag : Tag, IEquatable<Tag>, ICloneable, IDictionary<string, Tag>, ICollection<KeyValuePair<string, Tag>>, IEnumerable<KeyValuePair<string, Tag>>, ICollection<Tag>, IEnumerable<Tag>, IEnumerable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Tag](Void.Minecraft.Nbt.Tag.md) ← 
[CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

#### Implements

[IEquatable<Tag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICloneable](https://learn.microsoft.com/dotnet/api/system.icloneable), 
[IDictionary<string, Tag\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.idictionary\-2), 
[ICollection<KeyValuePair<string, Tag\>\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.icollection\-1), 
[IEnumerable<KeyValuePair<string, Tag\>\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1), 
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

## Remarks

This along with the <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> class define the structure of the NBT format. Children are not order-dependent, nor is order guaranteed. The
closing <xref href="Void.Minecraft.Nbt.EndTag" data-throw-if-not-resolved="false"></xref> does not require to be explicitly added, it will be added automatically during serialization.

## Constructors

### <a id="Void_Minecraft_Nbt_CompoundTag__ctor_System_String_"></a> CompoundTag\(string?\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> class.

```csharp
public CompoundTag(string? name)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

### <a id="Void_Minecraft_Nbt_CompoundTag__ctor_System_String_System_Collections_Generic_IEnumerable_Void_Minecraft_Nbt_Tag__"></a> CompoundTag\(string?, IEnumerable<Tag\>\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> class.

```csharp
public CompoundTag(string? name, IEnumerable<Tag> values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

`values` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Tag](Void.Minecraft.Nbt.Tag.md)\>

A collection <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> objects that are children of this object.

## Properties

### <a id="Void_Minecraft_Nbt_CompoundTag_Count"></a> Count

Removes all items from the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public int Count { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Exceptions

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> is read-only.

### <a id="Void_Minecraft_Nbt_CompoundTag_Keys"></a> Keys

Gets an <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> containing the keys of the <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref>.

```csharp
public ICollection<string> Keys { get; }
```

#### Property Value

 [ICollection](https://learn.microsoft.com/dotnet/api/system.collections.generic.icollection\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Void_Minecraft_Nbt_CompoundTag_Values"></a> Values

Gets an <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> containing the values in the <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref>.

```csharp
public ICollection<Tag> Values { get; }
```

#### Property Value

 [ICollection](https://learn.microsoft.com/dotnet/api/system.collections.generic.icollection\-1)<[Tag](Void.Minecraft.Nbt.Tag.md)\>

### <a id="Void_Minecraft_Nbt_CompoundTag_Item_System_String_"></a> this\[string\]

Gets or sets the element with the specified key.

```csharp
public Tag this[string name] { get; set; }
```

#### Property Value

 [Tag](Void.Minecraft.Nbt.Tag.md)

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

<code class="paramref">key</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

 [KeyNotFoundException](https://learn.microsoft.com/dotnet/api/system.collections.generic.keynotfoundexception)

The property is retrieved and <code class="paramref">key</code> is not found.

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The property is set and the <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref> is read-only.

## Methods

### <a id="Void_Minecraft_Nbt_CompoundTag_Add_System_String_Void_Minecraft_Nbt_Tag_"></a> Add\(string, Tag\)

Adds an element with the provided key and value to the <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref>.

```csharp
public void Add(string key, Tag value)
```

#### Parameters

`key` [string](https://learn.microsoft.com/dotnet/api/system.string)

The object to use as the key of the element to add.

`value` [Tag](Void.Minecraft.Nbt.Tag.md)

The object to use as the value of the element to add.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

<code class="paramref">key</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

 [ArgumentException](https://learn.microsoft.com/dotnet/api/system.argumentexception)

An element with the same key already exists in the <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref>.

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref> is read-only.

### <a id="Void_Minecraft_Nbt_CompoundTag_Add_Void_Minecraft_Nbt_Tag_"></a> Add\(Tag\)

Adds an item to the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public void Add(Tag value)
```

#### Parameters

`value` [Tag](Void.Minecraft.Nbt.Tag.md)

#### Exceptions

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> is read-only.

### <a id="Void_Minecraft_Nbt_CompoundTag_Clear"></a> Clear\(\)

Removes all items from the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public void Clear()
```

#### Exceptions

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> is read-only.

### <a id="Void_Minecraft_Nbt_CompoundTag_Contains_Void_Minecraft_Nbt_Tag_"></a> Contains\(Tag\)

Determines whether the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref> contains a specific value.

```csharp
public bool Contains(Tag tag)
```

#### Parameters

`tag` [Tag](Void.Minecraft.Nbt.Tag.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">item</code> is found in the <xref href="System.Collections.Generic.ICollection%601" data-throw-if-not-resolved="false"></xref>; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Nbt_CompoundTag_ContainsKey_System_String_"></a> ContainsKey\(string\)

Determines whether the <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref> contains an element with the specified key.

```csharp
public bool ContainsKey(string key)
```

#### Parameters

`key` [string](https://learn.microsoft.com/dotnet/api/system.string)

The key to locate in the <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref> contains an element with the key; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

<code class="paramref">key</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

### <a id="Void_Minecraft_Nbt_CompoundTag_CopyTo_Void_Minecraft_Nbt_Tag___System_Int32_"></a> CopyTo\(Tag\[\], int\)

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

### <a id="Void_Minecraft_Nbt_CompoundTag_Find__1_System_String_System_Boolean_"></a> Find<TTag\>\(string, bool\)

Searches the children of this tag, returning the first child with the specified <code class="paramref">name</code>.

```csharp
public TTag? Find<TTag>(string name, bool recursive = false) where TTag : Tag
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

The name of the tag to search for.

`recursive` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> to recursively search children, otherwise <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> to only search direct descendants.

#### Returns

 TTag?

The first tag found with <code class="paramref">name</code>, otherwise <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if none was found.

#### Type Parameters

`TTag` 

### <a id="Void_Minecraft_Nbt_CompoundTag_Get__1_System_String_"></a> Get<TTag\>\(string\)

```csharp
public TTag Get<TTag>(string name) where TTag : Tag
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 TTag

#### Type Parameters

`TTag` 

### <a id="Void_Minecraft_Nbt_CompoundTag_GetEnumerator"></a> GetEnumerator\(\)

Returns an enumerator that iterates through the collection.

```csharp
public IEnumerator<Tag> GetEnumerator()
```

#### Returns

 [IEnumerator](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerator\-1)<[Tag](Void.Minecraft.Nbt.Tag.md)\>

An enumerator that can be used to iterate through the collection.

### <a id="Void_Minecraft_Nbt_CompoundTag_PrettyPrinted_System_String_"></a> PrettyPrinted\(string?\)

Retrieves a "pretty-printed" multiline string representing the complete tree structure of the tag.

```csharp
public string PrettyPrinted(string? indent = "    ")
```

#### Parameters

`indent` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The prefix that will be applied to each indent-level of nested nodes in the tree structure.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

The pretty-printed string.

### <a id="Void_Minecraft_Nbt_CompoundTag_PrettyPrinted_System_Text_StringBuilder_System_Int32_System_String_"></a> PrettyPrinted\(StringBuilder, int, string\)

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

### <a id="Void_Minecraft_Nbt_CompoundTag_Remove_System_String_"></a> Remove\(string\)

Removes the element with the specified key from the <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref>.

```csharp
public bool Remove(string key)
```

#### Parameters

`key` [string](https://learn.microsoft.com/dotnet/api/system.string)

The key of the element to remove.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the element is successfully removed; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.  This method also returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> if <code class="paramref">key</code> was not found in the original <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref>.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

<code class="paramref">key</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

 [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception)

The <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref> is read-only.

### <a id="Void_Minecraft_Nbt_CompoundTag_Remove_Void_Minecraft_Nbt_Tag_"></a> Remove\(Tag\)

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

### <a id="Void_Minecraft_Nbt_CompoundTag_Stringify_System_Boolean_"></a> Stringify\(bool\)

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

### <a id="Void_Minecraft_Nbt_CompoundTag_Stringify_System_Boolean_System_Boolean_"></a> Stringify\(bool, bool\)

Gets the <i>string</i> representation of this NBT tag (SNBT).

```csharp
public string Stringify(bool topLevel, bool named)
```

#### Parameters

`topLevel` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this is the top-level tag that should be wrapped in braces.

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if the name of the NBT should be written.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

This NBT tag in SNBT format.

#### See Also

[https://minecraft.fandom.com/wiki/NBT\_format\#SNBT\_format](https://minecraft.fandom.com/wiki/NBT\_format\#SNBT\_format)

### <a id="Void_Minecraft_Nbt_CompoundTag_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

### <a id="Void_Minecraft_Nbt_CompoundTag_TryGetValue_System_String_Void_Minecraft_Nbt_Tag__"></a> TryGetValue\(string, out Tag\)

Gets the value associated with the specified key.

```csharp
public bool TryGetValue(string key, out Tag value)
```

#### Parameters

`key` [string](https://learn.microsoft.com/dotnet/api/system.string)

The key whose value to get.

`value` [Tag](Void.Minecraft.Nbt.Tag.md)

When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <code class="paramref">value</code> parameter. This parameter is passed uninitialized.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the object that implements <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref> contains an element with the specified key; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

<code class="paramref">key</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

### <a id="Void_Minecraft_Nbt_CompoundTag_TryGetValue__1_System_String___0__"></a> TryGetValue<TTag\>\(string, out TTag\)

Gets the value associated with the specified key.

```csharp
public bool TryGetValue<TTag>(string key, out TTag value) where TTag : Tag
```

#### Parameters

`key` [string](https://learn.microsoft.com/dotnet/api/system.string)

The key whose value to get.

`value` TTag

When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <code class="paramref">value</code> parameter. This parameter is passed uninitialized.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the object that implements <xref href="System.Collections.Generic.IDictionary%602" data-throw-if-not-resolved="false"></xref> contains an element with the specified key; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

#### Type Parameters

`TTag` 

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

<code class="paramref">key</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

### <a id="Void_Minecraft_Nbt_CompoundTag_WriteJson_System_Text_Json_Utf8JsonWriter_System_Boolean_"></a> WriteJson\(Utf8JsonWriter, bool\)

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

