# <a id="Void_Minecraft_Players_MinecraftPlayer"></a> Class MinecraftPlayer

Namespace: [Void.Minecraft.Players](Void.Minecraft.Players.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class MinecraftPlayer : IPlayer, IEquatable<IPlayer>, ICommandSource, IAsyncDisposable, IDisposable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[MinecraftPlayer](Void.Minecraft.Players.MinecraftPlayer.md)

#### Implements

[IPlayer](Void.Proxy.Api.Players.IPlayer.md), 
[IEquatable<IPlayer\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md), 
[IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Players_MinecraftPlayer__ctor_System_Net_Sockets_TcpClient_Void_Proxy_Api_Players_Contexts_IPlayerContext_System_String_System_DateTimeOffset_Void_Minecraft_Network_ProtocolVersion_"></a> MinecraftPlayer\(TcpClient, IPlayerContext, string, DateTimeOffset, ProtocolVersion\)

```csharp
public MinecraftPlayer(TcpClient client, IPlayerContext context, string remoteEndPoint, DateTimeOffset connectedAt, ProtocolVersion protocolVersion)
```

#### Parameters

`client` [TcpClient](https://learn.microsoft.com/dotnet/api/system.net.sockets.tcpclient)

`context` [IPlayerContext](Void.Proxy.Api.Players.Contexts.IPlayerContext.md)

`remoteEndPoint` [string](https://learn.microsoft.com/dotnet/api/system.string)

`connectedAt` [DateTimeOffset](https://learn.microsoft.com/dotnet/api/system.datetimeoffset)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

## Properties

### <a id="Void_Minecraft_Players_MinecraftPlayer_Client"></a> Client

Gets the underlying <xref href="System.Net.Sockets.TcpClient" data-throw-if-not-resolved="false"></xref> used for network.
communication.

```csharp
public TcpClient Client { get; }
```

#### Property Value

 [TcpClient](https://learn.microsoft.com/dotnet/api/system.net.sockets.tcpclient)

### <a id="Void_Minecraft_Players_MinecraftPlayer_ConnectedAt"></a> ConnectedAt

Gets the date and time when the player connected to the proxy.

```csharp
public DateTimeOffset ConnectedAt { get; }
```

#### Property Value

 [DateTimeOffset](https://learn.microsoft.com/dotnet/api/system.datetimeoffset)

### <a id="Void_Minecraft_Players_MinecraftPlayer_Context"></a> Context

Gets the context containing services and state associated with the
player.

```csharp
public IPlayerContext Context { get; }
```

#### Property Value

 [IPlayerContext](Void.Proxy.Api.Players.Contexts.IPlayerContext.md)

### <a id="Void_Minecraft_Players_MinecraftPlayer_IdentifiedKey"></a> IdentifiedKey

```csharp
public IdentifiedKey? IdentifiedKey { get; set; }
```

#### Property Value

 [IdentifiedKey](Void.Minecraft.Profiles.IdentifiedKey.md)?

### <a id="Void_Minecraft_Players_MinecraftPlayer_Name"></a> Name

Gets the name of the player. Falls back to IP address if unavailable.

```csharp
public string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Players_MinecraftPlayer_Phase"></a> Phase

```csharp
public Phase Phase { get; set; }
```

#### Property Value

 [Phase](Void.Minecraft.Network.Phase.md)

### <a id="Void_Minecraft_Players_MinecraftPlayer_Profile"></a> Profile

```csharp
public GameProfile? Profile { get; set; }
```

#### Property Value

 [GameProfile](Void.Minecraft.Profiles.GameProfile.md)?

### <a id="Void_Minecraft_Players_MinecraftPlayer_ProtocolVersion"></a> ProtocolVersion

```csharp
public ProtocolVersion ProtocolVersion { get; }
```

#### Property Value

 [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

### <a id="Void_Minecraft_Players_MinecraftPlayer_RemoteEndPoint"></a> RemoteEndPoint

Gets the textual representation of the client's remote endpoint.

```csharp
public string RemoteEndPoint { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Void_Minecraft_Players_MinecraftPlayer_Dispose"></a> Dispose\(\)

Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

```csharp
public void Dispose()
```

### <a id="Void_Minecraft_Players_MinecraftPlayer_DisposeAsync"></a> DisposeAsync\(\)

Performs application-defined tasks associated with freeing, releasing, or
resetting unmanaged resources asynchronously.

```csharp
public ValueTask DisposeAsync()
```

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

A task that represents the asynchronous dispose operation.

### <a id="Void_Minecraft_Players_MinecraftPlayer_Equals_Void_Proxy_Api_Players_IPlayer_"></a> Equals\(IPlayer?\)

Indicates whether the current object is equal to another object of the same type.

```csharp
public bool Equals(IPlayer? other)
```

#### Parameters

`other` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)?

An object to compare with this object.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the current object is equal to the <code class="paramref">other</code> parameter; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Players_MinecraftPlayer_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

