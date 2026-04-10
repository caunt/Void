# <a id="Void_Proxy_Api_Servers_IServer"></a> Interface IServer

Namespace: [Void.Proxy.Api.Servers](Void.Proxy.Api.Servers.md)  
Assembly: Void.Proxy.Api.dll  

Represents a server that players can connect to.

```csharp
public interface IServer
```

## Properties

### <a id="Void_Proxy_Api_Servers_IServer_Brand"></a> Brand

Gets or sets the implementation brand of the server.

```csharp
string? Brand { get; set; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Proxy_Api_Servers_IServer_Host"></a> Host

Gets the host address of the server.

```csharp
string Host { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Proxy_Api_Servers_IServer_Name"></a> Name

Gets the name of the server.

```csharp
string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Proxy_Api_Servers_IServer_Override"></a> Override

Gets or sets the hostname override for the server.

```csharp
string? Override { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Proxy_Api_Servers_IServer_Port"></a> Port

Gets the port used for connecting to the server.

```csharp
int Port { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Void_Proxy_Api_Servers_IServer_CreateTcpClient"></a> CreateTcpClient\(\)

Creates a <xref href="System.Net.Sockets.TcpClient" data-throw-if-not-resolved="false"></xref> connected to this server.

```csharp
TcpClient CreateTcpClient()
```

#### Returns

 [TcpClient](https://learn.microsoft.com/dotnet/api/system.net.sockets.tcpclient)

A configured <xref href="System.Net.Sockets.TcpClient" data-throw-if-not-resolved="false"></xref> instance.

