# <a id="Void_Minecraft_Links_Extensions_LinkExtensions"></a> Class LinkExtensions

Namespace: [Void.Minecraft.Links.Extensions](Void.Minecraft.Links.Extensions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class LinkExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[LinkExtensions](Void.Minecraft.Links.Extensions.LinkExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Links_Extensions_LinkExtensions_SendPacketAsync__1_Void_Proxy_Api_Links_ILink_System_Threading_CancellationToken_"></a> SendPacketAsync<T\>\(ILink, CancellationToken\)

```csharp
public static ValueTask SendPacketAsync<T>(this ILink link, CancellationToken cancellationToken) where T : class, IMinecraftMessage, new()
```

#### Parameters

`link` [ILink](Void.Proxy.Api.Links.ILink.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Links_Extensions_LinkExtensions_SendPacketAsync__1_Void_Proxy_Api_Links_ILink___0_System_Threading_CancellationToken_"></a> SendPacketAsync<T\>\(ILink, T, CancellationToken\)

```csharp
public static ValueTask SendPacketAsync<T>(this ILink link, T packet, CancellationToken cancellationToken) where T : class, IMinecraftMessage
```

#### Parameters

`link` [ILink](Void.Proxy.Api.Links.ILink.md)

`packet` T

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Links_Extensions_LinkExtensions_SendPacketAsync__1_Void_Proxy_Api_Links_ILink_Void_Proxy_Api_Network_Side___0_System_Threading_CancellationToken_"></a> SendPacketAsync<T\>\(ILink, Side, T, CancellationToken\)

```csharp
public static ValueTask SendPacketAsync<T>(this ILink link, Side side, T packet, CancellationToken cancellationToken) where T : IMinecraftMessage
```

#### Parameters

`link` [ILink](Void.Proxy.Api.Links.ILink.md)

`side` [Side](Void.Proxy.Api.Network.Side.md)

`packet` T

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

#### Type Parameters

`T` 

