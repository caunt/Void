# <a id="Void_Minecraft_Nbt_TagWriter"></a> Class TagWriter

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Provides methods for writing NBT tags to a stream.

```csharp
public class TagWriter : TagIO, IDisposable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TagIO](Void.Minecraft.Nbt.TagIO.md) ← 
[TagWriter](Void.Minecraft.Nbt.TagWriter.md)

#### Derived

[BufferedTagWriter](Void.Minecraft.Nbt.BufferedTagWriter.md)

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

#### Inherited Members

[TagIO.BaseStream](Void.Minecraft.Nbt.TagIO.md\#Void\_Minecraft\_Nbt\_TagIO\_BaseStream), 
[TagIO.SwapEndian](Void.Minecraft.Nbt.TagIO.md\#Void\_Minecraft\_Nbt\_TagIO\_SwapEndian), 
[TagIO.UseVarInt](Void.Minecraft.Nbt.TagIO.md\#Void\_Minecraft\_Nbt\_TagIO\_UseVarInt), 
[TagIO.ZigZagEncoding](Void.Minecraft.Nbt.TagIO.md\#Void\_Minecraft\_Nbt\_TagIO\_ZigZagEncoding), 
[TagIO.FormatOptions](Void.Minecraft.Nbt.TagIO.md\#Void\_Minecraft\_Nbt\_TagIO\_FormatOptions), 
[TagIO.Dispose\(\)](Void.Minecraft.Nbt.TagIO.md\#Void\_Minecraft\_Nbt\_TagIO\_Dispose), 
[TagIO.DisposeAsync\(\)](Void.Minecraft.Nbt.TagIO.md\#Void\_Minecraft\_Nbt\_TagIO\_DisposeAsync), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Nbt_TagWriter__ctor_System_IO_Stream_Void_Minecraft_Nbt_FormatOptions_System_Boolean_"></a> TagWriter\(Stream, FormatOptions, bool\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.TagWriter" data-throw-if-not-resolved="false"></xref> class from the given <code class="paramref">stream</code>.

```csharp
public TagWriter(Stream stream, FormatOptions options, bool leaveOpen = false)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

A <xref href="System.IO.Stream" data-throw-if-not-resolved="false"></xref> instance that the writer will be writing to.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`leaveOpen` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

 to leave the <code class="paramref">stream</code> object open after disposing the <xref href="Void.Minecraft.Nbt.TagWriter" data-throw-if-not-resolved="false"></xref>
object; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

## Methods

### <a id="Void_Minecraft_Nbt_TagWriter_Dispose"></a> Dispose\(\)

Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

```csharp
public override void Dispose()
```

### <a id="Void_Minecraft_Nbt_TagWriter_DisposeAsync"></a> DisposeAsync\(\)

Asynchronously releases the unmanaged resources used by the <xref href="Void.Minecraft.Nbt.TagWriter" data-throw-if-not-resolved="false"></xref>.

```csharp
public override ValueTask DisposeAsync()
```

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Minecraft_Nbt_TagWriter_WriteBuilder_Void_Minecraft_Nbt_TagBuilder_"></a> WriteBuilder\(TagBuilder\)

Convenience method to build and write a <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance to the underlying stream.

```csharp
public virtual void WriteBuilder(TagBuilder builder)
```

#### Parameters

`builder` [TagBuilder](Void.Minecraft.Nbt.TagBuilder.md)

A <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteByte_Void_Minecraft_Nbt_ByteTag_"></a> WriteByte\(ByteTag\)

Writes a <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteByte(ByteTag tag)
```

#### Parameters

`tag` [ByteTag](Void.Minecraft.Nbt.ByteTag.md)

The <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteByteArray_Void_Minecraft_Nbt_ByteArrayTag_"></a> WriteByteArray\(ByteArrayTag\)

Writes a <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteByteArray(ByteArrayTag tag)
```

#### Parameters

`tag` [ByteArrayTag](Void.Minecraft.Nbt.ByteArrayTag.md)

The <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteCompound_Void_Minecraft_Nbt_CompoundTag_"></a> WriteCompound\(CompoundTag\)

Writes a <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteCompound(CompoundTag tag)
```

#### Parameters

`tag` [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

The <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteDouble_Void_Minecraft_Nbt_DoubleTag_"></a> WriteDouble\(DoubleTag\)

Writes a <xref href="Void.Minecraft.Nbt.DoubleTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteDouble(DoubleTag tag)
```

#### Parameters

`tag` [DoubleTag](Void.Minecraft.Nbt.DoubleTag.md)

The <xref href="Void.Minecraft.Nbt.DoubleTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteEndTag_Void_Minecraft_Nbt_EndTag_"></a> WriteEndTag\(EndTag?\)

```csharp
public virtual void WriteEndTag(EndTag? tag = null)
```

#### Parameters

`tag` [EndTag](Void.Minecraft.Nbt.EndTag.md)?

### <a id="Void_Minecraft_Nbt_TagWriter_WriteFloat_Void_Minecraft_Nbt_FloatTag_"></a> WriteFloat\(FloatTag\)

Writes a <xref href="Void.Minecraft.Nbt.FloatTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteFloat(FloatTag tag)
```

#### Parameters

`tag` [FloatTag](Void.Minecraft.Nbt.FloatTag.md)

The <xref href="Void.Minecraft.Nbt.FloatTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteInt_Void_Minecraft_Nbt_IntTag_"></a> WriteInt\(IntTag\)

Writes a <xref href="Void.Minecraft.Nbt.IntTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteInt(IntTag tag)
```

#### Parameters

`tag` [IntTag](Void.Minecraft.Nbt.IntTag.md)

The <xref href="Void.Minecraft.Nbt.IntTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteIntArray_Void_Minecraft_Nbt_IntArrayTag_"></a> WriteIntArray\(IntArrayTag\)

Writes a <xref href="Void.Minecraft.Nbt.IntArrayTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteIntArray(IntArrayTag tag)
```

#### Parameters

`tag` [IntArrayTag](Void.Minecraft.Nbt.IntArrayTag.md)

The <xref href="Void.Minecraft.Nbt.IntArrayTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteList_Void_Minecraft_Nbt_ListTag_"></a> WriteList\(ListTag\)

Writes a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteList(ListTag tag)
```

#### Parameters

`tag` [ListTag](Void.Minecraft.Nbt.ListTag.md)

The <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteLong_Void_Minecraft_Nbt_LongTag_"></a> WriteLong\(LongTag\)

Writes a <xref href="Void.Minecraft.Nbt.LongTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteLong(LongTag tag)
```

#### Parameters

`tag` [LongTag](Void.Minecraft.Nbt.LongTag.md)

The <xref href="Void.Minecraft.Nbt.LongTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteLongArray_Void_Minecraft_Nbt_LongArrayTag_"></a> WriteLongArray\(LongArrayTag\)

Writes a <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteLongArray(LongArrayTag tag)
```

#### Parameters

`tag` [LongArrayTag](Void.Minecraft.Nbt.LongArrayTag.md)

The <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteShort_Void_Minecraft_Nbt_ShortTag_"></a> WriteShort\(ShortTag\)

Writes a <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteShort(ShortTag tag)
```

#### Parameters

`tag` [ShortTag](Void.Minecraft.Nbt.ShortTag.md)

The <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteString_Void_Minecraft_Nbt_StringTag_"></a> WriteString\(StringTag\)

Writes a <xref href="Void.Minecraft.Nbt.StringTag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public virtual void WriteString(StringTag tag)
```

#### Parameters

`tag` [StringTag](Void.Minecraft.Nbt.StringTag.md)

The <xref href="Void.Minecraft.Nbt.StringTag" data-throw-if-not-resolved="false"></xref> instance to write.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteTag_Void_Minecraft_Nbt_Tag_"></a> WriteTag\(Tag\)

Writes the given <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public void WriteTag(Tag tag)
```

#### Parameters

`tag` [Tag](Void.Minecraft.Nbt.Tag.md)

The <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> instance to be written.

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

Thrown when the tag type is unrecognized.

### <a id="Void_Minecraft_Nbt_TagWriter_WriteTagAsync_Void_Minecraft_Nbt_Tag_"></a> WriteTagAsync\(Tag\)

Asynchronously writes the given <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> to the stream.

```csharp
public Task WriteTagAsync(Tag tag)
```

#### Parameters

`tag` [Tag](Void.Minecraft.Nbt.Tag.md)

The <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> instance to be written.

#### Returns

 [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

Thrown when the tag type is unrecognized.

