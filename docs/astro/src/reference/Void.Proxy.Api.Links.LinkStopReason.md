# <a id="Void_Proxy_Api_Links_LinkStopReason"></a> Enum LinkStopReason

Namespace: [Void.Proxy.Api.Links](Void.Proxy.Api.Links.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public enum LinkStopReason
```

#### Extension Methods

[StructExtensions.IsDefault<LinkStopReason\>\(LinkStopReason\)](Void.Proxy.Api.Extensions.StructExtensions.md\#Void\_Proxy\_Api\_Extensions\_StructExtensions\_IsDefault\_\_1\_\_\_0\_)

## Fields

`InternalException = 2` 

An internal exception occurred in <xref href="Void.Proxy.Api.Links.ILink" data-throw-if-not-resolved="false"></xref> implementation.



`PlayerDisconnected = 0` 

Specifies that the player disconnected from the proxy.



`Requested = 3` 

Manually requested to stop the <xref href="Void.Proxy.Api.Links.ILink" data-throw-if-not-resolved="false"></xref> implementation, e.g., by calling the <xref href="Void.Proxy.Api.Links.ILink.StopAsync(System.Threading.CancellationToken)" data-throw-if-not-resolved="false"></xref> method.



`ServerDisconnected = 1` 

Specifies that the player was kicked from the server or the server closed the connection abnormally.



