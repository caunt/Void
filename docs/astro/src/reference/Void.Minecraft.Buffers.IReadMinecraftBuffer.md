# <a id="Void_Minecraft_Buffers_IReadMinecraftBuffer"></a> Interface IReadMinecraftBuffer

Namespace: [Void.Minecraft.Buffers](Void.Minecraft.Buffers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IReadMinecraftBuffer : ICommonMinecraftBuffer
```

#### Implements

[ICommonMinecraftBuffer](Void.Minecraft.Buffers.ICommonMinecraftBuffer.md)

## Properties

### <a id="Void_Minecraft_Buffers_IReadMinecraftBuffer_Remaining"></a> Remaining

Calculates the number of remaining elements in a source collection. It subtracts the current position from the
total length.

```csharp
int Remaining { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

