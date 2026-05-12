# <a id="Void_Minecraft_Nbt_TagReader"></a> Class TagReader

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Provides methods for reading NBT data from a stream.

```csharp
public class TagReader : TagIO, IDisposable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TagIO](Void.Minecraft.Nbt.TagIO.md) ← 
[TagReader](Void.Minecraft.Nbt.TagReader.md)

#### Derived

[NbtReader](Void.Minecraft.Nbt.NbtReader.md)

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

### <a id="Void_Minecraft_Nbt_TagReader__ctor_System_IO_Stream_Void_Minecraft_Nbt_FormatOptions_System_Boolean_"></a> TagReader\(Stream, FormatOptions, bool\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.TagReader" data-throw-if-not-resolved="false"></xref> class from the given <code class="paramref">stream</code>.

```csharp
public TagReader(Stream stream, FormatOptions options, bool leaveOpen = false)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

A <xref href="System.IO.Stream" data-throw-if-not-resolved="false"></xref> instance that the reader will be reading from.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`leaveOpen` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

 to leave the <code class="paramref">stream</code> object open after disposing the <xref href="Void.Minecraft.Nbt.TagReader" data-throw-if-not-resolved="false"></xref>
object; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

## Methods

### <a id="Void_Minecraft_Nbt_TagReader_Dispose"></a> Dispose\(\)

Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

```csharp
public override void Dispose()
```

### <a id="Void_Minecraft_Nbt_TagReader_DisposeAsync"></a> DisposeAsync\(\)

Asynchronously releases the unmanaged resources used by the <xref href="Void.Minecraft.Nbt.TagReader" data-throw-if-not-resolved="false"></xref>.

```csharp
public override ValueTask DisposeAsync()
```

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Minecraft_Nbt_TagReader_OnTagEncountered_Void_Minecraft_Nbt_TagType_System_Boolean_"></a> OnTagEncountered\(TagType, bool\)

Invokes the <xref href="Void.Minecraft.Nbt.TagReader.TagEncountered" data-throw-if-not-resolved="false"></xref> event when the stream is positioned at the beginning of an unread tag.

```csharp
protected virtual Tag? OnTagEncountered(TagType type, bool named)
```

#### Parameters

`type` [TagType](Void.Minecraft.Nbt.TagType.md)

The type of tag next to be read from the stream.

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named.

#### Returns

 [Tag](Void.Minecraft.Nbt.Tag.md)?

When handled by an event subscriber, returns a parsed <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> instance, otherwise returns <param langword="null">.</param>

### <a id="Void_Minecraft_Nbt_TagReader_OnTagRead_Void_Minecraft_Nbt_Tag_"></a> OnTagRead\(Tag\)

Invokes the <xref href="Void.Minecraft.Nbt.TagReader.TagRead" data-throw-if-not-resolved="false"></xref> event when a tag has been fully deserialized from the <xref href="Void.Minecraft.Nbt.TagIO.BaseStream" data-throw-if-not-resolved="false"></xref>.

```csharp
protected virtual void OnTagRead(Tag tag)
```

#### Parameters

`tag` [Tag](Void.Minecraft.Nbt.Tag.md)

The deserialized <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> instance.

### <a id="Void_Minecraft_Nbt_TagReader_ReadByte_System_Boolean_"></a> ReadByte\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public ByteTag ReadByte(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [ByteTag](Void.Minecraft.Nbt.ByteTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.ByteTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadByteArray_System_Boolean_"></a> ReadByteArray\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public ByteArrayTag ReadByteArray(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [ByteArrayTag](Void.Minecraft.Nbt.ByteArrayTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.ByteArrayTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadCompound_System_Boolean_"></a> ReadCompound\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public CompoundTag ReadCompound(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadDouble_System_Boolean_"></a> ReadDouble\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.DoubleTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public DoubleTag ReadDouble(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [DoubleTag](Void.Minecraft.Nbt.DoubleTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.DoubleTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadFloat_System_Boolean_"></a> ReadFloat\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.FloatTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public FloatTag ReadFloat(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [FloatTag](Void.Minecraft.Nbt.FloatTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.FloatTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadInt_System_Boolean_"></a> ReadInt\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.IntTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public IntTag ReadInt(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [IntTag](Void.Minecraft.Nbt.IntTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.IntTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadIntArray_System_Boolean_"></a> ReadIntArray\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.IntArrayTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public IntArrayTag ReadIntArray(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [IntArrayTag](Void.Minecraft.Nbt.IntArrayTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.IntArrayTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadList_System_Boolean_"></a> ReadList\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public ListTag ReadList(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [ListTag](Void.Minecraft.Nbt.ListTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadLong_System_Boolean_"></a> ReadLong\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.LongTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public LongTag ReadLong(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [LongTag](Void.Minecraft.Nbt.LongTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.LongTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadLongArray_System_Boolean_"></a> ReadLongArray\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public LongArrayTag ReadLongArray(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [LongArrayTag](Void.Minecraft.Nbt.LongArrayTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.LongArrayTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadShort_System_Boolean_"></a> ReadShort\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public ShortTag ReadShort(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [ShortTag](Void.Minecraft.Nbt.ShortTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.ShortTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadString_System_Boolean_"></a> ReadString\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.StringTag" data-throw-if-not-resolved="false"></xref> from the stream.

```csharp
public StringTag ReadString(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [StringTag](Void.Minecraft.Nbt.StringTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.StringTag" data-throw-if-not-resolved="false"></xref> instance.

#### Remarks

It is assumed that the stream is positioned at the beginning of the tag payload.

### <a id="Void_Minecraft_Nbt_TagReader_ReadTag_System_Boolean_"></a> ReadTag\(bool\)

Reads a <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> from the current position in the stream.

```csharp
public Tag ReadTag(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [Tag](Void.Minecraft.Nbt.Tag.md)

The tag instance that was read from the stream.

### <a id="Void_Minecraft_Nbt_TagReader_ReadTag__1_System_Boolean_"></a> ReadTag<T\>\(bool\)

Convenience method to read a tag and cast it automatically.

```csharp
public T ReadTag<T>(bool named = true) where T : Tag
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 T

The tag instance that was read from the stream.

#### Type Parameters

`T` 

The tag type that is being read from the stream.

#### Remarks

This is typically only used when reading the top-level <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> of a document where the type is already known.

### <a id="Void_Minecraft_Nbt_TagReader_ReadTagAsync_System_Boolean_"></a> ReadTagAsync\(bool\)

Asynchronously reads a <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> from the current position in the stream.

```csharp
public Task<Tag> ReadTagAsync(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task\-1)<[Tag](Void.Minecraft.Nbt.Tag.md)\>

The tag instance that was read from the stream.

### <a id="Void_Minecraft_Nbt_TagReader_ReadTagAsync__1_System_Boolean_"></a> ReadTagAsync<T\>\(bool\)

Convenience method to asynchronously read a tag and cast it automatically.

```csharp
public Task<T> ReadTagAsync<T>(bool named = true) where T : Tag
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

#### Returns

 [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task\-1)<T\>

The tag instance that was read from the stream.

#### Type Parameters

`T` 

The tag type that is being read from the stream.

#### Remarks

This is typically only used when reading the top-level <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> of a document where the type is already known.

### <a id="Void_Minecraft_Nbt_TagReader_ReadToFixSizedBuffer_System_Span_System_Byte__"></a> ReadToFixSizedBuffer\(Span<byte\>\)

Reads bytes from the streams and stores them into the <code class="paramref">buffer</code>.
The number of read bytes is dictated by the size of the buffer.
This method ensures that all requested bytes are read.

```csharp
protected void ReadToFixSizedBuffer(Span<byte> buffer)
```

#### Parameters

`buffer` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The buffer where the read bytes are written to. the buffer size defines the number of bytes to read.

#### Remarks

Use this instead of <code>BaseStream.Read(buffer)</code>.
There was a breaking change in .NET 6 where the <xref href="System.IO.Stream.Read(System.Byte%5b%5d%2cSystem.Int32%2cSystem.Int32)" data-throw-if-not-resolved="false"></xref> can read less bytes than requested for certain streams.
Read more here: https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/partial-byte-reads-in-streams

#### Exceptions

 [EndOfStreamException](https://learn.microsoft.com/dotnet/api/system.io.endofstreamexception)

Throws if no more bytes could be read from the stream, but the buffer wasn't completely filled yet.

### <a id="Void_Minecraft_Nbt_TagReader_ReadToFixSizedBuffer_System_Byte___System_Int32_System_Int32_"></a> ReadToFixSizedBuffer\(byte\[\], int, int\)

Reads bytes from the streams and stores them into the <code class="paramref">buffer</code>.
This method ensures that all requested bytes are read.

```csharp
protected void ReadToFixSizedBuffer(byte[] buffer, int offset, int count)
```

#### Parameters

`buffer` [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

The buffer where the read bytes are written to. The data will be stored starting at <code class="paramref">offset</code> to <code class="paramref">offset</code> + <code class="paramref">count</code> - 1.

`offset` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The offset in <code class="paramref">buffer</code> where the read data is stored.

`count` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The number of bytes to read. Must be positive.

#### Remarks

Use this instead of <code>BaseStream.Read(buffer, offset, count)</code>.
There was a breaking change in .NET 6 where the <xref href="System.IO.Stream.Read(System.Byte%5b%5d%2cSystem.Int32%2cSystem.Int32)" data-throw-if-not-resolved="false"></xref> can read less bytes than requested for certain streams.
Read more here: https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/partial-byte-reads-in-streams

#### Exceptions

 [EndOfStreamException](https://learn.microsoft.com/dotnet/api/system.io.endofstreamexception)

Throws if no more bytes could be read from the stream, but the buffer wasn't completely filled yet.

### <a id="Void_Minecraft_Nbt_TagReader_ReadUTF8String"></a> ReadUTF8String\(\)

Reads a length-prefixed UTF-8 string from the stream.

```csharp
protected string? ReadUTF8String()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

The deserialized string instance.

### <a id="Void_Minecraft_Nbt_TagReader_TagEncountered"></a> TagEncountered

Occurs when a tag has been encountered in the stream, after reading the first byte to determine its <xref href="Void.Minecraft.Nbt.TagType" data-throw-if-not-resolved="false"></xref>.

```csharp
public event TagReaderCallback<TagHandledEventArgs>? TagEncountered
```

#### Event Type

 [TagReaderCallback](Void.Minecraft.Nbt.TagReaderCallback\-1.md)<[TagHandledEventArgs](Void.Minecraft.Nbt.TagHandledEventArgs.md)\>?

### <a id="Void_Minecraft_Nbt_TagReader_TagRead"></a> TagRead

Occurs when a tag has been fully deserialized from the stream.

```csharp
public event TagReaderCallback<TagEventArgs>? TagRead
```

#### Event Type

 [TagReaderCallback](Void.Minecraft.Nbt.TagReaderCallback\-1.md)<[TagEventArgs](Void.Minecraft.Nbt.TagEventArgs.md)\>?

