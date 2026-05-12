# <a id="Void_Minecraft_Nbt_NbtReader"></a> Class NbtReader

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class NbtReader : TagReader, IDisposable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TagIO](Void.Minecraft.Nbt.TagIO.md) ← 
[TagReader](Void.Minecraft.Nbt.TagReader.md) ← 
[NbtReader](Void.Minecraft.Nbt.NbtReader.md)

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

#### Inherited Members

[TagReader.TagRead](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_TagRead), 
[TagReader.TagEncountered](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_TagEncountered), 
[TagReader.ReadByte\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadByte\_System\_Boolean\_), 
[TagReader.ReadShort\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadShort\_System\_Boolean\_), 
[TagReader.ReadInt\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadInt\_System\_Boolean\_), 
[TagReader.ReadLong\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadLong\_System\_Boolean\_), 
[TagReader.ReadFloat\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadFloat\_System\_Boolean\_), 
[TagReader.ReadDouble\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadDouble\_System\_Boolean\_), 
[TagReader.ReadString\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadString\_System\_Boolean\_), 
[TagReader.ReadByteArray\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadByteArray\_System\_Boolean\_), 
[TagReader.ReadIntArray\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadIntArray\_System\_Boolean\_), 
[TagReader.ReadLongArray\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadLongArray\_System\_Boolean\_), 
[TagReader.ReadList\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadList\_System\_Boolean\_), 
[TagReader.ReadCompound\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadCompound\_System\_Boolean\_), 
[TagReader.ReadTag\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadTag\_System\_Boolean\_), 
[TagReader.ReadTagAsync\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadTagAsync\_System\_Boolean\_), 
[TagReader.ReadTag<T\>\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadTag\_\_1\_System\_Boolean\_), 
[TagReader.ReadTagAsync<T\>\(bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadTagAsync\_\_1\_System\_Boolean\_), 
[TagReader.ReadUTF8String\(\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadUTF8String), 
[TagReader.ReadToFixSizedBuffer\(Span<byte\>\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadToFixSizedBuffer\_System\_Span\_System\_Byte\_\_), 
[TagReader.ReadToFixSizedBuffer\(byte\[\], int, int\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_ReadToFixSizedBuffer\_System\_Byte\_\_\_System\_Int32\_System\_Int32\_), 
[TagReader.Dispose\(\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_Dispose), 
[TagReader.DisposeAsync\(\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_DisposeAsync), 
[TagReader.OnTagRead\(Tag\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_OnTagRead\_Void\_Minecraft\_Nbt\_Tag\_), 
[TagReader.OnTagEncountered\(TagType, bool\)](Void.Minecraft.Nbt.TagReader.md\#Void\_Minecraft\_Nbt\_TagReader\_OnTagEncountered\_Void\_Minecraft\_Nbt\_TagType\_System\_Boolean\_), 
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

### <a id="Void_Minecraft_Nbt_NbtReader__ctor_System_IO_Stream_Void_Minecraft_Nbt_FormatOptions_System_Boolean_"></a> NbtReader\(Stream, FormatOptions, bool\)

```csharp
public NbtReader(Stream stream, FormatOptions options, bool leaveOpen = false)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

`leaveOpen` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Methods

### <a id="Void_Minecraft_Nbt_NbtReader_ReadCompound_System_Boolean_"></a> ReadCompound\(bool\)

```csharp
public CompoundTag ReadCompound(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

### <a id="Void_Minecraft_Nbt_NbtReader_ReadList_System_Boolean_"></a> ReadList\(bool\)

```csharp
public ListTag ReadList(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [ListTag](Void.Minecraft.Nbt.ListTag.md)

### <a id="Void_Minecraft_Nbt_NbtReader_ReadTag_System_Boolean_"></a> ReadTag\(bool\)

```csharp
public Tag ReadTag(bool named = true)
```

#### Parameters

`named` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [Tag](Void.Minecraft.Nbt.Tag.md)

