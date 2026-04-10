# <a id="Void_Proxy_Api_IProxy"></a> Interface IProxy

Namespace: [Void.Proxy.Api](Void.Proxy.Api.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IProxy
```

## Properties

### <a id="Void_Proxy_Api_IProxy_Interface"></a> Interface

Gets the IP address of the network interface on which the connection listener is bound.

```csharp
IPAddress Interface { get; }
```

#### Property Value

 [IPAddress](https://learn.microsoft.com/dotnet/api/system.net.ipaddress)

### <a id="Void_Proxy_Api_IProxy_Port"></a> Port

Gets the network port number used for the connection listener.

```csharp
int Port { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Proxy_Api_IProxy_Status"></a> Status

Gets a value indicating the current status of the proxy.

```csharp
ProxyStatus Status { get; }
```

#### Property Value

 [ProxyStatus](Void.Proxy.Api.ProxyStatus.md)

## Methods

### <a id="Void_Proxy_Api_IProxy_PauseAcceptingConnections"></a> PauseAcceptingConnections\(\)

Pauses the acceptance of new incoming connections.

```csharp
void PauseAcceptingConnections()
```

#### Remarks

This method temporarily halts the ability to accept new connections. Existing connections
    remain unaffected and continue to operate normally. Use this method when you need to stop accepting new
    connections without disrupting online players.

### <a id="Void_Proxy_Api_IProxy_StartAcceptingConnectionsAsync_System_Threading_CancellationToken_"></a> StartAcceptingConnectionsAsync\(CancellationToken\)

Starts accepting connections on the listener.

```csharp
ValueTask StartAcceptingConnectionsAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_IProxy_Stop_System_Boolean_"></a> Stop\(bool\)

Stops the server and optionally waits for all online players to disconnect.

```csharp
void Stop(bool waitOnlinePlayers = false)
```

#### Parameters

`waitOnlinePlayers` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

A value indicating whether to wait for all online players to disconnect before stopping the server. <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> to wait for players to disconnect; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

#### Remarks

If <code class="paramref">waitOnlinePlayers</code> is set to <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>, the proxy will
    wait until all players have disconnected. Use this option to ensure a graceful shutdown when active player
    sessions are present.

