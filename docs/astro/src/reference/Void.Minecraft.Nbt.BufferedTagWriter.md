# <a id="Void_Minecraft_Nbt_BufferedTagWriter"></a> Class BufferedTagWriter

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Provides a <xref href="Void.Minecraft.Nbt.TagWriter" data-throw-if-not-resolved="false"></xref> object that writes to an internal buffer instead of a <xref href="System.IO.Stream" data-throw-if-not-resolved="false"></xref> object, which then can be retrieved as
an array of bytes or written directly to a stream. This is especially convenient when creating packets to be sent over a network, where the size of
the packet must be pre-determined before sending.

```csharp
public class BufferedTagWriter : TagWriter, IDisposable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TagIO](Void.Minecraft.Nbt.TagIO.md) ← 
[TagWriter](Void.Minecraft.Nbt.TagWriter.md) ← 
[BufferedTagWriter](Void.Minecraft.Nbt.BufferedTagWriter.md)

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

#### Inherited Members

[TagWriter.WriteByte\(ByteTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteByte\_Void\_Minecraft\_Nbt\_ByteTag\_), 
[TagWriter.WriteShort\(ShortTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteShort\_Void\_Minecraft\_Nbt\_ShortTag\_), 
[TagWriter.WriteInt\(IntTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteInt\_Void\_Minecraft\_Nbt\_IntTag\_), 
[TagWriter.WriteLong\(LongTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteLong\_Void\_Minecraft\_Nbt\_LongTag\_), 
[TagWriter.WriteFloat\(FloatTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteFloat\_Void\_Minecraft\_Nbt\_FloatTag\_), 
[TagWriter.WriteDouble\(DoubleTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteDouble\_Void\_Minecraft\_Nbt\_DoubleTag\_), 
[TagWriter.WriteString\(StringTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteString\_Void\_Minecraft\_Nbt\_StringTag\_), 
[TagWriter.WriteByteArray\(ByteArrayTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteByteArray\_Void\_Minecraft\_Nbt\_ByteArrayTag\_), 
[TagWriter.WriteIntArray\(IntArrayTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteIntArray\_Void\_Minecraft\_Nbt\_IntArrayTag\_), 
[TagWriter.WriteLongArray\(LongArrayTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteLongArray\_Void\_Minecraft\_Nbt\_LongArrayTag\_), 
[TagWriter.WriteList\(ListTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteList\_Void\_Minecraft\_Nbt\_ListTag\_), 
[TagWriter.WriteCompound\(CompoundTag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteCompound\_Void\_Minecraft\_Nbt\_CompoundTag\_), 
[TagWriter.WriteBuilder\(TagBuilder\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteBuilder\_Void\_Minecraft\_Nbt\_TagBuilder\_), 
[TagWriter.WriteEndTag\(EndTag?\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteEndTag\_Void\_Minecraft\_Nbt\_EndTag\_), 
[TagWriter.WriteTag\(Tag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteTag\_Void\_Minecraft\_Nbt\_Tag\_), 
[TagWriter.WriteTagAsync\(Tag\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_WriteTagAsync\_Void\_Minecraft\_Nbt\_Tag\_), 
[TagWriter.Dispose\(\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_Dispose), 
[TagWriter.DisposeAsync\(\)](Void.Minecraft.Nbt.TagWriter.md\#Void\_Minecraft\_Nbt\_TagWriter\_DisposeAsync), 
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

## Properties

### <a id="Void_Minecraft_Nbt_BufferedTagWriter_Capacity"></a> Capacity

Gets the capacity of the internal buffer.

```csharp
public long Capacity { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

#### Remarks

The capacity will expand automatically as-needed.

### <a id="Void_Minecraft_Nbt_BufferedTagWriter_Length"></a> Length

Gets the number of bytes in the internal buffer.

```csharp
public long Length { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Void_Minecraft_Nbt_BufferedTagWriter_CopyTo_System_IO_Stream_"></a> CopyTo\(Stream\)

Copies the internal buffer to the specified <code class="paramref">stream</code>;

```csharp
public void CopyTo(Stream stream)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

A <xref href="System.IO.Stream" data-throw-if-not-resolved="false"></xref> instance to write to.

### <a id="Void_Minecraft_Nbt_BufferedTagWriter_CopyToAsync_System_IO_Stream_"></a> CopyToAsync\(Stream\)

Asynchronously copies the internal buffer to the specified <code class="paramref">stream</code>;

```csharp
public Task CopyToAsync(Stream stream)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

A <xref href="System.IO.Stream" data-throw-if-not-resolved="false"></xref> instance to write to.

#### Returns

 [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)

### <a id="Void_Minecraft_Nbt_BufferedTagWriter_Create_Void_Minecraft_Nbt_CompressionType_Void_Minecraft_Nbt_FormatOptions_"></a> Create\(CompressionType, FormatOptions\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.BufferedTagWriter" data-throw-if-not-resolved="false"></xref> class.

```csharp
public static BufferedTagWriter Create(CompressionType compression, FormatOptions options)
```

#### Parameters

`compression` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

Indicates the compression algorithm used to compress the file.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

#### Returns

 [BufferedTagWriter](Void.Minecraft.Nbt.BufferedTagWriter.md)

A newly created <xref href="Void.Minecraft.Nbt.BufferedTagWriter" data-throw-if-not-resolved="false"></xref> instance.

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

Thrown when an invalid compression type is specified.

### <a id="Void_Minecraft_Nbt_BufferedTagWriter_Create_Void_Minecraft_Nbt_CompressionType_Void_Minecraft_Nbt_FormatOptions_System_Int32_"></a> Create\(CompressionType, FormatOptions, int\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.BufferedTagWriter" data-throw-if-not-resolved="false"></xref> class.

```csharp
public static BufferedTagWriter Create(CompressionType compression, FormatOptions options, int capacity)
```

#### Parameters

`compression` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

Indicates the compression algorithm used to compress the file.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The initial capacity of the buffer.

#### Returns

 [BufferedTagWriter](Void.Minecraft.Nbt.BufferedTagWriter.md)

A newly created <xref href="Void.Minecraft.Nbt.BufferedTagWriter" data-throw-if-not-resolved="false"></xref> instance.

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

Thrown when an invalid compression type is specified.

### <a id="Void_Minecraft_Nbt_BufferedTagWriter_ToArray"></a> ToArray\(\)

Gets the internal buffer as an array of bytes containing the NBT data written so far.

```csharp
public byte[] ToArray()
```

#### Returns

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

An array of bytes containing the NBT data.

## Operators

### <a id="Void_Minecraft_Nbt_BufferedTagWriter_op_Implicit_Void_Minecraft_Nbt_BufferedTagWriter__System_ReadOnlySpan_System_Byte_"></a> implicit operator ReadOnlySpan<byte\>\(BufferedTagWriter\)

Implicit conversion of <xref href="Void.Minecraft.Nbt.BufferedTagWriter" data-throw-if-not-resolved="false"></xref> to a <xref href="System.ReadOnlySpan%601" data-throw-if-not-resolved="false"></xref>.

```csharp
public static implicit operator ReadOnlySpan<byte>(BufferedTagWriter writer)
```

#### Parameters

`writer` [BufferedTagWriter](Void.Minecraft.Nbt.BufferedTagWriter.md)

#### Returns

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

