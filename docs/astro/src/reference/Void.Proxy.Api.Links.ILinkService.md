# <a id="Void_Proxy_Api_Links_ILinkService"></a> Interface ILinkService

Namespace: [Void.Proxy.Api.Links](Void.Proxy.Api.Links.md)  
Assembly: Void.Proxy.Api.dll  

Manages the lifecycle of <xref href="Void.Proxy.Api.Links.ILink" data-throw-if-not-resolved="false"></xref> instances that connect players to backend servers,
and provides methods to establish or look up player-to-server connections.

```csharp
public interface ILinkService
```

## Properties

### <a id="Void_Proxy_Api_Links_ILinkService_All"></a> All

Gets all currently active links — links that have completed authentication and are
actively forwarding traffic between a player and a backend server.

```csharp
IReadOnlyList<ILink> All { get; }
```

#### Property Value

 [IReadOnlyList](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist\-1)<[ILink](Void.Proxy.Api.Links.ILink.md)\>

## Methods

### <a id="Void_Proxy_Api_Links_ILinkService_ConnectAsync_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Servers_IServer_System_Threading_CancellationToken_"></a> ConnectAsync\(IPlayer, IServer, CancellationToken\)

Connects the specified player to the given backend server.
If the target server is unreachable, the player is redirected back to their previous server.
If the previous server is also unreachable, the player is kicked with an explanatory message.

```csharp
ValueTask<ConnectionResult> ConnectAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The player to connect.

`server` [IServer](Void.Proxy.Api.Servers.IServer.md)

The target backend server.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[ConnectionResult](Void.Proxy.Api.Links.ConnectionResult.md)\>

<xref href="Void.Proxy.Api.Links.ConnectionResult.Connected" data-throw-if-not-resolved="false"></xref> if the player connected to the target or the previous server;
<xref href="Void.Proxy.Api.Links.ConnectionResult.NotConnected" data-throw-if-not-resolved="false"></xref> if neither the target nor the previous server could be reached.

### <a id="Void_Proxy_Api_Links_ILinkService_ConnectPlayerAnywhereAsync_Void_Proxy_Api_Players_IPlayer_System_Threading_CancellationToken_"></a> ConnectPlayerAnywhereAsync\(IPlayer, CancellationToken\)

Connects the specified player to the first available backend server by firing a
<xref href="Void.Proxy.Api.Events.Player.PlayerSearchServerEvent" data-throw-if-not-resolved="false"></xref> to let plugins nominate a preferred server.
If no plugin selects a server, every configured server is tried in order until one accepts the player.

```csharp
ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, CancellationToken cancellationToken = default)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The player to connect.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[ConnectionResult](Void.Proxy.Api.Links.ConnectionResult.md)\>

<xref href="Void.Proxy.Api.Links.ConnectionResult.Connected" data-throw-if-not-resolved="false"></xref> if a server was found and the player authenticated successfully;
<xref href="Void.Proxy.Api.Links.ConnectionResult.NotConnected" data-throw-if-not-resolved="false"></xref> if no available server accepted the player.

### <a id="Void_Proxy_Api_Links_ILinkService_ConnectPlayerAnywhereAsync_Void_Proxy_Api_Players_IPlayer_System_Collections_Generic_IEnumerable_Void_Proxy_Api_Servers_IServer__System_Threading_CancellationToken_"></a> ConnectPlayerAnywhereAsync\(IPlayer, IEnumerable<IServer\>, CancellationToken\)

Connects the specified player to the first available backend server that is not in
<code class="paramref">ignoredServers</code> by firing a <xref href="Void.Proxy.Api.Events.Player.PlayerSearchServerEvent" data-throw-if-not-resolved="false"></xref>
to let plugins nominate a preferred server.
If no plugin selects a server, every configured server not in the ignore list is tried in order.

```csharp
ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, IEnumerable<IServer> ignoredServers, CancellationToken cancellationToken = default)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The player to connect.

`ignoredServers` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[IServer](Void.Proxy.Api.Servers.IServer.md)\>

Servers that must not be considered during this connection attempt.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[ConnectionResult](Void.Proxy.Api.Links.ConnectionResult.md)\>

<xref href="Void.Proxy.Api.Links.ConnectionResult.Connected" data-throw-if-not-resolved="false"></xref> if a server was found and the player authenticated successfully;
<xref href="Void.Proxy.Api.Links.ConnectionResult.NotConnected" data-throw-if-not-resolved="false"></xref> if no available server accepted the player.

### <a id="Void_Proxy_Api_Links_ILinkService_HasLink_Void_Proxy_Api_Players_IPlayer_"></a> HasLink\(IPlayer\)

Determines whether the specified player has an active link to a backend server.

```csharp
bool HasLink(IPlayer player)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The player to check.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the player has an active link; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Proxy_Api_Links_ILinkService_TryGetLink_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Links_ILink__"></a> TryGetLink\(IPlayer, out ILink?\)

Attempts to retrieve the active link for the specified player.
A link is considered active once authentication has completed and traffic forwarding has started.

```csharp
bool TryGetLink(IPlayer player, out ILink? link)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The player whose active link to look up.

`link` [ILink](Void.Proxy.Api.Links.ILink.md)?

When this method returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>, contains the player's active link;
otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if an active link was found; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Proxy_Api_Links_ILinkService_TryGetWeakLink_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Links_ILink__"></a> TryGetWeakLink\(IPlayer, out ILink?\)

Attempts to retrieve a link for the specified player that is currently in the authentication phase
and has not yet been promoted to an active link.
A weak link is created at the start of the connection attempt and exists until authentication
either succeeds — at which point it also becomes an active link — or fails.

```csharp
bool TryGetWeakLink(IPlayer player, out ILink? link)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The player whose pending link to look up.

`link` [ILink](Void.Proxy.Api.Links.ILink.md)?

When this method returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>, contains the player's pending link;
otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if a pending link was found; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

