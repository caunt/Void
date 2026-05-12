# <a id="Void_Minecraft_Nbt_NbtFile"></a> Class NbtFile

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Provides static convenience methods for working with NBT-formatted files, including both reading and writing.

```csharp
public static class NbtFile
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtFile](Void.Minecraft.Nbt.NbtFile.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Nbt_NbtFile_OpenRead_System_String_Void_Minecraft_Nbt_FormatOptions_Void_Minecraft_Nbt_CompressionType_"></a> OpenRead\(string, FormatOptions, CompressionType\)

Opens an existing NBT file for reading, and returns a <xref href="Void.Minecraft.Nbt.TagReader" data-throw-if-not-resolved="false"></xref> instance for it.

```csharp
public static TagReader OpenRead(string path, FormatOptions options, CompressionType compression = CompressionType.AutoDetect)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

The path of the file to open for reading.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`compression` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

Indicates the compression algorithm used to compress the file.

#### Returns

 [TagReader](Void.Minecraft.Nbt.TagReader.md)

A <xref href="Void.Minecraft.Nbt.TagReader" data-throw-if-not-resolved="false"></xref> instance for the file stream.

#### Remarks

File compression will be automatically detected and handled when necessary.

### <a id="Void_Minecraft_Nbt_NbtFile_OpenWrite_System_String_Void_Minecraft_Nbt_FormatOptions_Void_Minecraft_Nbt_CompressionType_System_IO_Compression_CompressionLevel_"></a> OpenWrite\(string, FormatOptions, CompressionType, CompressionLevel\)

Opens an existing NBT file or creates a new one if it does not exist, and returns a <xref href="Void.Minecraft.Nbt.TagWriter" data-throw-if-not-resolved="false"></xref> instance for it.

```csharp
public static TagWriter OpenWrite(string path, FormatOptions options, CompressionType type = CompressionType.GZip, CompressionLevel level = CompressionLevel.Fastest)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

The path of the file to open for writing.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`type` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

A flag indicating the type of compression to use.

`level` [CompressionLevel](https://learn.microsoft.com/dotnet/api/system.io.compression.compressionlevel)

A flag indicating the compression strategy that will be used, if any.

#### Returns

 [TagWriter](Void.Minecraft.Nbt.TagWriter.md)

A <xref href="Void.Minecraft.Nbt.TagWriter" data-throw-if-not-resolved="false"></xref> instance for the file stream.

### <a id="Void_Minecraft_Nbt_NbtFile_Read__1_System_String_Void_Minecraft_Nbt_FormatOptions_Void_Minecraft_Nbt_CompressionType_"></a> Read<T\>\(string, FormatOptions, CompressionType\)

Reads a file at the given <code class="paramref">path</code> and deserializes the top-level <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> contained in the file.

```csharp
public static T Read<T>(string path, FormatOptions options, CompressionType compression = CompressionType.AutoDetect) where T : Tag, ICollection<Tag>
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

The path to the file to be read.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`compression` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

Indicates the compression algorithm used to compress the file.

#### Returns

 T

The deserialized <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> instance.

#### Type Parameters

`T` 

The type of tag to deserialize.

### <a id="Void_Minecraft_Nbt_NbtFile_Read_System_String_Void_Minecraft_Nbt_FormatOptions_Void_Minecraft_Nbt_CompressionType_"></a> Read\(string, FormatOptions, CompressionType\)

Reads a file at the given <code class="paramref">path</code> and deserializes the top-level <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> contained in the file.

```csharp
public static CompoundTag Read(string path, FormatOptions options, CompressionType compression = CompressionType.AutoDetect)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

The path to the file to be read.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`compression` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

Indicates the compression algorithm used to compress the file.

#### Returns

 [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

The deserialized <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance.

### <a id="Void_Minecraft_Nbt_NbtFile_ReadAsync__1_System_String_Void_Minecraft_Nbt_FormatOptions_Void_Minecraft_Nbt_CompressionType_"></a> ReadAsync<T\>\(string, FormatOptions, CompressionType\)

Asynchronously reads a file at the given <code class="paramref">path</code> and deserializes the top-level <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> contained in the file.

```csharp
public static Task<T> ReadAsync<T>(string path, FormatOptions options, CompressionType compression = CompressionType.AutoDetect) where T : Tag, ICollection<Tag>
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

The path to the file to be read.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`compression` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

Indicates the compression algorithm used to compress the file.

#### Returns

 [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task\-1)<T\>

The deserialized <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> instance.

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Nbt_NbtFile_ReadAsync_System_String_Void_Minecraft_Nbt_FormatOptions_Void_Minecraft_Nbt_CompressionType_"></a> ReadAsync\(string, FormatOptions, CompressionType\)

Asynchronously reads a file at the given <code class="paramref">path</code> and deserializes the top-level <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> contained in the file.

```csharp
public static Task<CompoundTag> ReadAsync(string path, FormatOptions options, CompressionType compression = CompressionType.AutoDetect)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

The path to the file to be read.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`compression` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

Indicates the compression algorithm used to compress the file.

#### Returns

 [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task\-1)<[CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)\>

The deserialized <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance.

### <a id="Void_Minecraft_Nbt_NbtFile_Write_System_String_Void_Minecraft_Nbt_CompoundTag_Void_Minecraft_Nbt_FormatOptions_Void_Minecraft_Nbt_CompressionType_System_IO_Compression_CompressionLevel_"></a> Write\(string, CompoundTag, FormatOptions, CompressionType, CompressionLevel\)

Writes the given <code class="paramref">tag</code> to a file at the specified <code class="paramref">path</code>.

```csharp
public static void Write(string path, CompoundTag tag, FormatOptions options, CompressionType type = CompressionType.GZip, CompressionLevel level = CompressionLevel.Fastest)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

The path to the file to be written to.

`tag` [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

The top-level <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance to be serialized.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`type` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

A flag indicating the type of compression to use.

`level` [CompressionLevel](https://learn.microsoft.com/dotnet/api/system.io.compression.compressionlevel)

Indicates a compression strategy to be used, if any.

### <a id="Void_Minecraft_Nbt_NbtFile_Write_System_String_Void_Minecraft_Nbt_ListTag_Void_Minecraft_Nbt_FormatOptions_Void_Minecraft_Nbt_CompressionType_System_IO_Compression_CompressionLevel_"></a> Write\(string, ListTag, FormatOptions, CompressionType, CompressionLevel\)

Writes the given <code class="paramref">tag</code> to a file at the specified <code class="paramref">path</code>.

```csharp
public static void Write(string path, ListTag tag, FormatOptions options, CompressionType type = CompressionType.GZip, CompressionLevel level = CompressionLevel.Fastest)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

The path to the file to be written to.

`tag` [ListTag](Void.Minecraft.Nbt.ListTag.md)

The top-level <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance to be serialized.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`type` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

A flag indicating the type of compression to use.

`level` [CompressionLevel](https://learn.microsoft.com/dotnet/api/system.io.compression.compressionlevel)

Indicates a compression strategy to be used, if any.

### <a id="Void_Minecraft_Nbt_NbtFile_WriteAsync_System_String_Void_Minecraft_Nbt_CompoundTag_Void_Minecraft_Nbt_FormatOptions_Void_Minecraft_Nbt_CompressionType_System_IO_Compression_CompressionLevel_"></a> WriteAsync\(string, CompoundTag, FormatOptions, CompressionType, CompressionLevel\)

Asynchronously writes the given <code class="paramref">tag</code> to a file at the specified <code class="paramref">path</code>.

```csharp
public static Task WriteAsync(string path, CompoundTag tag, FormatOptions options, CompressionType type = CompressionType.GZip, CompressionLevel level = CompressionLevel.Fastest)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

The path to the file to be written to.

`tag` [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

The top-level <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance to be serialized.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`type` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

A flag indicating the type of compression to use.

`level` [CompressionLevel](https://learn.microsoft.com/dotnet/api/system.io.compression.compressionlevel)

Indicates a compression strategy to be used, if any.

#### Returns

 [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)

### <a id="Void_Minecraft_Nbt_NbtFile_WriteAsync_System_String_Void_Minecraft_Nbt_ListTag_Void_Minecraft_Nbt_FormatOptions_Void_Minecraft_Nbt_CompressionType_System_IO_Compression_CompressionLevel_"></a> WriteAsync\(string, ListTag, FormatOptions, CompressionType, CompressionLevel\)

Asynchronously writes the given <code class="paramref">tag</code> to a file at the specified <code class="paramref">path</code>.

```csharp
public static Task WriteAsync(string path, ListTag tag, FormatOptions options, CompressionType type = CompressionType.GZip, CompressionLevel level = CompressionLevel.Fastest)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

The path to the file to be written to.

`tag` [ListTag](Void.Minecraft.Nbt.ListTag.md)

The top-level <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance to be serialized.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

`type` [CompressionType](Void.Minecraft.Nbt.CompressionType.md)

A flag indicating the type of compression to use.

`level` [CompressionLevel](https://learn.microsoft.com/dotnet/api/system.io.compression.compressionlevel)

Indicates a compression strategy to be used, if any.

#### Returns

 [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)

