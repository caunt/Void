# <a id="Void_Proxy_Api_Network_Channels_ChannelBuilder"></a> Delegate ChannelBuilder

Namespace: [Void.Proxy.Api.Network.Channels](Void.Proxy.Api.Network.Channels.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public delegate ValueTask<INetworkChannel> ChannelBuilder(IPlayer player, Side side, NetworkStream networkStream, CancellationToken cancellationToken)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`side` [Side](Void.Proxy.Api.Network.Side.md)

`networkStream` [NetworkStream](https://learn.microsoft.com/dotnet/api/system.net.sockets.networkstream)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)\>

