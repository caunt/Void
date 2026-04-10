# <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions"></a> Class ReadMinecraftBufferExtensions

Namespace: [Void.Minecraft.Buffers.Extensions](Void.Minecraft.Buffers.Extensions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class ReadMinecraftBufferExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ReadMinecraftBufferExtensions](Void.Minecraft.Buffers.Extensions.ReadMinecraftBufferExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_Dump__1___0__"></a> Dump<TBuffer\>\(ref TBuffer\)

Debug only. Returns a read-only span of bytes from the specified buffer starting at index 0 up to its length.

```csharp
public static ReadOnlySpan<byte> Dump<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to a buffer from which the byte span is accessed.

#### Returns

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The method returns a read-only span of bytes representing the contents of the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_Read__1___0__System_Int32_"></a> Read<TBuffer\>\(ref TBuffer, int\)

Reads a specified number of bytes from a buffer and returns them as a read-only span.

```csharp
public static ReadOnlySpan<byte> Read<TBuffer>(this ref TBuffer buffer, int length) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer from which bytes are read and manipulated.

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The number of bytes to read from the buffer.

#### Returns

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A read-only span containing the bytes that were read from the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadBoolean__1___0__"></a> ReadBoolean<TBuffer\>\(ref TBuffer\)

Reads a boolean value from the provided buffer.

```csharp
public static bool ReadBoolean<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer from which the boolean value is read.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Returns the boolean value read from the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for reading data.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadByteArray__1___0__"></a> ReadByteArray<TBuffer\>\(ref TBuffer\)

```csharp
public static ReadOnlySpan<byte> ReadByteArray<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

#### Returns

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

#### Type Parameters

`TBuffer` 

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadComponent__1___0__"></a> ReadComponent<TBuffer\>\(ref TBuffer\)

Reads an NBT-encoded component from the specified buffer and deserializes it into a <xref href="Void.Minecraft.Components.Text.Component" data-throw-if-not-resolved="false"></xref>.

```csharp
public static Component ReadComponent<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

A reference to the buffer from which the NBT-encoded component is read.

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

The <xref href="Void.Minecraft.Components.Text.Component" data-throw-if-not-resolved="false"></xref> deserialized from the NBT tag read out of the buffer.

#### Type Parameters

`TBuffer` 

The type of the buffer, which must be a value type that implements the
    <xref href="Void.Minecraft.Buffers.IMinecraftBuffer%601" data-throw-if-not-resolved="false"></xref> interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadDouble__1___0__"></a> ReadDouble<TBuffer\>\(ref TBuffer\)

Reads an 8-byte double value from a buffer in big-endian format.

```csharp
public static double ReadDouble<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The parameter provides access to the data from which the double value is read.

#### Returns

 [double](https://learn.microsoft.com/dotnet/api/system.double)

The method returns the double value read from the buffer.

#### Type Parameters

`TBuffer` 

The type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadFloat__1___0__"></a> ReadFloat<TBuffer\>\(ref TBuffer\)

Reads a 4-byte floating-point value from a buffer in big-endian format.

```csharp
public static float ReadFloat<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer from which the floating-point value is read.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

The method returns the floating-point value read from the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadInt__1___0__"></a> ReadInt<TBuffer\>\(ref TBuffer\)

Reads a 32-bit integer from a buffer in big-endian format.

```csharp
public static int ReadInt<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer from which the integer is read, passed by reference.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

The 32-bit integer read from the buffer.

#### Type Parameters

`TBuffer` 

The type used for the buffer must be a struct that implements a specific interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadIntArray__1___0__"></a> ReadIntArray<TBuffer\>\(ref TBuffer\)

Reads a sequence of 32-bit integers from the specified Minecraft buffer.

```csharp
public static ReadOnlyMemory<int> ReadIntArray<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

A reference to the buffer from which the integer array is read. The buffer must be a valid instance of a type
    that implements IMinecraftBuffer.

#### Returns

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

A read-only span containing the integers read from the buffer.

#### Type Parameters

`TBuffer` 

The type of the buffer that implements the IMinecraftBuffer interface.

#### Remarks

This method is an extension for types implementing IMinecraftBuffer, enabling efficient
    reading of integer arrays without additional allocations. The returned span is only valid as long as the
    underlying buffer remains unchanged.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadLong__1___0__"></a> ReadLong<TBuffer\>\(ref TBuffer\)

Reads a 64-bit integer from a buffer in big-endian format.

```csharp
public static long ReadLong<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The parameter provides access to the data from which the integer is read.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

Returns the 64-bit integer read from the buffer.

#### Type Parameters

`TBuffer` 

The type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadProperty__1___0__"></a> ReadProperty<TBuffer\>\(ref TBuffer\)

Reads a property from a buffer and returns it.

```csharp
public static Property ReadProperty<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The parameter is a reference to the buffer from which the property is read.

#### Returns

 [Property](Void.Minecraft.Profiles.Property.md)

The method returns the property read from the buffer.

#### Type Parameters

`TBuffer` 

The type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadPropertyArray__1___0__System_Int32_"></a> ReadPropertyArray<TBuffer\>\(ref TBuffer, int\)

Reads an array of properties from a buffer, allowing for a specified number of properties to be read.

```csharp
public static ReadOnlyMemory<Property> ReadPropertyArray<TBuffer>(this ref TBuffer buffer, int count = -1) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer from which properties are read, passed by reference to allow modifications.

`count` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Specifies the number of properties to read from the buffer, with a default value indicating all properties
    should be read.

#### Returns

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[Property](Void.Minecraft.Profiles.Property.md)\>

An array of properties read from the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for reading data.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadShort__1___0__"></a> ReadShort<TBuffer\>\(ref TBuffer\)

Reads a 16-bit short value from a buffer in big-endian format.

```csharp
public static short ReadShort<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The parameter provides access to the data from which the short value is read.

#### Returns

 [short](https://learn.microsoft.com/dotnet/api/system.int16)

Returns the 16-bit short value read from the buffer.

#### Type Parameters

`TBuffer` 

The type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadString__1___0__System_Int32_"></a> ReadString<TBuffer\>\(ref TBuffer, int\)

Reads a string from a buffer with a specified maximum length.

```csharp
public static string ReadString<TBuffer>(this ref TBuffer buffer, int maxLength = 32767) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer from which the string is read.

`maxLength` [int](https://learn.microsoft.com/dotnet/api/system.int32)

This parameter defines the maximum number of characters to read from the buffer.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

The method returns the string read from the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadTag__1___0__System_Boolean_"></a> ReadTag<TBuffer\>\(ref TBuffer, bool\)

Reads a single NBT tag from the buffer, advancing the buffer position by the number of bytes consumed.

```csharp
public static NbtTag ReadTag<TBuffer>(this ref TBuffer buffer, bool readName = true) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

A reference to the buffer from which the NBT tag is read.

`readName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

When <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>, reads the tag's name prefix from the binary stream (a type byte followed
by a two-byte-length-prefixed UTF-8 name string). When <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>, only the type byte and
payload are read, which is appropriate for tags embedded inside a List where names are omitted.
Defaults to <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>.

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

The <xref href="Void.Minecraft.Nbt.NbtTag" data-throw-if-not-resolved="false"></xref> parsed from the buffer.

#### Type Parameters

`TBuffer` 

The buffer type. Must be a value type implementing <xref href="Void.Minecraft.Buffers.IMinecraftBuffer%601" data-throw-if-not-resolved="false"></xref>.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadToEnd__1___0__"></a> ReadToEnd<TBuffer\>\(ref TBuffer\)

Reads all remaining data from a buffer and returns it as a read-only span of bytes.

```csharp
public static ReadOnlySpan<byte> ReadToEnd<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer from which data will be read.

#### Returns

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A read-only span containing the bytes read from the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadUnsignedByte__1___0__"></a> ReadUnsignedByte<TBuffer\>\(ref TBuffer\)

Reads a single byte from the provided buffer.

```csharp
public static byte ReadUnsignedByte<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The parameter is a reference to a buffer from which a byte will be read.

#### Returns

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

Returns the byte read from the buffer.

#### Type Parameters

`TBuffer` 

The type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadUnsignedShort__1___0__"></a> ReadUnsignedShort<TBuffer\>\(ref TBuffer\)

Reads a 16-bit unsigned integer from a buffer in big-endian format.

```csharp
public static ushort ReadUnsignedShort<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The parameter provides access to the data from which the unsigned short is read.

#### Returns

 [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

Returns the 16-bit unsigned integer read from the buffer.

#### Type Parameters

`TBuffer` 

The type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadUuid__1___0__"></a> ReadUuid<TBuffer\>\(ref TBuffer\)

Reads a UUID from a buffer by extracting two long values.

```csharp
public static Uuid ReadUuid<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer from which the UUID is read.

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

Returns a UUID constructed from the two long values read from the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for reading data.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadUuidAsIntArray__1___0__"></a> ReadUuidAsIntArray<TBuffer\>\(ref TBuffer\)

Reads a UUID from a buffer and returns it as an integer array.

```csharp
public static Uuid ReadUuidAsIntArray<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer is used to read the UUID data from its current position.

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

An integer array representing the UUID.

#### Type Parameters

`TBuffer` 

This type is a struct that implements a specific buffer interface for reading data.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadVarInt__1___0__"></a> ReadVarInt<TBuffer\>\(ref TBuffer\)

Reads a variable-length integer from a buffer.

```csharp
public static int ReadVarInt<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The parameter is a reference to the buffer from which the variable-length integer is read.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

Returns the integer value read from the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for reading data.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadVarIntArray__1___0__"></a> ReadVarIntArray<TBuffer\>\(ref TBuffer\)

```csharp
public static ReadOnlyMemory<int> ReadVarIntArray<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

#### Returns

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

#### Type Parameters

`TBuffer` 

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadVarLong__1___0__"></a> ReadVarLong<TBuffer\>\(ref TBuffer\)

Reads a variable-length long value from a buffer.

```csharp
public static long ReadVarLong<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The parameter is a reference to the buffer from which the long value is read.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

Returns the long value read from the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for reading data.

### <a id="Void_Minecraft_Buffers_Extensions_ReadMinecraftBufferExtensions_ReadVarShort__1___0__"></a> ReadVarShort<TBuffer\>\(ref TBuffer\)

Reads a variable-length short value from a buffer.

```csharp
public static int ReadVarShort<TBuffer>(this ref TBuffer buffer) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The reference to the buffer from which the variable-length short value is read.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

Returns the short value read from the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for reading data.

