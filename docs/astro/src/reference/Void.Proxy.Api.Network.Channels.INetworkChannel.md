# <a id="Void_Proxy_Api_Network_Channels_INetworkChannel"></a> Interface INetworkChannel

Namespace: [Void.Proxy.Api.Network.Channels](Void.Proxy.Api.Network.Channels.md)  
Assembly: Void.Proxy.Api.dll  

Represents a network channel that manages a pipeline of <xref href="Void.Proxy.Api.Network.Streams.IMessageStream" data-throw-if-not-resolved="false"></xref> instances.

```csharp
public interface INetworkChannel : IDisposable, IAsyncDisposable
```

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), 
[IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

## Properties

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_CanRead"></a> CanRead

Gets a value indicating whether the channel can read messages.

```csharp
bool CanRead { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_CanWrite"></a> CanWrite

Gets a value indicating whether the channel can write messages.

```csharp
bool CanWrite { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_Head"></a> Head

Gets the first stream in the channel's pipeline.

```csharp
IMessageStreamBase Head { get; }
```

#### Property Value

 [IMessageStreamBase](Void.Proxy.Api.Network.Streams.IMessageStreamBase.md)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_IsAlive"></a> IsAlive

Gets a value indicating whether the channel is active.

```csharp
bool IsAlive { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_IsConfigured"></a> IsConfigured

Gets a value indicating whether the channel has been configured.

```csharp
bool IsConfigured { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_IsPaused"></a> IsPaused

Gets a value indicating whether the channel is currently paused.

```csharp
bool IsPaused { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_IsPausedRead"></a> IsPausedRead

Gets a value indicating whether read operations are currently paused.

```csharp
bool IsPausedRead { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_IsPausedWrite"></a> IsPausedWrite

Gets a value indicating whether write operations are currently paused.

```csharp
bool IsPausedWrite { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Methods

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_Add__1"></a> Add<T\>\(\)

Adds a new stream of type <code class="typeparamref">T</code> to the end of the pipeline.

```csharp
void Add<T>() where T : class, IMessageStream, new()
```

#### Type Parameters

`T` 

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_Add__1___0_"></a> Add<T\>\(T\)

Adds the specified <code class="paramref">stream</code> to the end of the pipeline.

```csharp
void Add<T>(T stream) where T : class, IMessageStream
```

#### Parameters

`stream` T

#### Type Parameters

`T` 

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_AddBefore__2"></a> AddBefore<TBefore, TValue\>\(\)

Inserts a new stream of type <code class="typeparamref">TValue</code> before <code class="typeparamref">TBefore</code>.

```csharp
void AddBefore<TBefore, TValue>() where TBefore : class, IMessageStream where TValue : class, IMessageStream, new()
```

#### Type Parameters

`TBefore` 

`TValue` 

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_AddBefore__2___1_"></a> AddBefore<TBefore, TValue\>\(TValue\)

Inserts the specified <code class="paramref">stream</code> before <code class="typeparamref">TBefore</code>.

```csharp
void AddBefore<TBefore, TValue>(TValue stream) where TBefore : class, IMessageStream where TValue : class, IMessageStream
```

#### Parameters

`stream` TValue

#### Type Parameters

`TBefore` 

`TValue` 

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_Close"></a> Close\(\)

Closes the channel and releases resources.

```csharp
void Close()
```

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_Flush"></a> Flush\(\)

Flushes any buffered data.

```csharp
void Flush()
```

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_FlushAsync_System_Threading_CancellationToken_"></a> FlushAsync\(CancellationToken\)

Asynchronously flushes any buffered data.

```csharp
ValueTask FlushAsync(CancellationToken cancellationToken = default)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_Get__1"></a> Get<T\>\(\)

Gets the stream of type <code class="typeparamref">T</code>.

```csharp
T Get<T>() where T : class, IMessageStreamBase
```

#### Returns

 T

#### Type Parameters

`T` 

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_Has__1"></a> Has<T\>\(\)

Determines whether a stream of type <code class="typeparamref">T</code> exists in the pipeline.

```csharp
bool Has<T>() where T : class, IMessageStreamBase
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`T` 

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_Pause_Void_Proxy_Api_Network_Operation_"></a> Pause\(Operation\)

Pauses channel operations.

```csharp
void Pause(Operation operation = Operation.Read)
```

#### Parameters

`operation` [Operation](Void.Proxy.Api.Network.Operation.md)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_PrependBuffer_System_Memory_System_Byte__"></a> PrependBuffer\(Memory<byte\>\)

Inserts the given <code class="paramref">memory</code> at the start of the buffer.

```csharp
void PrependBuffer(Memory<byte> memory)
```

#### Parameters

`memory` [Memory](https://learn.microsoft.com/dotnet/api/system.memory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_ReadMessageAsync_System_Threading_CancellationToken_"></a> ReadMessageAsync\(CancellationToken\)

Reads the next message from the channel.

```csharp
ValueTask<INetworkMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[INetworkMessage](Void.Proxy.Api.Network.Messages.INetworkMessage.md)\>

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_Remove__1"></a> Remove<T\>\(\)

Removes a stream of type <code class="typeparamref">T</code> from the pipeline.

```csharp
void Remove<T>() where T : class, IMessageStream
```

#### Type Parameters

`T` 

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_Remove__1___0_"></a> Remove<T\>\(T\)

Removes the specified <code class="paramref">stream</code> from the pipeline.

```csharp
void Remove<T>(T stream) where T : class, IMessageStream
```

#### Parameters

`stream` T

#### Type Parameters

`T` 

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_Resume_Void_Proxy_Api_Network_Operation_"></a> Resume\(Operation\)

Resumes channel operations.

```csharp
void Resume(Operation operation = Operation.Read)
```

#### Parameters

`operation` [Operation](Void.Proxy.Api.Network.Operation.md)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_TryGet__1___0__"></a> TryGet<T\>\(out T\)

Attempts to retrieve a stream of type <code class="typeparamref">T</code>.

```csharp
bool TryGet<T>(out T result) where T : class, IMessageStreamBase
```

#### Parameters

`result` T

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`T` 

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_TryPause_Void_Proxy_Api_Network_Operation_"></a> TryPause\(Operation\)

Attempts to pause channel operations.

```csharp
bool TryPause(Operation operation = Operation.Read)
```

#### Parameters

`operation` [Operation](Void.Proxy.Api.Network.Operation.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_TryResume_Void_Proxy_Api_Network_Operation_"></a> TryResume\(Operation\)

Attempts to resume channel operations.

```csharp
bool TryResume(Operation operation = Operation.Read)
```

#### Parameters

`operation` [Operation](Void.Proxy.Api.Network.Operation.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Network_Channels_INetworkChannel_WriteMessageAsync_Void_Proxy_Api_Network_Messages_INetworkMessage_System_Threading_CancellationToken_"></a> WriteMessageAsync\(INetworkMessage, CancellationToken\)

Writes the specified <code class="paramref">message</code> to the channel.

```csharp
ValueTask WriteMessageAsync(INetworkMessage message, CancellationToken cancellationToken = default)
```

#### Parameters

`message` [INetworkMessage](Void.Proxy.Api.Network.Messages.INetworkMessage.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

