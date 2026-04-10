# <a id="Void_Proxy_Api_Players_IPlayerService"></a> Interface IPlayerService

Namespace: [Void.Proxy.Api.Players](Void.Proxy.Api.Players.md)  
Assembly: Void.Proxy.Api.dll  

Provides operations for managing <xref href="Void.Proxy.Api.Players.IPlayer" data-throw-if-not-resolved="false"></xref> instances.

```csharp
public interface IPlayerService
```

#### Extension Methods

[PlayerServiceExtensions.TryGetByName\(IPlayerService, string, out IPlayer?\)](Void.Minecraft.Players.Extensions.PlayerServiceExtensions.md\#Void\_Minecraft\_Players\_Extensions\_PlayerServiceExtensions\_TryGetByName\_Void\_Proxy\_Api\_Players\_IPlayerService\_System\_String\_Void\_Proxy\_Api\_Players\_IPlayer\_\_)

## Properties

### <a id="Void_Proxy_Api_Players_IPlayerService_All"></a> All

Gets a collection containing all active players.

```csharp
IEnumerable<IPlayer> All { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[IPlayer](Void.Proxy.Api.Players.IPlayer.md)\>

## Methods

### <a id="Void_Proxy_Api_Players_IPlayerService_AcceptPlayerAsync_System_Net_Sockets_TcpClient_System_Threading_CancellationToken_"></a> AcceptPlayerAsync\(TcpClient, CancellationToken\)

Accepts a new connection and creates a player instance from the provided <xref href="System.Net.Sockets.TcpClient" data-throw-if-not-resolved="false"></xref>.

```csharp
ValueTask AcceptPlayerAsync(TcpClient client, CancellationToken cancellationToken = default)
```

#### Parameters

`client` [TcpClient](https://learn.microsoft.com/dotnet/api/system.net.sockets.tcpclient)

The client representing the player connection.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

Token used to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_Players_IPlayerService_ForEach_System_Action_Void_Proxy_Api_Players_IPlayer__"></a> ForEach\(Action<IPlayer\>\)

Executes the specified <code class="paramref">action</code> for each player.

```csharp
void ForEach(Action<IPlayer> action)
```

#### Parameters

`action` [Action](https://learn.microsoft.com/dotnet/api/system.action\-1)<[IPlayer](Void.Proxy.Api.Players.IPlayer.md)\>

The action to perform for each player.

### <a id="Void_Proxy_Api_Players_IPlayerService_ForEachAsync_System_Func_Void_Proxy_Api_Players_IPlayer_System_Threading_CancellationToken_System_Threading_Tasks_ValueTask__System_Threading_CancellationToken_"></a> ForEachAsync\(Func<IPlayer, CancellationToken, ValueTask\>, CancellationToken\)

Executes the specified asynchronous <code class="paramref">action</code> for each player.

```csharp
ValueTask ForEachAsync(Func<IPlayer, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
```

#### Parameters

`action` [Func](https://learn.microsoft.com/dotnet/api/system.func\-3)<[IPlayer](Void.Proxy.Api.Players.IPlayer.md), [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken), [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)\>

The asynchronous action to execute for each player.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

Token used to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_Players_IPlayerService_KickPlayerAsync_Void_Proxy_Api_Players_IPlayer_System_String_System_Threading_CancellationToken_"></a> KickPlayerAsync\(IPlayer, string?, CancellationToken\)

Kicks a player with an optional message.

```csharp
ValueTask KickPlayerAsync(IPlayer player, string? text = null, CancellationToken cancellationToken = default)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The player to kick.

`text` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The optional kick message.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

Token used to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_Players_IPlayerService_KickPlayerAsync_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Events_Player_PlayerKickEvent_System_Threading_CancellationToken_"></a> KickPlayerAsync\(IPlayer, PlayerKickEvent, CancellationToken\)

Kicks a player using a <xref href="Void.Proxy.Api.Events.Player.PlayerKickEvent" data-throw-if-not-resolved="false"></xref> instance.

```csharp
ValueTask KickPlayerAsync(IPlayer player, PlayerKickEvent playerKickEvent, CancellationToken cancellationToken = default)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The player to kick.

`playerKickEvent` [PlayerKickEvent](Void.Proxy.Api.Events.Player.PlayerKickEvent.md)

The event describing the kick.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

Token used to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_Players_IPlayerService_UpgradeAsync_Void_Proxy_Api_Players_IPlayer_Void_Proxy_Api_Players_IPlayer_System_Threading_CancellationToken_"></a> UpgradeAsync\(IPlayer, IPlayer, CancellationToken\)

Upgrades the specified <code class="paramref">player</code> to <code class="paramref">upgradedPlayer</code>.

```csharp
ValueTask UpgradeAsync(IPlayer player, IPlayer upgradedPlayer, CancellationToken cancellationToken)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The current player instance.

`upgradedPlayer` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The upgraded player instance.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

Token used to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

