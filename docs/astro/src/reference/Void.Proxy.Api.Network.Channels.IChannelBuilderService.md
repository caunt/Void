# <a id="Void_Proxy_Api_Network_Channels_IChannelBuilderService"></a> Interface IChannelBuilderService

Namespace: [Void.Proxy.Api.Network.Channels](Void.Proxy.Api.Network.Channels.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IChannelBuilderService
```

## Properties

### <a id="Void_Proxy_Api_Network_Channels_IChannelBuilderService_IsFallbackBuilder"></a> IsFallbackBuilder

```csharp
bool IsFallbackBuilder { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Methods

### <a id="Void_Proxy_Api_Network_Channels_IChannelBuilderService_BuildPlayerChannelAsync_Void_Proxy_Api_Players_IPlayer_System_Threading_CancellationToken_"></a> BuildPlayerChannelAsync\(IPlayer, CancellationToken\)

```csharp
ValueTask<INetworkChannel> BuildPlayerChannelAsync(IPlayer player, CancellationToken cancellationToken = default)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)\>

### <a id="Void_Proxy_Api_Network_Channels_IChannelBuilderService_BuildServerChannelAsync_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Servers_IServer_System_Threading_CancellationToken_"></a> BuildServerChannelAsync\(IPlayer, IServer, CancellationToken\)

```csharp
ValueTask<INetworkChannel> BuildServerChannelAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`server` [IServer](Void.Proxy.Api.Servers.IServer.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)\>

### <a id="Void_Proxy_Api_Network_Channels_IChannelBuilderService_SearchChannelBuilderAsync_Void_Proxy_Api_Players_IPlayer_System_Threading_CancellationToken_"></a> SearchChannelBuilderAsync\(IPlayer, CancellationToken\)

```csharp
ValueTask SearchChannelBuilderAsync(IPlayer player, CancellationToken cancellationToken = default)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

