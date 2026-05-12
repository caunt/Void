# <a id="Void_Minecraft_Buffers_Exceptions_EndOfBufferException"></a> Class EndOfBufferException

Namespace: [Void.Minecraft.Buffers.Exceptions](Void.Minecraft.Buffers.Exceptions.md)  
Assembly: Void.Minecraft.dll  

Indicates an attempt to access data beyond the available buffer size.

```csharp
public class EndOfBufferException : Exception, ISerializable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Exception](https://learn.microsoft.com/dotnet/api/system.exception) ← 
[EndOfBufferException](Void.Minecraft.Buffers.Exceptions.EndOfBufferException.md)

#### Implements

[ISerializable](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)

#### Inherited Members

[Exception.GetBaseException\(\)](https://learn.microsoft.com/dotnet/api/system.exception.getbaseexception), 
[Exception.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.exception.gettype), 
[Exception.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.exception.tostring), 
[Exception.Data](https://learn.microsoft.com/dotnet/api/system.exception.data), 
[Exception.HelpLink](https://learn.microsoft.com/dotnet/api/system.exception.helplink), 
[Exception.HResult](https://learn.microsoft.com/dotnet/api/system.exception.hresult), 
[Exception.InnerException](https://learn.microsoft.com/dotnet/api/system.exception.innerexception), 
[Exception.Message](https://learn.microsoft.com/dotnet/api/system.exception.message), 
[Exception.Source](https://learn.microsoft.com/dotnet/api/system.exception.source), 
[Exception.StackTrace](https://learn.microsoft.com/dotnet/api/system.exception.stacktrace), 
[Exception.TargetSite](https://learn.microsoft.com/dotnet/api/system.exception.targetsite), 
[Exception.SerializeObjectState](https://learn.microsoft.com/dotnet/api/system.exception.serializeobjectstate), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Buffers_Exceptions_EndOfBufferException__ctor_System_Int64_System_Int64_System_Int64_"></a> EndOfBufferException\(long, long, long\)

Indicates an attempt to access data beyond the available buffer size.

```csharp
public EndOfBufferException(long bufferSize, long bufferPosition, long requestedLength)
```

#### Parameters

`bufferSize` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The total size of the buffer in bytes.

`bufferPosition` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The current read position within the buffer at the time of the failed access.

`requestedLength` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The number of bytes that were requested but could not be satisfied.

### <a id="Void_Minecraft_Buffers_Exceptions_EndOfBufferException__ctor_System_Int32_System_Int32_System_Int32_"></a> EndOfBufferException\(int, int, int\)

```csharp
public EndOfBufferException(int bufferSize, int bufferPosition, int requestedLength)
```

#### Parameters

`bufferSize` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`bufferPosition` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`requestedLength` [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Buffers_Exceptions_EndOfBufferException__ctor_System_IntPtr_System_IntPtr_System_IntPtr_"></a> EndOfBufferException\(nint, nint, nint\)

```csharp
public EndOfBufferException(nint bufferSize, nint bufferPosition, nint requestedLength)
```

#### Parameters

`bufferSize` [nint](https://learn.microsoft.com/dotnet/api/system.intptr)

`bufferPosition` [nint](https://learn.microsoft.com/dotnet/api/system.intptr)

`requestedLength` [nint](https://learn.microsoft.com/dotnet/api/system.intptr)

### <a id="Void_Minecraft_Buffers_Exceptions_EndOfBufferException__ctor_System_Int16_System_Int16_System_Int16_"></a> EndOfBufferException\(short, short, short\)

```csharp
public EndOfBufferException(short bufferSize, short bufferPosition, short requestedLength)
```

#### Parameters

`bufferSize` [short](https://learn.microsoft.com/dotnet/api/system.int16)

`bufferPosition` [short](https://learn.microsoft.com/dotnet/api/system.int16)

`requestedLength` [short](https://learn.microsoft.com/dotnet/api/system.int16)

### <a id="Void_Minecraft_Buffers_Exceptions_EndOfBufferException__ctor_System_Byte_System_Byte_System_Byte_"></a> EndOfBufferException\(byte, byte, byte\)

```csharp
public EndOfBufferException(byte bufferSize, byte bufferPosition, byte requestedLength)
```

#### Parameters

`bufferSize` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

`bufferPosition` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

`requestedLength` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

## Properties

### <a id="Void_Minecraft_Buffers_Exceptions_EndOfBufferException_BufferPosition"></a> BufferPosition

```csharp
public long BufferPosition { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Buffers_Exceptions_EndOfBufferException_BufferSize"></a> BufferSize

```csharp
public long BufferSize { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Buffers_Exceptions_EndOfBufferException_RequestedLength"></a> RequestedLength

```csharp
public long RequestedLength { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

