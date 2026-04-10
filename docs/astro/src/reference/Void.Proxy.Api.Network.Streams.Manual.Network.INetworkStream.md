# <a id="Void_Proxy_Api_Network_Streams_Manual_Network_INetworkStream"></a> Interface INetworkStream

Namespace: [Void.Proxy.Api.Network.Streams.Manual.Network](Void.Proxy.Api.Network.Streams.Manual.Network.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface INetworkStream : IManualStream, IMessageStreamBase, IDisposable, IAsyncDisposable
```

#### Implements

[IManualStream](Void.Proxy.Api.Network.Streams.Manual.IManualStream.md), 
[IMessageStreamBase](Void.Proxy.Api.Network.Streams.IMessageStreamBase.md), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), 
[IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

## Properties

### <a id="Void_Proxy_Api_Network_Streams_Manual_Network_INetworkStream_BaseStream"></a> BaseStream

```csharp
NetworkStream BaseStream { get; }
```

#### Property Value

 [NetworkStream](https://learn.microsoft.com/dotnet/api/system.net.sockets.networkstream)

## Methods

### <a id="Void_Proxy_Api_Network_Streams_Manual_Network_INetworkStream_PrependBuffer_System_Memory_System_Byte__"></a> PrependBuffer\(Memory<byte\>\)

```csharp
void PrependBuffer(Memory<byte> buffer)
```

#### Parameters

`buffer` [Memory](https://learn.microsoft.com/dotnet/api/system.memory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

