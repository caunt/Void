# <a id="Void_Proxy_Api_Network_Streams_Manual_Binary_IBufferedMessageStream"></a> Interface IBufferedMessageStream

Namespace: [Void.Proxy.Api.Network.Streams.Manual.Binary](Void.Proxy.Api.Network.Streams.Manual.Binary.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IBufferedMessageStream : IManualStream, IMessageStream, IMessageStreamBase, IDisposable, IAsyncDisposable
```

#### Implements

[IManualStream](Void.Proxy.Api.Network.Streams.Manual.IManualStream.md), 
[IMessageStream](Void.Proxy.Api.Network.Streams.IMessageStream.md), 
[IMessageStreamBase](Void.Proxy.Api.Network.Streams.IMessageStreamBase.md), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), 
[IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

## Methods

### <a id="Void_Proxy_Api_Network_Streams_Manual_Binary_IBufferedMessageStream_ReadAsMessage_System_Int32_"></a> ReadAsMessage\(int\)

```csharp
IBufferedBinaryMessage ReadAsMessage(int maxSize = 2048)
```

#### Parameters

`maxSize` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [IBufferedBinaryMessage](Void.Proxy.Api.Network.Messages.IBufferedBinaryMessage.md)

### <a id="Void_Proxy_Api_Network_Streams_Manual_Binary_IBufferedMessageStream_ReadAsMessageAsync_System_Int32_System_Threading_CancellationToken_"></a> ReadAsMessageAsync\(int, CancellationToken\)

```csharp
ValueTask<IBufferedBinaryMessage> ReadAsMessageAsync(int maxSize = 2048, CancellationToken cancellationToken = default)
```

#### Parameters

`maxSize` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[IBufferedBinaryMessage](Void.Proxy.Api.Network.Messages.IBufferedBinaryMessage.md)\>

### <a id="Void_Proxy_Api_Network_Streams_Manual_Binary_IBufferedMessageStream_WriteAsMessage_Void_Proxy_Api_Network_Messages_IBufferedBinaryMessage_"></a> WriteAsMessage\(IBufferedBinaryMessage\)

```csharp
void WriteAsMessage(IBufferedBinaryMessage message)
```

#### Parameters

`message` [IBufferedBinaryMessage](Void.Proxy.Api.Network.Messages.IBufferedBinaryMessage.md)

### <a id="Void_Proxy_Api_Network_Streams_Manual_Binary_IBufferedMessageStream_WriteAsMessageAsync_Void_Proxy_Api_Network_Messages_IBufferedBinaryMessage_System_Threading_CancellationToken_"></a> WriteAsMessageAsync\(IBufferedBinaryMessage, CancellationToken\)

```csharp
ValueTask WriteAsMessageAsync(IBufferedBinaryMessage message, CancellationToken cancellationToken = default)
```

#### Parameters

`message` [IBufferedBinaryMessage](Void.Proxy.Api.Network.Messages.IBufferedBinaryMessage.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

