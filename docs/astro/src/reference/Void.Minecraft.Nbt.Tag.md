# <a id="Void_Minecraft_Nbt_Tag"></a> Class Tag

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Abstract base class that all NBT tags inherit from.

```csharp
public abstract class Tag : IEquatable<Tag>, ICloneable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Tag](Void.Minecraft.Nbt.Tag.md)

#### Derived

[ArrayTag<T\>](Void.Minecraft.Nbt.ArrayTag\-1.md), 
[BoolTag](Void.Minecraft.Nbt.BoolTag.md), 
[CompoundTag](Void.Minecraft.Nbt.CompoundTag.md), 
[EndTag](Void.Minecraft.Nbt.EndTag.md), 
[ListTag](Void.Minecraft.Nbt.ListTag.md), 
[NumericTag<T\>](Void.Minecraft.Nbt.NumericTag\-1.md), 
[StringTag](Void.Minecraft.Nbt.StringTag.md)

#### Implements

[IEquatable<Tag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICloneable](https://learn.microsoft.com/dotnet/api/system.icloneable)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Nbt_Tag__ctor_Void_Minecraft_Nbt_TagType_System_String_"></a> Tag\(TagType, string?\)

Initializes a new instance of the <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> class.

```csharp
protected Tag(TagType type, string? name)
```

#### Parameters

`type` [TagType](Void.Minecraft.Nbt.TagType.md)

A constant describing the NBT type for this tag.

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The name of the tag, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if tag has no name.

## Fields

### <a id="Void_Minecraft_Nbt_Tag_NoName"></a> NoName

Text applied in a pretty-print string when a tag has no defined <xref href="Void.Minecraft.Nbt.Tag.Name" data-throw-if-not-resolved="false"></xref> value.

```csharp
protected const string NoName = "None"
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Properties

### <a id="Void_Minecraft_Nbt_Tag_Name"></a> Name

Gets the name assigned to this <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref>.

```csharp
public string? Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Minecraft_Nbt_Tag_Parent"></a> Parent

Gets the parent <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> this object is a child of.

```csharp
[Obsolete("Parent property may be removed in a future version.")]
public Tag? Parent { get; }
```

#### Property Value

 [Tag](Void.Minecraft.Nbt.Tag.md)?

### <a id="Void_Minecraft_Nbt_Tag_PrettyName"></a> PrettyName

Gets the name of the object as a human-readable quoted string, or a default name to indicate it has no name when applicable.

```csharp
protected string PrettyName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Nbt_Tag_StringifyName"></a> StringifyName

Gets the name in a formatted properly for SNBT.

```csharp
protected string StringifyName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Nbt_Tag_Type"></a> Type

Gets a constant describing the NBT type this object represents.

```csharp
public TagType Type { get; }
```

#### Property Value

 [TagType](Void.Minecraft.Nbt.TagType.md)

## Methods

### <a id="Void_Minecraft_Nbt_Tag_Clone"></a> Clone\(\)

Creates a new object that is a copy of the current instance.

```csharp
public object Clone()
```

#### Returns

 [object](https://learn.microsoft.com/dotnet/api/system.object)

A new object that is a copy of this instance.

### <a id="Void_Minecraft_Nbt_Tag_Equals_Void_Minecraft_Nbt_Tag_"></a> Equals\(Tag?\)

Indicates whether the current object is equal to another object of the same type.

```csharp
public bool Equals(Tag? other)
```

#### Parameters

`other` [Tag](Void.Minecraft.Nbt.Tag.md)?

An object to compare with this object.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the current object is equal to the <code class="paramref">other</code> parameter; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Nbt_Tag_Equals_System_Object_"></a> Equals\(object?\)

Determines whether the specified object is equal to the current object.

```csharp
public override bool Equals(object? obj)
```

#### Parameters

`obj` [object](https://learn.microsoft.com/dotnet/api/system.object)?

The object to compare with the current object.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the specified object  is equal to the current object; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Nbt_Tag_GetHashCode"></a> GetHashCode\(\)

Serves as the default hash function.

```csharp
public override int GetHashCode()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A hash code for the current object.

### <a id="Void_Minecraft_Nbt_Tag_PrettyPrinted_System_Text_StringBuilder_System_Int32_System_String_"></a> PrettyPrinted\(StringBuilder, int, string\)

Writes this tag as a formatted string to the given <code class="paramref">buffer</code>.

```csharp
protected virtual void PrettyPrinted(StringBuilder buffer, int level, string indent)
```

#### Parameters

`buffer` [StringBuilder](https://learn.microsoft.com/dotnet/api/system.text.stringbuilder)

A <xref href="System.Text.StringBuilder" data-throw-if-not-resolved="false"></xref> instance to write to.

`level` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The current indent depth to write at.

`indent` [string](https://learn.microsoft.com/dotnet/api/system.string)

The string to use for indents.

### <a id="Void_Minecraft_Nbt_Tag_Stringify_System_Boolean_"></a> Stringify\(bool\)

Gets the <i>string</i> representation of this NBT tag (SNBT).

```csharp
public abstract string Stringify(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if the name of the tag should be written.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

This NBT tag in SNBT format.

#### See Also

[https://minecraft.fandom.com/wiki/NBT\_format\#SNBT\_format](https://minecraft.fandom.com/wiki/NBT\_format\#SNBT\_format)

### <a id="Void_Minecraft_Nbt_Tag_ToJson_System_Nullable_System_Text_Json_JsonWriterOptions__"></a> ToJson\(JsonWriterOptions?\)

Converts the NBT to an equivalent JSON representation, and returns it as a string.

```csharp
public string ToJson(JsonWriterOptions? options = null)
```

#### Parameters

`options` [JsonWriterOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonwriteroptions)?

Options that will be passed to the JSON writer.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

The JSON-encoded string representing describing the tag.

### <a id="Void_Minecraft_Nbt_Tag_ToJsonString_System_Boolean_System_String_"></a> ToJsonString\(bool, string\)

Gets a representation of this <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> as a JSON string.

```csharp
[Obsolete("Use WriteJson and ToJson instead.")]
public string ToJsonString(bool pretty = false, string indent = "    ")
```

#### Parameters

`pretty` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if formatting should be applied to make the string human-readable.

`indent` [string](https://learn.microsoft.com/dotnet/api/system.string)

Ignored

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A JSON string describing this object.

### <a id="Void_Minecraft_Nbt_Tag_WriteJson_System_Text_Json_Utf8JsonWriter_System_Boolean_"></a> WriteJson\(Utf8JsonWriter, bool\)

Uses the provided <code class="paramref">writer</code> to write the NBT tag in JSON format.

```csharp
protected abstract void WriteJson(Utf8JsonWriter writer, bool named = true)
```

#### Parameters

`writer` [Utf8JsonWriter](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonwriter)

A JSON writer instance.

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this object's name should be written as a property name, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when it
is a child of <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>, in which case it should be written as a JSON array element.

### <a id="Void_Minecraft_Nbt_Tag_WriteJson_System_IO_Stream_System_Nullable_System_Text_Json_JsonWriterOptions__"></a> WriteJson\(Stream, JsonWriterOptions?\)

Writes the tag to the specified <code class="paramref">stream</code> in JSON format.

```csharp
public void WriteJson(Stream stream, JsonWriterOptions? options = null)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

The stream instance to write to.

`options` [JsonWriterOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonwriteroptions)?

Options that will be passed to the JSON writer.

#### Exceptions

 [IOException](https://learn.microsoft.com/dotnet/api/system.io.ioexception)

The stream is no opened for writing.

### <a id="Void_Minecraft_Nbt_Tag_WriteJsonAsync_System_IO_Stream_System_Nullable_System_Text_Json_JsonWriterOptions__"></a> WriteJsonAsync\(Stream, JsonWriterOptions?\)

Asynchronously writes the tag to the specified <code class="paramref">stream</code> in JSON format.

```csharp
public Task WriteJsonAsync(Stream stream, JsonWriterOptions? options = null)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

The stream instance to write to.

`options` [JsonWriterOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonwriteroptions)?

Options that will be passed to the JSON writer.

#### Returns

 [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)

#### Exceptions

 [IOException](https://learn.microsoft.com/dotnet/api/system.io.ioexception)

The stream is no opened for writing.

## Operators

### <a id="Void_Minecraft_Nbt_Tag_op_Equality_Void_Minecraft_Nbt_Tag_Void_Minecraft_Nbt_Tag_"></a> operator ==\(Tag?, Tag?\)

Tests for equality of this object with another <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> instance.

```csharp
public static bool operator ==(Tag? left, Tag? right)
```

#### Parameters

`left` [Tag](Void.Minecraft.Nbt.Tag.md)?

First value to compare.

`right` [Tag](Void.Minecraft.Nbt.Tag.md)?

Second value to compare.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Result of comparison.

### <a id="Void_Minecraft_Nbt_Tag_op_Inequality_Void_Minecraft_Nbt_Tag_Void_Minecraft_Nbt_Tag_"></a> operator \!=\(Tag?, Tag?\)

Tests for inequality of this object with another <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> instance.

```csharp
public static bool operator !=(Tag? left, Tag? right)
```

#### Parameters

`left` [Tag](Void.Minecraft.Nbt.Tag.md)?

First value to compare.

`right` [Tag](Void.Minecraft.Nbt.Tag.md)?

Second value to compare.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Result of comparison.

