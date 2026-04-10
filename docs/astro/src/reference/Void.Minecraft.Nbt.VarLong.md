# <a id="Void_Minecraft_Nbt_VarLong"></a> Class VarLong

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Provides static methods for reading and writing variable-length integers that are up to 10 bytes from both streams and buffers.

```csharp
public static class VarLong
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[VarLong](Void.Minecraft.Nbt.VarLong.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Nbt_VarLong_Decode_System_Byte___System_Int32_System_Int32_System_Int32__System_Boolean_"></a> Decode\(byte\[\], int, int, out int, bool\)

Decodes a buffer of bytes that represent a variable-length integer up to 10 bytes long.

```csharp
public static long Decode(byte[] buffer, int offset, int count, out int size, bool zigzag = false)
```

#### Parameters

`buffer` [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

A buffer containing the data to be decoded.

`offset` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The offset into the <code class="paramref">buffer</code> to begin reading.

`count` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The maximum number of bytes that should be read from the <code class="paramref">buffer</code>.

`size` [int](https://learn.microsoft.com/dotnet/api/system.int32)

A variable to store the actual number of bytes read from the <code class="paramref">buffer</code>.

`zigzag` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if the value is ZigZag encoded.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

The decoded value.

### <a id="Void_Minecraft_Nbt_VarLong_Decode_System_Byte___System_Int32_System_Int32_System_Boolean_"></a> Decode\(byte\[\], int, int, bool\)

Decodes a buffer of bytes that represent a variable-length integer up to 10 bytes long.

```csharp
public static long Decode(byte[] buffer, int offset, int count, bool zigzag = false)
```

#### Parameters

`buffer` [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

A buffer containing the data to be decoded.

`offset` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The offset into the <code class="paramref">buffer</code> to begin reading.

`count` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The maximum number of bytes that should be read from the <code class="paramref">buffer</code>.

`zigzag` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if the value is ZigZag encoded.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

The decoded value.

### <a id="Void_Minecraft_Nbt_VarLong_Decode_System_ReadOnlySpan_System_Byte__System_Int32__System_Boolean_"></a> Decode\(ReadOnlySpan<byte\>, out int, bool\)

Decodes a buffer of bytes that represent a variable-length integer up to 10 bytes long.

```csharp
public static long Decode(ReadOnlySpan<byte> buffer, out int size, bool zigzag = false)
```

#### Parameters

`buffer` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A buffer containing the data to be decoded.

`size` [int](https://learn.microsoft.com/dotnet/api/system.int32)

A variable to store the actual number of bytes read from the <code class="paramref">buffer</code>.

`zigzag` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if the value is ZigZag encoded.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

The decoded value.

### <a id="Void_Minecraft_Nbt_VarLong_Decode_System_ReadOnlySpan_System_Byte__System_Boolean_"></a> Decode\(ReadOnlySpan<byte\>, bool\)

Decodes a buffer of bytes that represent a variable-length integer up to 10 bytes long.

```csharp
public static long Decode(ReadOnlySpan<byte> buffer, bool zigzag = false)
```

#### Parameters

`buffer` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A buffer containing the data to be decoded.

`zigzag` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if the value is ZigZag encoded.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

The decoded value.

### <a id="Void_Minecraft_Nbt_VarLong_Encode_System_Int64_System_Boolean_"></a> Encode\(long, bool\)

Encodes the given <code class="paramref">value</code> and returns an array of bytes that represent it.

```csharp
public static byte[] Encode(long value, bool zigzag = false)
```

#### Parameters

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The value to encode.

`zigzag` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if the value will be ZigZag encoded.

#### Returns

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

An array of bytes representing the <code class="paramref">value</code> as a variable length integer.

### <a id="Void_Minecraft_Nbt_VarLong_Read_System_IO_Stream_System_Boolean_"></a> Read\(Stream, bool\)

Reads up to 10 bytes from the given <code class="paramref">stream</code> and returns the VarLong value as a 64-bit integer.

```csharp
public static long Read(Stream stream, bool zigzag = false)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

A <xref href="System.IO.Stream" data-throw-if-not-resolved="false"></xref> instance to read from.

`zigzag` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if the value is ZigZag encoded.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

The parsed value read from the <code class="paramref">stream</code>.

### <a id="Void_Minecraft_Nbt_VarLong_Read_System_IO_Stream_System_Int32__System_Boolean_"></a> Read\(Stream, out int, bool\)

Reads up to 10 bytes from the given <code class="paramref">stream</code> and returns the VarLong value as a 64-bit integer.

```csharp
public static long Read(Stream stream, out int size, bool zigzag = false)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

A <xref href="System.IO.Stream" data-throw-if-not-resolved="false"></xref> instance to read from.

`size` [int](https://learn.microsoft.com/dotnet/api/system.int32)

A variable to store the number of bytes read from the <code class="paramref">stream</code>.

`zigzag` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if the value is ZigZag encoded.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

The parsed value read from the <code class="paramref">stream</code>.

### <a id="Void_Minecraft_Nbt_VarLong_Write_System_IO_Stream_System_Int64_System_Boolean_"></a> Write\(Stream, long, bool\)

Encodes the given <code class="paramref">value</code> to a variable-length integer up to 10 bytes long, and writes it to the <code class="paramref">stream</code>.

```csharp
public static int Write(Stream stream, long value, bool zigzag = false)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

A <xref href="System.IO.Stream" data-throw-if-not-resolved="false"></xref> instance to write the value to.

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The value to encode and write.

`zigzag` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if the value will be ZigZag encoded.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

The number of bytes written to the <code class="paramref">stream</code>.

