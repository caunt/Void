# <a id="Void_Minecraft_Buffers_BufferSpan"></a> Struct BufferSpan

Namespace: [Void.Minecraft.Buffers](Void.Minecraft.Buffers.md)  
Assembly: Void.Minecraft.dll  

Manages a span of bytes, allowing access and manipulation of its position within the buffer.

```csharp
public ref struct BufferSpan : IMinecraftBuffer<BufferSpan>, IReadMinecraftBuffer, IWriteMinecraftBuffer, ICommonMinecraftBuffer, IDisposable
```

#### Implements

[IMinecraftBuffer<BufferSpan\>](Void.Minecraft.Buffers.IMinecraftBuffer\-1.md), 
[IReadMinecraftBuffer](Void.Minecraft.Buffers.IReadMinecraftBuffer.md), 
[IWriteMinecraftBuffer](Void.Minecraft.Buffers.IWriteMinecraftBuffer.md), 
[ICommonMinecraftBuffer](Void.Minecraft.Buffers.ICommonMinecraftBuffer.md), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

#### Extension Methods

[ReadMinecraftBufferExtensions.Dump<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_Dump\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.Read<BufferSpan\>\(ref BufferSpan, int\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_Read\_\_1\_\_\_0\_\_System\_Int32\_), 
[ReadMinecraftBufferExtensions.ReadBoolean<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadBoolean\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadByteArray<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadByteArray\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadComponent<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadComponent\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadDouble<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadDouble\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadFloat<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadFloat\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadInt<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadInt\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadIntArray<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadIntArray\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadLong<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadLong\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadProperty<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadProperty\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadPropertyArray<BufferSpan\>\(ref BufferSpan, int\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadPropertyArray\_\_1\_\_\_0\_\_System\_Int32\_), 
[ReadMinecraftBufferExtensions.ReadShort<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadShort\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadString<BufferSpan\>\(ref BufferSpan, int\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadString\_\_1\_\_\_0\_\_System\_Int32\_), 
[ReadMinecraftBufferExtensions.ReadTag<BufferSpan\>\(ref BufferSpan, bool\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadTag\_\_1\_\_\_0\_\_System\_Boolean\_), 
[ReadMinecraftBufferExtensions.ReadToEnd<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadToEnd\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadUnsignedByte<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadUnsignedByte\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadUnsignedShort<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadUnsignedShort\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadUuid<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadUuid\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadUuidAsIntArray<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadUuidAsIntArray\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadVarInt<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadVarInt\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadVarIntArray<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadVarIntArray\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadVarLong<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadVarLong\_\_1\_\_\_0\_\_), 
[ReadMinecraftBufferExtensions.ReadVarShort<BufferSpan\>\(ref BufferSpan\)](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_ReadMinecraftBufferExtensions\_ReadVarShort\_\_1\_\_\_0\_\_), 
[WriteMinecraftBufferExtensions.Write<BufferSpan\>\(ref BufferSpan, scoped ReadOnlySpan<byte\>\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_Write\_\_1\_\_\_0\_\_System\_ReadOnlySpan\_System\_Byte\_\_), 
[WriteMinecraftBufferExtensions.Write<BufferSpan\>\(ref BufferSpan, Stream\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_Write\_\_1\_\_\_0\_\_System\_IO\_Stream\_), 
[WriteMinecraftBufferExtensions.WriteBoolean<BufferSpan\>\(ref BufferSpan, bool\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteBoolean\_\_1\_\_\_0\_\_System\_Boolean\_), 
[WriteMinecraftBufferExtensions.WriteByteArray<BufferSpan\>\(ref BufferSpan, scoped ReadOnlySpan<byte\>\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteByteArray\_\_1\_\_\_0\_\_System\_ReadOnlySpan\_System\_Byte\_\_), 
[WriteMinecraftBufferExtensions.WriteComponent<BufferSpan\>\(ref BufferSpan, Component\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteComponent\_\_1\_\_\_0\_\_Void\_Minecraft\_Components\_Text\_Component\_), 
[WriteMinecraftBufferExtensions.WriteDouble<BufferSpan\>\(ref BufferSpan, double\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteDouble\_\_1\_\_\_0\_\_System\_Double\_), 
[WriteMinecraftBufferExtensions.WriteFloat<BufferSpan\>\(ref BufferSpan, float\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteFloat\_\_1\_\_\_0\_\_System\_Single\_), 
[WriteMinecraftBufferExtensions.WriteInt<BufferSpan\>\(ref BufferSpan, int\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteInt\_\_1\_\_\_0\_\_System\_Int32\_), 
[WriteMinecraftBufferExtensions.WriteIntArray<BufferSpan\>\(ref BufferSpan, scoped ReadOnlySpan<int\>\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteIntArray\_\_1\_\_\_0\_\_System\_ReadOnlySpan\_System\_Int32\_\_), 
[WriteMinecraftBufferExtensions.WriteLong<BufferSpan\>\(ref BufferSpan, long\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteLong\_\_1\_\_\_0\_\_System\_Int64\_), 
[WriteMinecraftBufferExtensions.WriteProperty<BufferSpan\>\(ref BufferSpan, Property\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteProperty\_\_1\_\_\_0\_\_Void\_Minecraft\_Profiles\_Property\_), 
[WriteMinecraftBufferExtensions.WritePropertyArray<BufferSpan\>\(ref BufferSpan, Property\[\]?\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WritePropertyArray\_\_1\_\_\_0\_\_Void\_Minecraft\_Profiles\_Property\_\_\_), 
[WriteMinecraftBufferExtensions.WriteShort<BufferSpan\>\(ref BufferSpan, short\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteShort\_\_1\_\_\_0\_\_System\_Int16\_), 
[WriteMinecraftBufferExtensions.WriteString<BufferSpan\>\(ref BufferSpan, ReadOnlySpan<char\>\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteString\_\_1\_\_\_0\_\_System\_ReadOnlySpan\_System\_Char\_\_), 
[WriteMinecraftBufferExtensions.WriteTag<BufferSpan\>\(ref BufferSpan, NbtTag, bool\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteTag\_\_1\_\_\_0\_\_Void\_Minecraft\_Nbt\_NbtTag\_System\_Boolean\_), 
[WriteMinecraftBufferExtensions.WriteUnsignedByte<BufferSpan\>\(ref BufferSpan, byte\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteUnsignedByte\_\_1\_\_\_0\_\_System\_Byte\_), 
[WriteMinecraftBufferExtensions.WriteUnsignedShort<BufferSpan\>\(ref BufferSpan, ushort\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteUnsignedShort\_\_1\_\_\_0\_\_System\_UInt16\_), 
[WriteMinecraftBufferExtensions.WriteUuid<BufferSpan\>\(ref BufferSpan, Uuid\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteUuid\_\_1\_\_\_0\_\_Void\_Minecraft\_Profiles\_Uuid\_), 
[WriteMinecraftBufferExtensions.WriteUuidAsIntArray<BufferSpan\>\(ref BufferSpan, Uuid\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteUuidAsIntArray\_\_1\_\_\_0\_\_Void\_Minecraft\_Profiles\_Uuid\_), 
[WriteMinecraftBufferExtensions.WriteVarInt<BufferSpan\>\(ref BufferSpan, int\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteVarInt\_\_1\_\_\_0\_\_System\_Int32\_), 
[WriteMinecraftBufferExtensions.WriteVarIntArray<BufferSpan\>\(ref BufferSpan, scoped ReadOnlySpan<int\>\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteVarIntArray\_\_1\_\_\_0\_\_System\_ReadOnlySpan\_System\_Int32\_\_), 
[WriteMinecraftBufferExtensions.WriteVarLong<BufferSpan\>\(ref BufferSpan, long\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteVarLong\_\_1\_\_\_0\_\_System\_Int64\_), 
[WriteMinecraftBufferExtensions.WriteVarShort<BufferSpan\>\(ref BufferSpan, int\)](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_WriteMinecraftBufferExtensions\_WriteVarShort\_\_1\_\_\_0\_\_System\_Int32\_)

## Constructors

### <a id="Void_Minecraft_Buffers_BufferSpan__ctor_System_Span_System_Byte__"></a> BufferSpan\(Span<byte\>\)

Initializes a new instance of the BufferSpan class with a byte span.

```csharp
public BufferSpan(Span<byte> source)
```

#### Parameters

`source` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The byte span provides the data to be managed within the BufferSpan instance.

## Properties

### <a id="Void_Minecraft_Buffers_BufferSpan_Length"></a> Length

Returns the length of the source as an integer. It is a read-only property.

```csharp
public readonly int Length { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Buffers_BufferSpan_Position"></a> Position

Gets or sets the position within a buffer. The position must be between 0 and the length of the source,
otherwise an exception is thrown.

```csharp
public int Position { readonly get; set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Buffers_BufferSpan_Remaining"></a> Remaining

Calculates the number of remaining elements in a source collection. It subtracts the current position from the
total length.

```csharp
public readonly int Remaining { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Void_Minecraft_Buffers_BufferSpan_Access_System_Int32_"></a> Access\(int\)

Returns a writable view of <code class="paramref">length</code> bytes starting at the current <xref href="Void.Minecraft.Buffers.BufferSpan.Position" data-throw-if-not-resolved="false"></xref>.

```csharp
public readonly Span<byte> Access(int length)
```

#### Parameters

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The number of bytes to expose from the current position.

#### Returns

 [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A writable <xref href="System.Span%601" data-throw-if-not-resolved="false"></xref> over the requested region. When <code class="paramref">length</code> is <code>0</code>, an empty span at the current <xref href="Void.Minecraft.Buffers.BufferSpan.Position" data-throw-if-not-resolved="false"></xref> is returned.

#### Examples

<pre><code class="lang-csharp">var buffer = new BufferSpan(stackalloc byte[8]);
var payload = buffer.Access(4);
payload[0] = 0x01;
buffer.Seek(4);</code></pre>

#### Remarks

<p>This method intentionally does not advance <xref href="Void.Minecraft.Buffers.BufferSpan.Position" data-throw-if-not-resolved="false"></xref>; callers that consume or fill the returned span must move the cursor explicitly, for example with <xref href="Void.Minecraft.Buffers.BufferSpan.Seek(System.Int32%2cSystem.IO.SeekOrigin)" data-throw-if-not-resolved="false"></xref>.</p>
<p>The returned span aliases the underlying buffer, so writing through it mutates the same storage owned by this <xref href="Void.Minecraft.Buffers.BufferSpan" data-throw-if-not-resolved="false"></xref> instance.</p>

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

Thrown when <code class="paramref">length</code> is negative.

 [EndOfBufferException](Void.Minecraft.Buffers.Exceptions.EndOfBufferException.md)

Thrown when the requested range extends past the end of the underlying span.

#### See Also

[BufferSpan](Void.Minecraft.Buffers.BufferSpan.md).[Access](Void.Minecraft.Buffers.BufferSpan.md\#Void\_Minecraft\_Buffers\_BufferSpan\_Access\_System\_Int32\_System\_Int32\_)\([int](https://learn.microsoft.com/dotnet/api/system.int32), [int](https://learn.microsoft.com/dotnet/api/system.int32)\), 
[BufferSpan](Void.Minecraft.Buffers.BufferSpan.md).[Seek](Void.Minecraft.Buffers.BufferSpan.md\#Void\_Minecraft\_Buffers\_BufferSpan\_Seek\_System\_Int32\_System\_IO\_SeekOrigin\_)\([int](https://learn.microsoft.com/dotnet/api/system.int32), [SeekOrigin](https://learn.microsoft.com/dotnet/api/system.io.seekorigin)\)

### <a id="Void_Minecraft_Buffers_BufferSpan_Access_System_Int32_System_Int32_"></a> Access\(int, int\)

Returns a writable view of <code class="paramref">length</code> bytes starting at an absolute <code class="paramref">position</code>.

```csharp
public readonly Span<byte> Access(int position, int length)
```

#### Parameters

`position` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The zero-based index in the underlying span where the returned region begins.

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The number of bytes in the returned region.

#### Returns

 [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A writable <xref href="System.Span%601" data-throw-if-not-resolved="false"></xref> over the specified region. When <code class="paramref">length</code> is <code>0</code>, an empty span at <code class="paramref">position</code> is returned.

#### Examples

<pre><code class="lang-csharp">var buffer = new BufferSpan(stackalloc byte[16]);
var header = buffer.Access(0, 2);
header[0] = 0xAA;
header[1] = 0xBB;</code></pre>

#### Remarks

<p>This overload performs bounds validation and then delegates to <xref href="System.Span%601.Slice(System.Int32%2cSystem.Int32)" data-throw-if-not-resolved="false"></xref> on the underlying storage.</p>
<p>It does not modify <xref href="Void.Minecraft.Buffers.BufferSpan.Position" data-throw-if-not-resolved="false"></xref>, which allows random access reads or writes without changing the current cursor.</p>

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

Thrown when <code class="paramref">position</code> or <code class="paramref">length</code> is negative.

 [EndOfBufferException](Void.Minecraft.Buffers.Exceptions.EndOfBufferException.md)

Thrown when <code class="paramref">position</code> + <code class="paramref">length</code> exceeds <xref href="Void.Minecraft.Buffers.BufferSpan.Length" data-throw-if-not-resolved="false"></xref>.

#### See Also

[BufferSpan](Void.Minecraft.Buffers.BufferSpan.md).[Access](Void.Minecraft.Buffers.BufferSpan.md\#Void\_Minecraft\_Buffers\_BufferSpan\_Access\_System\_Int32\_)\([int](https://learn.microsoft.com/dotnet/api/system.int32)\), 
[BufferSpan](Void.Minecraft.Buffers.BufferSpan.md).[Position](Void.Minecraft.Buffers.BufferSpan.md\#Void\_Minecraft\_Buffers\_BufferSpan\_Position)

### <a id="Void_Minecraft_Buffers_BufferSpan_Dispose"></a> Dispose\(\)

Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

```csharp
public readonly void Dispose()
```

### <a id="Void_Minecraft_Buffers_BufferSpan_Seek_System_Int32_System_IO_SeekOrigin_"></a> Seek\(int, SeekOrigin\)

Moves the current position within the underlying span relative to the specified origin.

```csharp
public void Seek(int offset, SeekOrigin origin = SeekOrigin.Current)
```

#### Parameters

`offset` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The byte offset applied from <code class="paramref">origin</code>.

`origin` [SeekOrigin](https://learn.microsoft.com/dotnet/api/system.io.seekorigin)

The reference point used to compute the new position.

#### Exceptions

 [ArgumentException](https://learn.microsoft.com/dotnet/api/system.argumentexception)

Thrown when <code class="paramref">origin</code> is not a supported <xref href="System.IO.SeekOrigin" data-throw-if-not-resolved="false"></xref> value.

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

Thrown when the resulting position would be negative.

 [EndOfBufferException](Void.Minecraft.Buffers.Exceptions.EndOfBufferException.md)

Thrown when the resulting position would move past the end of the span.

### <a id="Void_Minecraft_Buffers_BufferSpan_Slice_System_Int32_System_Int32_"></a> Slice\(int, int\)

Extracts a portion of a buffer starting from a specified position for a given length.

```csharp
public readonly BufferSpan Slice(int position, int length)
```

#### Parameters

`position` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Indicates the starting point from which the extraction begins.

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Specifies the number of elements to include in the extracted portion.

#### Returns

 [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

Returns a new buffer containing the specified slice of the original buffer.

