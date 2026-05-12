# <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions"></a> Class WriteMinecraftBufferExtensions

Namespace: [Void.Minecraft.Buffers.Extensions](Void.Minecraft.Buffers.Extensions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class WriteMinecraftBufferExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[WriteMinecraftBufferExtensions](Void.Minecraft.Buffers.Extensions.WriteMinecraftBufferExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_Write__1___0__System_ReadOnlySpan_System_Byte__"></a> Write<TBuffer\>\(ref TBuffer, scoped ReadOnlySpan<byte\>\)

Writes a byte span to a buffer, allowing for efficient data handling in a structured format.

```csharp
public static void Write<TBuffer>(this ref TBuffer buffer, scoped ReadOnlySpan<byte> data) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer that will receive the byte data.

`data` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

This parameter contains the byte data that will be copied into the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a struct that implements a specific buffer interface for Minecraft data
    operations.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_Write__1___0__System_IO_Stream_"></a> Write<TBuffer\>\(ref TBuffer, Stream\)

Writes data from a stream to a specified buffer. Requires the stream to support length property.

```csharp
public static void Write<TBuffer>(this ref TBuffer buffer, Stream value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer is a reference to the structure that will receive the data from the stream.

`value` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

The stream provides the data source that will be written into the buffer.

#### Type Parameters

`TBuffer` 

The type parameter represents a structure that can be used as a buffer for writing data.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteBoolean__1___0__System_Boolean_"></a> WriteBoolean<TBuffer\>\(ref TBuffer, bool\)

Writes a boolean value to a specified buffer by converting it to a byte.

```csharp
public static void WriteBoolean<TBuffer>(this ref TBuffer buffer, bool value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer is the destination where the boolean value will be written.

`value` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

The boolean value to be converted to a byte and written to the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for writing data.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteByteArray__1___0__System_ReadOnlySpan_System_Byte__"></a> WriteByteArray<TBuffer\>\(ref TBuffer, scoped ReadOnlySpan<byte\>\)

```csharp
public static void WriteByteArray<TBuffer>(this ref TBuffer buffer, scoped ReadOnlySpan<byte> data) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

`data` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

#### Type Parameters

`TBuffer` 

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteComponent__1___0__Void_Minecraft_Components_Text_Component_"></a> WriteComponent<TBuffer\>\(ref TBuffer, Component\)

Serializes the specified component to its NBT representation and writes it to the buffer.

```csharp
public static void WriteComponent<TBuffer>(this ref TBuffer buffer, Component value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

A reference to the buffer to which the serialized component will be written.

`value` [Component](Void.Minecraft.Components.Text.Component.md)

The component to serialize and write to the buffer.

#### Type Parameters

`TBuffer` 

The type of the buffer, which must be a value type that implements the
    <xref href="Void.Minecraft.Buffers.IMinecraftBuffer%601" data-throw-if-not-resolved="false"></xref> interface.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteDouble__1___0__System_Double_"></a> WriteDouble<TBuffer\>\(ref TBuffer, double\)

Writes a double value to a specified buffer in big-endian format.

```csharp
public static void WriteDouble<TBuffer>(this ref TBuffer buffer, double value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer where the double value will be written.

`value` [double](https://learn.microsoft.com/dotnet/api/system.double)

This parameter is the double value that will be written to the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for writing data.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteFloat__1___0__System_Single_"></a> WriteFloat<TBuffer\>\(ref TBuffer, float\)

Writes a floating-point number in big-endian format to a specified buffer.

```csharp
public static void WriteFloat<TBuffer>(this ref TBuffer buffer, float value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer is modified to store the floating-point value in a specific format.

`value` [float](https://learn.microsoft.com/dotnet/api/system.single)

The floating-point number to be written into the buffer.

#### Type Parameters

`TBuffer` 

This type is a structure that implements a specific buffer interface for writing data.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteInt__1___0__System_Int32_"></a> WriteInt<TBuffer\>\(ref TBuffer, int\)

Writes a 32-bit integer to a specified buffer in big-endian format.

```csharp
public static void WriteInt<TBuffer>(this ref TBuffer buffer, int value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer is modified to include the new integer value at the appropriate position.

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The integer to be written into the buffer in big-endian format.

#### Type Parameters

`TBuffer` 

This type is a struct that implements a specific buffer interface, allowing for efficient memory access.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteIntArray__1___0__System_ReadOnlySpan_System_Int32__"></a> WriteIntArray<TBuffer\>\(ref TBuffer, scoped ReadOnlySpan<int\>\)

Writes a sequence of 32-bit integers to the specified Minecraft buffer.

```csharp
public static void WriteIntArray<TBuffer>(this ref TBuffer buffer, scoped ReadOnlySpan<int> data) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

A reference to the buffer to which the integer array will be written.

`data` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

A read-only span containing the integers to write to the buffer.

#### Type Parameters

`TBuffer` 

The type of the buffer that implements the IMinecraftBuffer interface.

#### Remarks

Use this method to efficiently serialize an array of integers into a buffer that conforms to
    the IMinecraftBuffer interface. The method is intended for use with value-type buffers and supports efficient,
    allocation-free writing.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteLong__1___0__System_Int64_"></a> WriteLong<TBuffer\>\(ref TBuffer, long\)

Writes a 64-bit integer to a specified buffer in big-endian format.

```csharp
public static void WriteLong<TBuffer>(this ref TBuffer buffer, long value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer is modified to accommodate the new data being written.

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The 64-bit integer to be written into the buffer.

#### Type Parameters

`TBuffer` 

This type is a struct that implements a specific buffer interface for writing data.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteProperty__1___0__Void_Minecraft_Profiles_Property_"></a> WriteProperty<TBuffer\>\(ref TBuffer, Property\)

Writes a property to a specified buffer.

```csharp
public static void WriteProperty<TBuffer>(this ref TBuffer buffer, Property value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer where the property will be written.

`value` [Property](Void.Minecraft.Profiles.Property.md)

This parameter represents the property that is to be written to the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for Minecraft data
    handling.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WritePropertyArray__1___0__Void_Minecraft_Profiles_Property___"></a> WritePropertyArray<TBuffer\>\(ref TBuffer, Property\[\]?\)

Writes an array of properties to a specified buffer.

```csharp
public static void WritePropertyArray<TBuffer>(this ref TBuffer buffer, Property[]? value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer to which the array of properties will be written.

`value` [Property](Void.Minecraft.Profiles.Property.md)\[\]?

The array of properties that will be written to the buffer.

#### Type Parameters

`TBuffer` 

This type is a structure that implements a specific buffer interface for Minecraft data handling.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteShort__1___0__System_Int16_"></a> WriteShort<TBuffer\>\(ref TBuffer, short\)

Writes a short integer to a specified buffer in big-endian format.

```csharp
public static void WriteShort<TBuffer>(this ref TBuffer buffer, short value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer where the short integer will be written.

`value` [short](https://learn.microsoft.com/dotnet/api/system.int16)

This parameter is the short integer value to be written to the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteString__1___0__System_ReadOnlySpan_System_Char__"></a> WriteString<TBuffer\>\(ref TBuffer, ReadOnlySpan<char\>\)

Writes a string to a specified buffer.

```csharp
public static void WriteString<TBuffer>(this ref TBuffer buffer, ReadOnlySpan<char> value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer where the string will be written.

`value` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[char](https://learn.microsoft.com/dotnet/api/system.char)\>

The characters that will be written to the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for Minecraft.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteTag__1___0__Void_Minecraft_Nbt_NbtTag_System_Boolean_"></a> WriteTag<TBuffer\>\(ref TBuffer, NbtTag, bool\)

Writes a tag to a specified buffer using a stream representation of the tag.

```csharp
public static void WriteTag<TBuffer>(this ref TBuffer buffer, NbtTag value, bool writeName = true) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer where the tag will be written.

`value` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

This parameter represents the tag that will be converted to a stream and written to the buffer.

`writeName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

This parameter indicates whether the name of the tag should be included when writing to the buffer. Default is true.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for Minecraft data
    handling.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteUnsignedByte__1___0__System_Byte_"></a> WriteUnsignedByte<TBuffer\>\(ref TBuffer, byte\)

Writes a single byte value to a specified buffer.

```csharp
public static void WriteUnsignedByte<TBuffer>(this ref TBuffer buffer, byte value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer is modified to store the byte value at a designated position.

`value` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

The byte value to be written into the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteUnsignedShort__1___0__System_UInt16_"></a> WriteUnsignedShort<TBuffer\>\(ref TBuffer, ushort\)

Writes an unsigned short value to a specified buffer in big-endian format.

```csharp
public static void WriteUnsignedShort<TBuffer>(this ref TBuffer buffer, ushort value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is a reference to the buffer where the unsigned short value will be written.

`value` [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

This parameter is the unsigned short value that will be written to the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for writing operations.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteUuid__1___0__Void_Minecraft_Profiles_Uuid_"></a> WriteUuid<TBuffer\>\(ref TBuffer, Uuid\)

Writes a UUID value to a specified buffer in a Minecraft-compatible format.

```csharp
public static void WriteUuid<TBuffer>(this ref TBuffer buffer, Uuid value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer is the destination where the UUID will be written.

`value` [Uuid](Void.Minecraft.Profiles.Uuid.md)

The UUID value to be converted and written to the buffer.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for Minecraft data
    handling.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteUuidAsIntArray__1___0__Void_Minecraft_Profiles_Uuid_"></a> WriteUuidAsIntArray<TBuffer\>\(ref TBuffer, Uuid\)

Writes a UUID as an integer array into a specified buffer.

```csharp
public static void WriteUuidAsIntArray<TBuffer>(this ref TBuffer buffer, Uuid value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer is modified to include the integer representation of the UUID.

`value` [Uuid](Void.Minecraft.Profiles.Uuid.md)

The UUID is converted and written into the buffer as an array of integers.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for Minecraft.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteVarInt__1___0__System_Int32_"></a> WriteVarInt<TBuffer\>\(ref TBuffer, int\)

Writes a variable-length integer to a specified buffer.

```csharp
public static void WriteVarInt<TBuffer>(this ref TBuffer buffer, int value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer is the destination where the variable-length integer will be written.

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The integer value to be written to the buffer in a variable-length format.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for Minecraft data
    handling.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteVarIntArray__1___0__System_ReadOnlySpan_System_Int32__"></a> WriteVarIntArray<TBuffer\>\(ref TBuffer, scoped ReadOnlySpan<int\>\)

Writes a sequence of variable-length integers to the specified buffer.

```csharp
public static void WriteVarIntArray<TBuffer>(this ref TBuffer buffer, scoped ReadOnlySpan<int> data) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

A reference to the buffer to which the variable-length integers will be written.

`data` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

A read-only span containing the integers to write as variable-length values.

#### Type Parameters

`TBuffer` 

The type of the buffer that implements the IMinecraftBuffer interface.

#### Remarks

This method is intended for use with buffers that implement the IMinecraftBuffer interface,
    enabling efficient serialization of integer arrays in variable-length format.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteVarLong__1___0__System_Int64_"></a> WriteVarLong<TBuffer\>\(ref TBuffer, long\)

Writes a variable-length long integer to a buffer.

```csharp
public static void WriteVarLong<TBuffer>(this ref TBuffer buffer, long value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer to which the variable-length long integer will be written.

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The long integer value that will be written to the buffer.

#### Type Parameters

`TBuffer` 

This type is a struct that implements a specific buffer interface for Minecraft data handling.

### <a id="Void_Minecraft_Buffers_Extensions_WriteMinecraftBufferExtensions_WriteVarShort__1___0__System_Int32_"></a> WriteVarShort<TBuffer\>\(ref TBuffer, int\)

Writes a variable-length short integer to the specified buffer.

```csharp
public static void WriteVarShort<TBuffer>(this ref TBuffer buffer, int value) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

The buffer is the destination where the variable-length short integer will be written.

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The integer value to be written to the buffer in a variable-length format.

#### Type Parameters

`TBuffer` 

This type is a struct that implements a specific buffer interface for Minecraft data handling.

