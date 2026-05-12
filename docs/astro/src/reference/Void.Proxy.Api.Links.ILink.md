# <a id="Void_Proxy_Api_Links_ILink"></a> Interface ILink

Namespace: [Void.Proxy.Api.Links](Void.Proxy.Api.Links.md)  
Assembly: Void.Proxy.Api.dll  

Represents an active bidirectional forwarding session between a player and a backend Minecraft server.

```csharp
public interface ILink : IEventListener, IAsyncDisposable
```

#### Implements

[IEventListener](Void.Proxy.Api.Events.IEventListener.md), 
[IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

#### Extension Methods

[LinkExtensions.SendPacketAsync<T\>\(ILink, CancellationToken\)](Void.Minecraft.Links.Extensions.LinkExtensions.md\#Void\_Minecraft\_Links\_Extensions\_LinkExtensions\_SendPacketAsync\_\_1\_Void\_Proxy\_Api\_Links\_ILink\_System\_Threading\_CancellationToken\_), 
[LinkExtensions.SendPacketAsync<T\>\(ILink, T, CancellationToken\)](Void.Minecraft.Links.Extensions.LinkExtensions.md\#Void\_Minecraft\_Links\_Extensions\_LinkExtensions\_SendPacketAsync\_\_1\_Void\_Proxy\_Api\_Links\_ILink\_\_\_0\_System\_Threading\_CancellationToken\_), 
[LinkExtensions.SendPacketAsync<T\>\(ILink, Side, T, CancellationToken\)](Void.Minecraft.Links.Extensions.LinkExtensions.md\#Void\_Minecraft\_Links\_Extensions\_LinkExtensions\_SendPacketAsync\_\_1\_Void\_Proxy\_Api\_Links\_ILink\_Void\_Proxy\_Api\_Network\_Side\_\_\_0\_System\_Threading\_CancellationToken\_)

## Remarks

A link owns two <xref href="Void.Proxy.Api.Network.Channels.INetworkChannel" data-throw-if-not-resolved="false"></xref> instances — one toward the player and one toward the server —
and drives two concurrent forwarding loops that relay <xref href="Void.Proxy.Api.Network.Messages.INetworkMessage" data-throw-if-not-resolved="false"></xref>
objects between them. Each inbound message is surfaced through the event system before being written to the
opposite channel, allowing plugins to inspect or cancel individual packets.
The link stops naturally when either side closes its connection, and raises
<xref href="Void.Proxy.Api.Events.Links.LinkStoppingEvent" data-throw-if-not-resolved="false"></xref> followed by
<xref href="Void.Proxy.Api.Events.Links.LinkStoppedEvent" data-throw-if-not-resolved="false"></xref> with the appropriate <xref href="Void.Proxy.Api.Links.LinkStopReason" data-throw-if-not-resolved="false"></xref>.

## Properties

### <a id="Void_Proxy_Api_Links_ILink_Channels"></a> Channels

Gets an enumerable containing both <xref href="Void.Proxy.Api.Links.ILink.PlayerChannel" data-throw-if-not-resolved="false"></xref> and <xref href="Void.Proxy.Api.Links.ILink.ServerChannel" data-throw-if-not-resolved="false"></xref>.

```csharp
IEnumerable<INetworkChannel> Channels { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)\>

### <a id="Void_Proxy_Api_Links_ILink_IsAlive"></a> IsAlive

Gets a value indicating whether both forwarding tasks are still running.

```csharp
bool IsAlive { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Links_ILink_Player"></a> Player

Gets the player whose client connection is being proxied through this link.

```csharp
IPlayer Player { get; }
```

#### Property Value

 [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

### <a id="Void_Proxy_Api_Links_ILink_PlayerChannel"></a> PlayerChannel

Gets the network channel that faces the player's client.

```csharp
INetworkChannel PlayerChannel { get; }
```

#### Property Value

 [INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)

### <a id="Void_Proxy_Api_Links_ILink_Server"></a> Server

Gets the backend server that the player is currently connected to through this link.

```csharp
IServer Server { get; }
```

#### Property Value

 [IServer](Void.Proxy.Api.Servers.IServer.md)

### <a id="Void_Proxy_Api_Links_ILink_ServerChannel"></a> ServerChannel

Gets the network channel that faces the backend server.

```csharp
INetworkChannel ServerChannel { get; }
```

#### Property Value

 [INetworkChannel](Void.Proxy.Api.Network.Channels.INetworkChannel.md)

## Methods

### <a id="Void_Proxy_Api_Links_ILink_StartAsync_System_Threading_CancellationToken_"></a> StartAsync\(CancellationToken\)

Starts the bidirectional forwarding loops that relay network messages between the player and the server.

```csharp
ValueTask StartAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token that can be used to cancel the start operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

#### Exceptions

 [InvalidOperationException](https://learn.microsoft.com/dotnet/api/system.invalidoperationexception)

Thrown when the link has already been started.

### <a id="Void_Proxy_Api_Links_ILink_StopAsync_System_Threading_CancellationToken_"></a> StopAsync\(CancellationToken\)

Requests an orderly shutdown of both forwarding loops and waits for them to drain.

```csharp
ValueTask StopAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token that can be used to cancel the stop operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

#### Exceptions

 [InvalidOperationException](https://learn.microsoft.com/dotnet/api/system.invalidoperationexception)

Thrown when the link has not been started yet.

