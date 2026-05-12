# <a id="Void_Proxy_Api_Network_Streams_Manual_IManualStream"></a> Interface IManualStream

Namespace: [Void.Proxy.Api.Network.Streams.Manual](Void.Proxy.Api.Network.Streams.Manual.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IManualStream
```

## Methods

### <a id="Void_Proxy_Api_Network_Streams_Manual_IManualStream_Read_System_Span_System_Byte__"></a> Read\(Span<byte\>\)

```csharp
int Read(Span<byte> span)
```

#### Parameters

`span` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Proxy_Api_Network_Streams_Manual_IManualStream_ReadAsync_System_Memory_System_Byte__System_Threading_CancellationToken_"></a> ReadAsync\(Memory<byte\>, CancellationToken\)

```csharp
ValueTask<int> ReadAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
```

#### Parameters

`memory` [Memory](https://learn.microsoft.com/dotnet/api/system.memory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

### <a id="Void_Proxy_Api_Network_Streams_Manual_IManualStream_ReadExactly_System_Span_System_Byte__"></a> ReadExactly\(Span<byte\>\)

```csharp
void ReadExactly(Span<byte> span)
```

#### Parameters

`span` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Proxy_Api_Network_Streams_Manual_IManualStream_ReadExactlyAsync_System_Memory_System_Byte__System_Threading_CancellationToken_"></a> ReadExactlyAsync\(Memory<byte\>, CancellationToken\)

```csharp
ValueTask ReadExactlyAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
```

#### Parameters

`memory` [Memory](https://learn.microsoft.com/dotnet/api/system.memory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_Network_Streams_Manual_IManualStream_Write_System_ReadOnlySpan_System_Byte__"></a> Write\(ReadOnlySpan<byte\>\)

```csharp
void Write(ReadOnlySpan<byte> span)
```

#### Parameters

`span` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Proxy_Api_Network_Streams_Manual_IManualStream_WriteAsync_System_ReadOnlyMemory_System_Byte__System_Threading_CancellationToken_"></a> WriteAsync\(ReadOnlyMemory<byte\>, CancellationToken\)

```csharp
ValueTask WriteAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
```

#### Parameters

`memory` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

