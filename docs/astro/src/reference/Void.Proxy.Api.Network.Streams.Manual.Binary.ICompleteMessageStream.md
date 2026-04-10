# <a id="Void_Proxy_Api_Network_Streams_Manual_Binary_ICompleteMessageStream"></a> Interface ICompleteMessageStream

Namespace: [Void.Proxy.Api.Network.Streams.Manual.Binary](Void.Proxy.Api.Network.Streams.Manual.Binary.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface ICompleteMessageStream : IMessageStream, IMessageStreamBase, IDisposable, IAsyncDisposable
```

#### Implements

[IMessageStream](Void.Proxy.Api.Network.Streams.IMessageStream.md), 
[IMessageStreamBase](Void.Proxy.Api.Network.Streams.IMessageStreamBase.md), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), 
[IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

## Methods

### <a id="Void_Proxy_Api_Network_Streams_Manual_Binary_ICompleteMessageStream_ReadMessage"></a> ReadMessage\(\)

```csharp
ICompleteBinaryMessage ReadMessage()
```

#### Returns

 [ICompleteBinaryMessage](Void.Proxy.Api.Network.Messages.ICompleteBinaryMessage.md)

### <a id="Void_Proxy_Api_Network_Streams_Manual_Binary_ICompleteMessageStream_ReadMessageAsync_System_Threading_CancellationToken_"></a> ReadMessageAsync\(CancellationToken\)

```csharp
ValueTask<ICompleteBinaryMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[ICompleteBinaryMessage](Void.Proxy.Api.Network.Messages.ICompleteBinaryMessage.md)\>

### <a id="Void_Proxy_Api_Network_Streams_Manual_Binary_ICompleteMessageStream_WriteMessage_Void_Proxy_Api_Network_Messages_ICompleteBinaryMessage_"></a> WriteMessage\(ICompleteBinaryMessage\)

```csharp
void WriteMessage(ICompleteBinaryMessage message)
```

#### Parameters

`message` [ICompleteBinaryMessage](Void.Proxy.Api.Network.Messages.ICompleteBinaryMessage.md)

### <a id="Void_Proxy_Api_Network_Streams_Manual_Binary_ICompleteMessageStream_WriteMessageAsync_Void_Proxy_Api_Network_Messages_ICompleteBinaryMessage_System_Threading_CancellationToken_"></a> WriteMessageAsync\(ICompleteBinaryMessage, CancellationToken\)

```csharp
ValueTask WriteMessageAsync(ICompleteBinaryMessage message, CancellationToken cancellationToken = default)
```

#### Parameters

`message` [ICompleteBinaryMessage](Void.Proxy.Api.Network.Messages.ICompleteBinaryMessage.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

