# <a id="Void_Proxy_Api_Events_Encryption_SearchServerPrivateKeyEvent"></a> Class SearchServerPrivateKeyEvent

Namespace: [Void.Proxy.Api.Events.Encryption](Void.Proxy.Api.Events.Encryption.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record SearchServerPrivateKeyEvent : IEventWithResult<byte[]>, IEvent, IEquatable<SearchServerPrivateKeyEvent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[SearchServerPrivateKeyEvent](Void.Proxy.Api.Events.Encryption.SearchServerPrivateKeyEvent.md)

#### Implements

[IEventWithResult<byte\[\]\>](Void.Proxy.Api.Events.IEventWithResult\-1.md), 
[IEvent](Void.Proxy.Api.Events.IEvent.md), 
[IEquatable<SearchServerPrivateKeyEvent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Encryption_SearchServerPrivateKeyEvent__ctor_Void_Proxy_Api_Servers_IServer_"></a> SearchServerPrivateKeyEvent\(IServer\)

```csharp
public SearchServerPrivateKeyEvent(IServer Server)
```

#### Parameters

`Server` [IServer](Void.Proxy.Api.Servers.IServer.md)

## Properties

### <a id="Void_Proxy_Api_Events_Encryption_SearchServerPrivateKeyEvent_Result"></a> Result

Gets or sets the value produced while handling the event.

```csharp
public byte[]? Result { get; set; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]?

### <a id="Void_Proxy_Api_Events_Encryption_SearchServerPrivateKeyEvent_Server"></a> Server

```csharp
public IServer Server { get; init; }
```

#### Property Value

 [IServer](Void.Proxy.Api.Servers.IServer.md)

