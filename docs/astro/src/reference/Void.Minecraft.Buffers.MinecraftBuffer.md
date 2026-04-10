# <a id="Void_Minecraft_Buffers_MinecraftBuffer"></a> Struct MinecraftBuffer

Namespace: [Void.Minecraft.Buffers](Void.Minecraft.Buffers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public ref struct MinecraftBuffer
```

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Buffers_MinecraftBuffer__ctor"></a> MinecraftBuffer\(\)

```csharp
public MinecraftBuffer()
```

### <a id="Void_Minecraft_Buffers_MinecraftBuffer__ctor_System_Span_System_Byte__"></a> MinecraftBuffer\(Span<byte\>\)

```csharp
public MinecraftBuffer(Span<byte> memory)
```

#### Parameters

`memory` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Minecraft_Buffers_MinecraftBuffer__ctor_System_ReadOnlySpan_System_Byte__"></a> MinecraftBuffer\(ReadOnlySpan<byte\>\)

```csharp
public MinecraftBuffer(ReadOnlySpan<byte> span)
```

#### Parameters

`span` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Minecraft_Buffers_MinecraftBuffer__ctor_System_Buffers_ReadOnlySequence_System_Byte__"></a> MinecraftBuffer\(ReadOnlySequence<byte\>\)

```csharp
public MinecraftBuffer(ReadOnlySequence<byte> sequence)
```

#### Parameters

`sequence` [ReadOnlySequence](https://learn.microsoft.com/dotnet/api/system.buffers.readonlysequence\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Minecraft_Buffers_MinecraftBuffer__ctor_System_IO_MemoryStream_"></a> MinecraftBuffer\(MemoryStream\)

```csharp
public MinecraftBuffer(MemoryStream memoryStream)
```

#### Parameters

`memoryStream` [MemoryStream](https://learn.microsoft.com/dotnet/api/system.io.memorystream)

## Properties

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_HasData"></a> HasData

```csharp
public readonly bool HasData { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_Length"></a> Length

```csharp
public readonly long Length { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_Position"></a> Position

```csharp
public readonly long Position { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_CopyAsBufferSpan_System_Boolean_"></a> CopyAsBufferSpan\(bool\)

```csharp
public BufferSpan CopyAsBufferSpan(bool read = false)
```

#### Parameters

`read` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_Dump"></a> Dump\(\)

```csharp
public string Dump()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_DumpBytes"></a> DumpBytes\(\)

```csharp
public ReadOnlySpan<byte> DumpBytes()
```

#### Returns

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_DumpHex"></a> DumpHex\(\)

```csharp
public string DumpHex()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_GetVarIntSize_System_Int32_"></a> GetVarIntSize\(int\)

```csharp
public static int GetVarIntSize(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_Read_System_Int64_"></a> Read\(long\)

```csharp
public ReadOnlySpan<byte> Read(long length)
```

#### Parameters

`length` [long](https://learn.microsoft.com/dotnet/api/system.int64)

#### Returns

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadBoolean"></a> ReadBoolean\(\)

```csharp
public bool ReadBoolean()
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadComponent_System_Boolean_"></a> ReadComponent\(bool\)

```csharp
public Component ReadComponent(bool asNbt = true)
```

#### Parameters

`asNbt` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadDouble"></a> ReadDouble\(\)

```csharp
public double ReadDouble()
```

#### Returns

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadFloat"></a> ReadFloat\(\)

```csharp
public float ReadFloat()
```

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadInt"></a> ReadInt\(\)

```csharp
public int ReadInt()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadJsonString"></a> ReadJsonString\(\)

```csharp
public JsonNode ReadJsonString()
```

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadLong"></a> ReadLong\(\)

```csharp
public long ReadLong()
```

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadProperty"></a> ReadProperty\(\)

```csharp
public Property ReadProperty()
```

#### Returns

 [Property](Void.Minecraft.Profiles.Property.md)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadPropertyArray"></a> ReadPropertyArray\(\)

```csharp
public Property[] ReadPropertyArray()
```

#### Returns

 [Property](Void.Minecraft.Profiles.Property.md)\[\]

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadShort"></a> ReadShort\(\)

```csharp
public short ReadShort()
```

#### Returns

 [short](https://learn.microsoft.com/dotnet/api/system.int16)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadString_System_Int32_"></a> ReadString\(int\)

```csharp
public string ReadString(int maxLength = 32767)
```

#### Parameters

`maxLength` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadTag_System_Boolean_"></a> ReadTag\(bool\)

```csharp
public NbtTag ReadTag(bool readName = true)
```

#### Parameters

`readName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadToEnd"></a> ReadToEnd\(\)

```csharp
public ReadOnlySpan<byte> ReadToEnd()
```

#### Returns

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadUnsignedByte"></a> ReadUnsignedByte\(\)

```csharp
public byte ReadUnsignedByte()
```

#### Returns

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadUnsignedShort"></a> ReadUnsignedShort\(\)

```csharp
public ushort ReadUnsignedShort()
```

#### Returns

 [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadUuid"></a> ReadUuid\(\)

```csharp
public Uuid ReadUuid()
```

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadUuidAsIntArray"></a> ReadUuidAsIntArray\(\)

```csharp
public Uuid ReadUuidAsIntArray()
```

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadVarInt"></a> ReadVarInt\(\)

```csharp
public int ReadVarInt()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadVarLong"></a> ReadVarLong\(\)

```csharp
public long ReadVarLong()
```

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_ReadVarShort"></a> ReadVarShort\(\)

```csharp
public int ReadVarShort()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_Reset"></a> Reset\(\)

```csharp
public void Reset()
```

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_Seek_System_Int64_"></a> Seek\(long\)

```csharp
public void Seek(long offset)
```

#### Parameters

`offset` [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_Seek_System_Int64_System_IO_SeekOrigin_"></a> Seek\(long, SeekOrigin\)

```csharp
public void Seek(long offset, SeekOrigin origin)
```

#### Parameters

`offset` [long](https://learn.microsoft.com/dotnet/api/system.int64)

`origin` [SeekOrigin](https://learn.microsoft.com/dotnet/api/system.io.seekorigin)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_Write_System_ReadOnlySpan_System_Byte__"></a> Write\(scoped ReadOnlySpan<byte\>\)

```csharp
public void Write(scoped ReadOnlySpan<byte> data)
```

#### Parameters

`data` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_Write_System_IO_Stream_"></a> Write\(Stream\)

```csharp
public void Write(Stream stream)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteBoolean_System_Boolean_"></a> WriteBoolean\(bool\)

```csharp
public void WriteBoolean(bool value)
```

#### Parameters

`value` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteComponent_Void_Minecraft_Components_Text_Component_System_Boolean_System_Boolean_"></a> WriteComponent\(Component, bool, bool\)

```csharp
public void WriteComponent(Component value, bool asNbt = true, bool writeNbtName = false)
```

#### Parameters

`value` [Component](Void.Minecraft.Components.Text.Component.md)

`asNbt` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`writeNbtName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteDouble_System_Double_"></a> WriteDouble\(double\)

```csharp
public void WriteDouble(double value)
```

#### Parameters

`value` [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteFloat_System_Single_"></a> WriteFloat\(float\)

```csharp
public void WriteFloat(float value)
```

#### Parameters

`value` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteInt_System_Int32_"></a> WriteInt\(int\)

```csharp
public void WriteInt(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteJsonString_System_Text_Json_Nodes_JsonNode_System_Text_Json_JsonSerializerOptions_"></a> WriteJsonString\(JsonNode, JsonSerializerOptions?\)

```csharp
public void WriteJsonString(JsonNode node, JsonSerializerOptions? jsonSerializerOptions = null)
```

#### Parameters

`node` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

`jsonSerializerOptions` [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)?

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteLong_System_Int64_"></a> WriteLong\(long\)

```csharp
public void WriteLong(long value)
```

#### Parameters

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteProperty_Void_Minecraft_Profiles_Property_"></a> WriteProperty\(Property\)

```csharp
public void WriteProperty(Property value)
```

#### Parameters

`value` [Property](Void.Minecraft.Profiles.Property.md)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WritePropertyArray_Void_Minecraft_Profiles_Property___"></a> WritePropertyArray\(Property\[\]?\)

```csharp
public void WritePropertyArray(Property[]? value)
```

#### Parameters

`value` [Property](Void.Minecraft.Profiles.Property.md)\[\]?

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteShort_System_Int16_"></a> WriteShort\(short\)

```csharp
public void WriteShort(short value)
```

#### Parameters

`value` [short](https://learn.microsoft.com/dotnet/api/system.int16)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteString_System_ReadOnlySpan_System_Char__"></a> WriteString\(ReadOnlySpan<char\>\)

```csharp
public void WriteString(ReadOnlySpan<char> value)
```

#### Parameters

`value` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[char](https://learn.microsoft.com/dotnet/api/system.char)\>

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteTag_Void_Minecraft_Nbt_NbtTag_System_Boolean_"></a> WriteTag\(NbtTag, bool\)

```csharp
public void WriteTag(NbtTag value, bool writeName = true)
```

#### Parameters

`value` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

`writeName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteUnsignedByte_System_Byte_"></a> WriteUnsignedByte\(byte\)

```csharp
public void WriteUnsignedByte(byte value)
```

#### Parameters

`value` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteUnsignedShort_System_UInt16_"></a> WriteUnsignedShort\(ushort\)

```csharp
public void WriteUnsignedShort(ushort value)
```

#### Parameters

`value` [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteUuid_Void_Minecraft_Profiles_Uuid_"></a> WriteUuid\(Uuid\)

```csharp
public void WriteUuid(Uuid value)
```

#### Parameters

`value` [Uuid](Void.Minecraft.Profiles.Uuid.md)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteUuidAsIntArray_Void_Minecraft_Profiles_Uuid_"></a> WriteUuidAsIntArray\(Uuid\)

```csharp
public void WriteUuidAsIntArray(Uuid value)
```

#### Parameters

`value` [Uuid](Void.Minecraft.Profiles.Uuid.md)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteVarInt_System_Int32_"></a> WriteVarInt\(int\)

```csharp
public void WriteVarInt(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteVarLong_System_Int64_"></a> WriteVarLong\(long\)

```csharp
public void WriteVarLong(long value)
```

#### Parameters

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Buffers_MinecraftBuffer_WriteVarShort_System_Int32_"></a> WriteVarShort\(int\)

```csharp
public void WriteVarShort(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

