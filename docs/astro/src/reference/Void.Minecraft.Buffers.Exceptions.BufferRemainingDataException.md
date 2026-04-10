# <a id="Void_Minecraft_Buffers_Exceptions_BufferRemainingDataException"></a> Class BufferRemainingDataException

Namespace: [Void.Minecraft.Buffers.Exceptions](Void.Minecraft.Buffers.Exceptions.md)  
Assembly: Void.Minecraft.dll  

Represents an error that occurs when a buffer operation completes with unread bytes remaining.

```csharp
public class BufferRemainingDataException : Exception, ISerializable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Exception](https://learn.microsoft.com/dotnet/api/system.exception) ← 
[BufferRemainingDataException](Void.Minecraft.Buffers.Exceptions.BufferRemainingDataException.md)

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

## Remarks

This exception is thrown by <xref href="Void.Minecraft.Buffers.BufferSpan.Dispose" data-throw-if-not-resolved="false"></xref> when the span is disposed before all bytes are consumed.

## Constructors

### <a id="Void_Minecraft_Buffers_Exceptions_BufferRemainingDataException__ctor_System_Int64_System_Int64_"></a> BufferRemainingDataException\(long, long\)

Represents an error that occurs when a buffer operation completes with unread bytes remaining.

```csharp
public BufferRemainingDataException(long bufferSize, long bufferPosition)
```

#### Parameters

`bufferSize` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The total length of the buffer that was being consumed.

`bufferPosition` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The final read position reached when the operation ended.

#### Remarks

This exception is thrown by <xref href="Void.Minecraft.Buffers.BufferSpan.Dispose" data-throw-if-not-resolved="false"></xref> when the span is disposed before all bytes are consumed.

## Properties

### <a id="Void_Minecraft_Buffers_Exceptions_BufferRemainingDataException_BufferPosition"></a> BufferPosition

Gets the read position at the time the exception was created.

```csharp
public long BufferPosition { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Buffers_Exceptions_BufferRemainingDataException_BufferSize"></a> BufferSize

Gets the total size of the buffer.

```csharp
public long BufferSize { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Buffers_Exceptions_BufferRemainingDataException_RemainingData"></a> RemainingData

Gets the number of unread bytes remaining in the buffer.

```csharp
public long RemainingData { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

