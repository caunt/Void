# <a id="Void_Minecraft_Buffers_ICommonMinecraftBuffer"></a> Interface ICommonMinecraftBuffer

Namespace: [Void.Minecraft.Buffers](Void.Minecraft.Buffers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface ICommonMinecraftBuffer
```

## Properties

### <a id="Void_Minecraft_Buffers_ICommonMinecraftBuffer_Length"></a> Length

Returns the length of the source as an integer. It is a read-only property.

```csharp
int Length { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Buffers_ICommonMinecraftBuffer_Position"></a> Position

Gets or sets the position within a buffer. The position must be between 0 and the length of the source,
otherwise an exception is thrown.

```csharp
int Position { get; set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Void_Minecraft_Buffers_ICommonMinecraftBuffer_Access_System_Int32_"></a> Access\(int\)

Retrieves a span of bytes from the underlying buffer starting at the current position
for the specified length. This method does not modify the current position of the buffer.

```csharp
Span<byte> Access(int length)
```

#### Parameters

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The number of bytes to retrieve from the current position. Must be non-negative.

#### Returns

 [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A <xref href="System.Span%601" data-throw-if-not-resolved="false"></xref> representing the specified number of bytes from the current position.

### <a id="Void_Minecraft_Buffers_ICommonMinecraftBuffer_Access_System_Int32_System_Int32_"></a> Access\(int, int\)

Retrieves a span of bytes from the underlying buffer starting at the specified position
for the given length.

```csharp
Span<byte> Access(int position, int length)
```

#### Parameters

`position` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The starting position in the buffer from which to retrieve the byte span. Must be a non-negative value.

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The number of bytes to include in the retrieved span. Must be non-negative.

#### Returns

 [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A <xref href="System.Span%601" data-throw-if-not-resolved="false"></xref> containing the specified range of bytes.

### <a id="Void_Minecraft_Buffers_ICommonMinecraftBuffer_Seek_System_Int32_System_IO_SeekOrigin_"></a> Seek\(int, SeekOrigin\)

Advances or sets the current position in the buffer relative to the specified origin.

```csharp
void Seek(int offset, SeekOrigin origin = SeekOrigin.Current)
```

#### Parameters

`offset` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The byte offset relative to the <code class="paramref">origin</code>.

`origin` [SeekOrigin](https://learn.microsoft.com/dotnet/api/system.io.seekorigin)

The point of reference used to obtain the new position.

