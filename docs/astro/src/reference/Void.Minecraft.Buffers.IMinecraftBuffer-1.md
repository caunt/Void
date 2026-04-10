# <a id="Void_Minecraft_Buffers_IMinecraftBuffer_1"></a> Interface IMinecraftBuffer<TBuffer\>

Namespace: [Void.Minecraft.Buffers](Void.Minecraft.Buffers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftBuffer<TBuffer> : IReadMinecraftBuffer, IWriteMinecraftBuffer, ICommonMinecraftBuffer where TBuffer : struct, allows ref struct
```

#### Type Parameters

`TBuffer` 

#### Implements

[IReadMinecraftBuffer](Void.Minecraft.Buffers.IReadMinecraftBuffer.md), 
[IWriteMinecraftBuffer](Void.Minecraft.Buffers.IWriteMinecraftBuffer.md), 
[ICommonMinecraftBuffer](Void.Minecraft.Buffers.ICommonMinecraftBuffer.md)

## Methods

### <a id="Void_Minecraft_Buffers_IMinecraftBuffer_1_Slice_System_Int32_System_Int32_"></a> Slice\(int, int\)

Extracts a portion of a buffer starting from a specified position for a given length.

```csharp
TBuffer Slice(int position, int length)
```

#### Parameters

`position` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Indicates the starting point from which the extraction begins.

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Specifies the number of elements to include in the extracted portion.

#### Returns

 TBuffer

Returns a new buffer containing the specified slice of the original buffer.

