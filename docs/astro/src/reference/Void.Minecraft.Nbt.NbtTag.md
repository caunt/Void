# <a id="Void_Minecraft_Nbt_NbtTag"></a> Class NbtTag

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

```csharp
public abstract record NbtTag : IEquatable<NbtTag>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Derived

[NbtByte](Void.Minecraft.Nbt.Tags.NbtByte.md), 
[NbtByteArray](Void.Minecraft.Nbt.Tags.NbtByteArray.md), 
[NbtCompound](Void.Minecraft.Nbt.Tags.NbtCompound.md), 
[NbtDouble](Void.Minecraft.Nbt.Tags.NbtDouble.md), 
[NbtEnd](Void.Minecraft.Nbt.Tags.NbtEnd.md), 
[NbtFloat](Void.Minecraft.Nbt.Tags.NbtFloat.md), 
[NbtInt](Void.Minecraft.Nbt.Tags.NbtInt.md), 
[NbtIntArray](Void.Minecraft.Nbt.Tags.NbtIntArray.md), 
[NbtList](Void.Minecraft.Nbt.Tags.NbtList.md), 
[NbtLong](Void.Minecraft.Nbt.Tags.NbtLong.md), 
[NbtLongArray](Void.Minecraft.Nbt.Tags.NbtLongArray.md), 
[NbtShort](Void.Minecraft.Nbt.Tags.NbtShort.md), 
[NbtString](Void.Minecraft.Nbt.Tags.NbtString.md), 
[NbtTag<T\>](Void.Minecraft.Nbt.NbtTag\-1.md)

#### Implements

[IEquatable<NbtTag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Properties

### <a id="Void_Minecraft_Nbt_NbtTag_Name"></a> Name

```csharp
public string? Name { get; set; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Minecraft_Nbt_NbtTag_Type"></a> Type

```csharp
public NbtTagType Type { get; }
```

#### Property Value

 [NbtTagType](Void.Minecraft.Nbt.NbtTagType.md)

## Methods

### <a id="Void_Minecraft_Nbt_NbtTag_AsJsonNode"></a> AsJsonNode\(\)

```csharp
public JsonNode AsJsonNode()
```

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

### <a id="Void_Minecraft_Nbt_NbtTag_AsStream_Void_Minecraft_Nbt_NbtFormatOptions_System_Boolean_"></a> AsStream\(NbtFormatOptions, bool\)

```csharp
public MemoryStream AsStream(NbtFormatOptions formatOptions = NbtFormatOptions.BigEndian, bool writeName = true)
```

#### Parameters

`formatOptions` [NbtFormatOptions](Void.Minecraft.Nbt.NbtFormatOptions.md)

`writeName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [MemoryStream](https://learn.microsoft.com/dotnet/api/system.io.memorystream)

### <a id="Void_Minecraft_Nbt_NbtTag_Parse_System_String_"></a> Parse\(string\)

```csharp
public static NbtTag Parse(string data)
```

#### Parameters

`data` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Nbt_NbtTag_Parse_System_ReadOnlyMemory_System_Byte__Void_Minecraft_Nbt_NbtTag__System_Boolean_Void_Minecraft_Nbt_NbtFormatOptions_"></a> Parse\(ReadOnlyMemory<byte\>, out NbtTag, bool, NbtFormatOptions\)

```csharp
public static long Parse(ReadOnlyMemory<byte> data, out NbtTag result, bool readName = true, NbtFormatOptions formatOptions = NbtFormatOptions.BigEndian)
```

#### Parameters

`data` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

`result` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

`readName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`formatOptions` [NbtFormatOptions](Void.Minecraft.Nbt.NbtFormatOptions.md)

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Nbt_NbtTag_Parse__1_System_ReadOnlyMemory_System_Byte____0__System_Boolean_Void_Minecraft_Nbt_NbtFormatOptions_"></a> Parse<T\>\(ReadOnlyMemory<byte\>, out T, bool, NbtFormatOptions\)

```csharp
public static long Parse<T>(ReadOnlyMemory<byte> data, out T result, bool readName = true, NbtFormatOptions formatOptions = NbtFormatOptions.BigEndian) where T : NbtTag
```

#### Parameters

`data` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

`result` T

`readName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`formatOptions` [NbtFormatOptions](Void.Minecraft.Nbt.NbtFormatOptions.md)

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Nbt_NbtTag_ReadFrom__1___0__System_Boolean_"></a> ReadFrom<TBuffer\>\(ref TBuffer, bool\)

```csharp
public static NbtTag ReadFrom<TBuffer>(ref TBuffer buffer, bool readName = true) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

`readName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Type Parameters

`TBuffer` 

### <a id="Void_Minecraft_Nbt_NbtTag_ToSnbt"></a> ToSnbt\(\)

```csharp
protected string ToSnbt()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Nbt_NbtTag_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

## Operators

### <a id="Void_Minecraft_Nbt_NbtTag_op_Implicit_Void_Minecraft_Nbt_Tag__Void_Minecraft_Nbt_NbtTag"></a> implicit operator NbtTag\(Tag\)

```csharp
public static implicit operator NbtTag(Tag tag)
```

#### Parameters

`tag` [Tag](Void.Minecraft.Nbt.Tag.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Nbt_NbtTag_op_Implicit_Void_Minecraft_Nbt_NbtTag__Void_Minecraft_Nbt_Tag"></a> implicit operator Tag\(NbtTag\)

```csharp
public static implicit operator Tag(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [Tag](Void.Minecraft.Nbt.Tag.md)

