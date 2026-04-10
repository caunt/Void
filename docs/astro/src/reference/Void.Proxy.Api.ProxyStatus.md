# <a id="Void_Proxy_Api_ProxyStatus"></a> Enum ProxyStatus

Namespace: [Void.Proxy.Api](Void.Proxy.Api.md)  
Assembly: Void.Proxy.Api.dll  

Represents the operational state of the proxy server.

```csharp
public enum ProxyStatus
```

#### Extension Methods

[StructExtensions.IsDefault<ProxyStatus\>\(ProxyStatus\)](Void.Proxy.Api.Extensions.StructExtensions.md\#Void\_Proxy\_Api\_Extensions\_StructExtensions\_IsDefault\_\_1\_\_\_0\_)

## Fields

`Alive = 0` 

The proxy is running and accepting connections.



`Paused = 1` 

The proxy has temporarily paused accepting new connections.



`Stopping = 2` 

The proxy is in the process of shutting down.



