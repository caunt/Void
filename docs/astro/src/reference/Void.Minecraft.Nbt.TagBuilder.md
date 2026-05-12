# <a id="Void_Minecraft_Nbt_TagBuilder"></a> Class TagBuilder

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Provides a mechanism for easily building a tree of NBT objects by handling the intermediate step of creating tags, allowing the direct adding of their
equivalent values.

<p></p>

All methods return the <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance itself, allowing for easily chaining calls to build a document.

```csharp
public class TagBuilder
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Nbt_TagBuilder__ctor_System_String_"></a> TagBuilder\(string?\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> class, optionally with a <code class="paramref">name</code> to assign the top-level
<xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> of the final result.

```csharp
public TagBuilder(string? name = null)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

## Properties

### <a id="Void_Minecraft_Nbt_TagBuilder_Depth"></a> Depth

Gets the zero-based depth of the current node, indicating how deeply nested it is within other tags.

```csharp
public int Depth { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Remarks

The implicit top-level <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> is not factored into this value.

## Methods

### <a id="Void_Minecraft_Nbt_TagBuilder_AddBool_System_String_System_Boolean_"></a> AddBool\(string?, bool\)

Adds a new <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddBool(string? name, bool value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddBool_System_Boolean_"></a> AddBool\(bool\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddBool(bool value)
```

#### Parameters

`value` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByte_System_String_System_Byte_"></a> AddByte\(string?, byte\)

Adds a new <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddByte(string? name, byte value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByte_System_String_System_Int32_"></a> AddByte\(string?, int\)

Adds a new <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddByte(string? name, int value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByte_System_String_System_SByte_"></a> AddByte\(string?, sbyte\)

Adds a new <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddByte(string? name, sbyte value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [sbyte](https://learn.microsoft.com/dotnet/api/system.sbyte)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByte_System_Byte_"></a> AddByte\(byte\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddByte(byte value)
```

#### Parameters

`value` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByte_System_Int32_"></a> AddByte\(int\)

```csharp
public TagBuilder AddByte(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByte_System_SByte_"></a> AddByte\(sbyte\)

```csharp
public TagBuilder AddByte(sbyte value)
```

#### Parameters

`value` [sbyte](https://learn.microsoft.com/dotnet/api/system.sbyte)

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByteArray_System_String_System_Byte___"></a> AddByteArray\(string?, params byte\[\]\)

Adds a new <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddByteArray(string? name, params byte[] values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`values` [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByteArray_System_String_System_Collections_Generic_IEnumerable_System_Byte__"></a> AddByteArray\(string?, IEnumerable<byte\>\)

Adds a new <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddByteArray(string? name, IEnumerable<byte> values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`values` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByteArray_System_Byte___"></a> AddByteArray\(params byte\[\]\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.StringTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddByteArray(params byte[] values)
```

#### Parameters

`values` [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByteArray_System_Collections_Generic_IEnumerable_System_Byte__"></a> AddByteArray\(IEnumerable<byte\>\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.StringTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddByteArray(IEnumerable<byte> values)
```

#### Parameters

`values` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByteArray_System_String_System_SByte___"></a> AddByteArray\(string?, params sbyte\[\]\)

Adds a new <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddByteArray(string? name, params sbyte[] values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`values` [sbyte](https://learn.microsoft.com/dotnet/api/system.sbyte)\[\]

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByteArray_System_String_System_Collections_Generic_IEnumerable_System_SByte__"></a> AddByteArray\(string?, IEnumerable<sbyte\>\)

Adds a new <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddByteArray(string? name, IEnumerable<sbyte> values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`values` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[sbyte](https://learn.microsoft.com/dotnet/api/system.sbyte)\>

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByteArray_System_SByte___"></a> AddByteArray\(params sbyte\[\]\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.StringTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddByteArray(params sbyte[] values)
```

#### Parameters

`values` [sbyte](https://learn.microsoft.com/dotnet/api/system.sbyte)\[\]

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddByteArray_System_Collections_Generic_IEnumerable_System_SByte__"></a> AddByteArray\(IEnumerable<sbyte\>\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.StringTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddByteArray(IEnumerable<sbyte> values)
```

#### Parameters

`values` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[sbyte](https://learn.microsoft.com/dotnet/api/system.sbyte)\>

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddDouble_System_String_System_Double_"></a> AddDouble\(string?, double\)

Adds a new <xref href="Void.Minecraft.Nbt.DoubleTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddDouble(string? name, double value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [double](https://learn.microsoft.com/dotnet/api/system.double)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddDouble_System_Double_"></a> AddDouble\(double\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.DoubleTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddDouble(double value)
```

#### Parameters

`value` [double](https://learn.microsoft.com/dotnet/api/system.double)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddFloat_System_String_System_Single_"></a> AddFloat\(string?, float\)

Adds a new <xref href="Void.Minecraft.Nbt.FloatTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddFloat(string? name, float value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [float](https://learn.microsoft.com/dotnet/api/system.single)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddFloat_System_Single_"></a> AddFloat\(float\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.FloatTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddFloat(float value)
```

#### Parameters

`value` [float](https://learn.microsoft.com/dotnet/api/system.single)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddInt_System_String_System_Int32_"></a> AddInt\(string?, int\)

Adds a new <xref href="Void.Minecraft.Nbt.IntTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddInt(string? name, int value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddInt_System_String_System_UInt32_"></a> AddInt\(string?, uint\)

Adds a new <xref href="Void.Minecraft.Nbt.IntTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddInt(string? name, uint value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddInt_System_Int32_"></a> AddInt\(int\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.IntTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddInt(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddInt_System_UInt32_"></a> AddInt\(uint\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.IntTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddInt(uint value)
```

#### Parameters

`value` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddIntArray_System_String_System_Int32___"></a> AddIntArray\(string?, params int\[\]\)

Adds a new <xref href="Void.Minecraft.Nbt.IntArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddIntArray(string? name, params int[] values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`values` [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddIntArray_System_String_System_Collections_Generic_IEnumerable_System_Int32__"></a> AddIntArray\(string?, IEnumerable<int\>\)

Adds a new <xref href="Void.Minecraft.Nbt.IntArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddIntArray(string? name, IEnumerable<int> values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`values` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddIntArray_System_Int32___"></a> AddIntArray\(params int\[\]\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.IntArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddIntArray(params int[] values)
```

#### Parameters

`values` [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddIntArray_System_Collections_Generic_IEnumerable_System_Int32__"></a> AddIntArray\(IEnumerable<int\>\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.IntArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddIntArray(IEnumerable<int> values)
```

#### Parameters

`values` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddLong_System_String_System_Int64_"></a> AddLong\(string?, long\)

Adds a new <xref href="Void.Minecraft.Nbt.LongTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddLong(string? name, long value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddLong_System_String_System_UInt64_"></a> AddLong\(string?, ulong\)

Adds a new <xref href="Void.Minecraft.Nbt.LongTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddLong(string? name, ulong value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddLong_System_Int64_"></a> AddLong\(long\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.LongTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddLong(long value)
```

#### Parameters

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddLong_System_UInt64_"></a> AddLong\(ulong\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.LongTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddLong(ulong value)
```

#### Parameters

`value` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddLongArray_System_String_System_Int64___"></a> AddLongArray\(string?, params long\[\]\)

Adds a new <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddLongArray(string? name, params long[] values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`values` [long](https://learn.microsoft.com/dotnet/api/system.int64)\[\]

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddLongArray_System_String_System_Collections_Generic_IEnumerable_System_Int64__"></a> AddLongArray\(string?, IEnumerable<long\>\)

Adds a new <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddLongArray(string? name, IEnumerable<long> values)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`values` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[long](https://learn.microsoft.com/dotnet/api/system.int64)\>

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddLongArray_System_Int64___"></a> AddLongArray\(params long\[\]\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddLongArray(params long[] values)
```

#### Parameters

`values` [long](https://learn.microsoft.com/dotnet/api/system.int64)\[\]

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddLongArray_System_Collections_Generic_IEnumerable_System_Int64__"></a> AddLongArray\(IEnumerable<long\>\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">values</code> to the tree at the current depth.

```csharp
public TagBuilder AddLongArray(IEnumerable<long> values)
```

#### Parameters

`values` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[long](https://learn.microsoft.com/dotnet/api/system.int64)\>

The value(s) that will be included in the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddShort_System_String_System_Int16_"></a> AddShort\(string?, short\)

Adds a new <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddShort(string? name, short value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [short](https://learn.microsoft.com/dotnet/api/system.int16)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddShort_System_String_System_Int32_"></a> AddShort\(string?, int\)

Adds a new <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddShort(string? name, int value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddShort_System_String_System_UInt16_"></a> AddShort\(string?, ushort\)

Adds a new <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddShort(string? name, ushort value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddShort_System_Int16_"></a> AddShort\(short\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddShort(short value)
```

#### Parameters

`value` [short](https://learn.microsoft.com/dotnet/api/system.int16)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddShort_System_Int32_"></a> AddShort\(int\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddShort(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddShort_System_UInt16_"></a> AddShort\(ushort\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddShort(ushort value)
```

#### Parameters

`value` [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddString_System_String_System_String_"></a> AddString\(string?, string?\)

Adds a new <xref href="Void.Minecraft.Nbt.StringTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">name</code> and <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddString(string? name, string? value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the node to add.

`value` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

### <a id="Void_Minecraft_Nbt_TagBuilder_AddString_System_String_"></a> AddString\(string?\)

Adds a new unnamed <xref href="Void.Minecraft.Nbt.StringTag" data-throw-if-not-resolved="false"></xref> with the specified <code class="paramref">value</code> to the tree at the current depth.

```csharp
public TagBuilder AddString(string? value)
```

#### Parameters

`value` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The value of the tag.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

### <a id="Void_Minecraft_Nbt_TagBuilder_AddTag_Void_Minecraft_Nbt_Tag_"></a> AddTag\(Tag\)

Adds an existing <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> object to the tree at the current depth.

```csharp
public TagBuilder AddTag(Tag tag)
```

#### Parameters

`tag` [Tag](Void.Minecraft.Nbt.Tag.md)

The <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> instance to add.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

#### Exceptions

 [ArgumentException](https://learn.microsoft.com/dotnet/api/system.argumentexception)

Thrown if adding to a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> node, and the type does not match.

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

Thrown when <code class="paramref">tag</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

### <a id="Void_Minecraft_Nbt_TagBuilder_BeginCompound_System_String_"></a> BeginCompound\(string?\)

Opens a new <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> section, increasing the current depth level by one.

```csharp
public TagBuilder BeginCompound(string? name = null)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name to apply to the <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> to omit a name.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

#### See Also

[TagBuilder](Void.Minecraft.Nbt.TagBuilder.md).[EndCompound](Void.Minecraft.Nbt.TagBuilder.md\#Void\_Minecraft\_Nbt\_TagBuilder\_EndCompound)\(\)

### <a id="Void_Minecraft_Nbt_TagBuilder_BeginList_Void_Minecraft_Nbt_TagType_System_String_"></a> BeginList\(TagType, string?\)

Opens a new <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> section, increasing the current depth level by one.

```csharp
public TagBuilder BeginList(TagType childType, string? name = null)
```

#### Parameters

`childType` [TagType](Void.Minecraft.Nbt.TagType.md)

The <xref href="Void.Minecraft.Nbt.TagType" data-throw-if-not-resolved="false"></xref> of the child items this list will contain.

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name to apply to the <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> to omit a name.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

#### See Also

[TagBuilder](Void.Minecraft.Nbt.TagBuilder.md).[EndList](Void.Minecraft.Nbt.TagBuilder.md\#Void\_Minecraft\_Nbt\_TagBuilder\_EndList)\(\)

### <a id="Void_Minecraft_Nbt_TagBuilder_Create"></a> Create\(\)

Closes any open compound/list sections, and returns the result as a <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public CompoundTag Create()
```

#### Returns

 [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

A <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> representing the result of this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> tree.

#### Remarks

Invoking this method moves the current <xref href="Void.Minecraft.Nbt.TagBuilder.Depth" data-throw-if-not-resolved="false"></xref> back to the top-level.

### <a id="Void_Minecraft_Nbt_TagBuilder_End"></a> End\(\)

Closes the current <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> or <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> section and decreases the <xref href="Void.Minecraft.Nbt.TagBuilder.Depth" data-throw-if-not-resolved="false"></xref> by one.

```csharp
public TagBuilder End()
```

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

#### Remarks

This method does nothing if the current location is already at the top-level.

### <a id="Void_Minecraft_Nbt_TagBuilder_EndCompound"></a> EndCompound\(\)

Closes the current <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> section and decreases the <xref href="Void.Minecraft.Nbt.TagBuilder.Depth" data-throw-if-not-resolved="false"></xref> by one. Does nothing if the current node does not
represent a <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public TagBuilder EndCompound()
```

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

#### See Also

[TagBuilder](Void.Minecraft.Nbt.TagBuilder.md).[BeginCompound](Void.Minecraft.Nbt.TagBuilder.md\#Void\_Minecraft\_Nbt\_TagBuilder\_BeginCompound\_System\_String\_)\([string](https://learn.microsoft.com/dotnet/api/system.string)?\)

### <a id="Void_Minecraft_Nbt_TagBuilder_EndList"></a> EndList\(\)

Closes the current <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> section and decreases the <xref href="Void.Minecraft.Nbt.TagBuilder.Depth" data-throw-if-not-resolved="false"></xref> by one. Does nothing if the current node does not
represent a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public TagBuilder EndList()
```

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

Returns this <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance for chaining.

#### See Also

[TagBuilder](Void.Minecraft.Nbt.TagBuilder.md).[BeginList](Void.Minecraft.Nbt.TagBuilder.md\#Void\_Minecraft\_Nbt\_TagBuilder\_BeginList\_Void\_Minecraft\_Nbt\_TagType\_System\_String\_)\([TagType](Void.Minecraft.Nbt.TagType.md), [string](https://learn.microsoft.com/dotnet/api/system.string)?\)

### <a id="Void_Minecraft_Nbt_TagBuilder_NewCompound_System_String_"></a> NewCompound\(string?\)

Creates a new <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> and pushes it to the current scope level, returning a <xref href="Void.Minecraft.Nbt.TagBuilder.Context" data-throw-if-not-resolved="false"></xref> object that pulls the current
scope back out one level when disposed.

```csharp
public TagBuilder.Context NewCompound(string? name)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name to apply to the <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> to omit a name.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md).[Context](Void.Minecraft.Nbt.TagBuilder.Context.md)

A <xref href="Void.Minecraft.Nbt.TagBuilder.Context" data-throw-if-not-resolved="false"></xref> that will close the <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> when disposed.

#### Remarks

This is essentially no different than <xref href="Void.Minecraft.Nbt.TagBuilder.BeginCompound(System.String)" data-throw-if-not-resolved="false"></xref> and <xref href="Void.Minecraft.Nbt.TagBuilder.EndCompound" data-throw-if-not-resolved="false"></xref> but can use `using` blocks to distinguish scope.

### <a id="Void_Minecraft_Nbt_TagBuilder_NewList_Void_Minecraft_Nbt_TagType_System_String_"></a> NewList\(TagType, string?\)

Creates a new <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> and pushes it to the current scope level, returning a <xref href="Void.Minecraft.Nbt.TagBuilder.Context" data-throw-if-not-resolved="false"></xref> object that pulls the current
scope back out one level when disposed.

```csharp
public TagBuilder.Context NewList(TagType childType, string? name)
```

#### Parameters

`childType` [TagType](Void.Minecraft.Nbt.TagType.md)

The <xref href="Void.Minecraft.Nbt.TagType" data-throw-if-not-resolved="false"></xref> of the child items this list will contain.

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name to apply to the <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> to omit a name.

#### Returns

 [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md).[Context](Void.Minecraft.Nbt.TagBuilder.Context.md)

A <xref href="Void.Minecraft.Nbt.TagBuilder.Context" data-throw-if-not-resolved="false"></xref> that will close the <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> when disposed.

#### Remarks

This is essentially no different than <xref href="Void.Minecraft.Nbt.TagBuilder.BeginList(Void.Minecraft.Nbt.TagType%2cSystem.String)" data-throw-if-not-resolved="false"></xref> and <xref href="Void.Minecraft.Nbt.TagBuilder.EndList" data-throw-if-not-resolved="false"></xref> but can use `using` blocks to distinguish scope.

