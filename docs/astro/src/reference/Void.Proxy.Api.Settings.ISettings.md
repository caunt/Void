# <a id="Void_Proxy_Api_Settings_ISettings"></a> Interface ISettings

Namespace: [Void.Proxy.Api.Settings](Void.Proxy.Api.Settings.md)  
Assembly: Void.Proxy.Api.dll  

Exposes the runtime-readable configuration values that govern core proxy behavior,
including network binding, protocol settings, and the backend server list.

```csharp
public interface ISettings
```

## Properties

### <a id="Void_Proxy_Api_Settings_ISettings_Address"></a> Address

Gets the network address the proxy binds to when accepting incoming player connections.
Defaults to <xref href="System.Net.IPAddress.Any" data-throw-if-not-resolved="false"></xref>, which listens on all available network interfaces.

```csharp
IPAddress Address { get; }
```

#### Property Value

 [IPAddress](https://learn.microsoft.com/dotnet/api/system.net.ipaddress)

### <a id="Void_Proxy_Api_Settings_ISettings_CompressionThreshold"></a> CompressionThreshold

Gets the minimum uncompressed packet payload size in bytes at which Zlib compression is applied.
Packets whose payload is smaller than this threshold are forwarded without compression.
Defaults to <code>256</code> bytes.

```csharp
int CompressionThreshold { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Proxy_Api_Settings_ISettings_KickTimeout"></a> KickTimeout

Gets the maximum time in milliseconds the proxy waits for plugins to finish graceful kick
handling before force-disconnecting the player.
Defaults to <code>10000</code> ms (10 seconds).

```csharp
int KickTimeout { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Proxy_Api_Settings_ISettings_LogLevel"></a> LogLevel

Gets the minimum severity level for log messages emitted by the proxy.
Defaults to <xref href="Microsoft.Extensions.Logging.LogLevel.Information" data-throw-if-not-resolved="false"></xref>.

```csharp
LogLevel LogLevel { get; }
```

#### Property Value

 [LogLevel](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.loglevel)

### <a id="Void_Proxy_Api_Settings_ISettings_Offline"></a> Offline

Gets or sets a value indicating whether the proxy operates in offline mode.
When <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>, Mojang authentication is bypassed and players can connect
without a valid premium account.

```csharp
bool Offline { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Remarks

This flag can be overridden at startup via the <code>VOID_OFFLINE</code> environment variable
or a command-line option.

### <a id="Void_Proxy_Api_Settings_ISettings_Port"></a> Port

Gets the TCP port the proxy listens on for incoming player connections.
Defaults to <code>25565</code>, the standard Minecraft Java Edition port.

```csharp
int Port { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Proxy_Api_Settings_ISettings_Servers"></a> Servers

Gets the collection of backend Minecraft servers the proxy is configured to route players to.

```csharp
IEnumerable<IServer> Servers { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[IServer](Void.Proxy.Api.Servers.IServer.md)\>

