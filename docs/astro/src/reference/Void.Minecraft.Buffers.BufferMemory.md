# <a id="Void_Minecraft_Buffers_BufferMemory"></a> Struct BufferMemory

Namespace: [Void.Minecraft.Buffers](Void.Minecraft.Buffers.md)  
Assembly: Void.Minecraft.dll  

Represents a memory buffer that can be sliced into smaller segments. Provides a way to access a specific range of
bytes within the buffer.

```csharp
public readonly struct BufferMemory
```

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

#### Extension Methods

[StructExtensions.IsDefault<BufferMemory\>\(BufferMemory\)](Void.Proxy.Api.Extensions.StructExtensions.md\#Void\_Proxy\_Api\_Extensions\_StructExtensions\_IsDefault\_\_1\_\_\_0\_)

## Constructors

### <a id="Void_Minecraft_Buffers_BufferMemory__ctor_System_Memory_System_Byte__"></a> BufferMemory\(Memory<byte\>\)

Represents a memory buffer that can be sliced into smaller segments. Provides a way to access a specific range of
bytes within the buffer.

```csharp
public BufferMemory(Memory<byte> source)
```

#### Parameters

`source` [Memory](https://learn.microsoft.com/dotnet/api/system.memory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The input memory provides the raw byte data to be manipulated and accessed.

## Properties

### <a id="Void_Minecraft_Buffers_BufferMemory_Span"></a> Span

Returns a new BufferSpan instance based on the source's Span. It provides a read-only view of the underlying
buffer.

```csharp
public BufferSpan Span { get; }
```

#### Property Value

 [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

## Methods

### <a id="Void_Minecraft_Buffers_BufferMemory_Slice_System_Int32_System_Int32_"></a> Slice\(int, int\)

Creates a new BufferMemory instance that represents a portion of the source buffer.

```csharp
public BufferMemory Slice(int position, int length)
```

#### Parameters

`position` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Specifies the starting index of the slice within the source buffer.

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Indicates the number of elements to include in the slice from the starting index.

#### Returns

 [BufferMemory](Void.Minecraft.Buffers.BufferMemory.md)

Returns a BufferMemory object containing the specified slice of the source buffer.

#### Exceptions

 [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception)

Thrown when the starting index or length is negative.

 [EndOfBufferException](Void.Minecraft.Buffers.Exceptions.EndOfBufferException.md)

Thrown when the sum of the starting index and length exceeds the source buffer's length.

