# <a id="Void_Proxy_Api_Network_Streams_IMessageStreamBase"></a> Interface IMessageStreamBase

Namespace: [Void.Proxy.Api.Network.Streams](Void.Proxy.Api.Network.Streams.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IMessageStreamBase : IDisposable, IAsyncDisposable
```

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), 
[IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

## Properties

### <a id="Void_Proxy_Api_Network_Streams_IMessageStreamBase_CanRead"></a> CanRead

```csharp
bool CanRead { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Network_Streams_IMessageStreamBase_CanWrite"></a> CanWrite

```csharp
bool CanWrite { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Network_Streams_IMessageStreamBase_IsAlive"></a> IsAlive

```csharp
bool IsAlive { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Methods

### <a id="Void_Proxy_Api_Network_Streams_IMessageStreamBase_Close"></a> Close\(\)

```csharp
void Close()
```

### <a id="Void_Proxy_Api_Network_Streams_IMessageStreamBase_Flush"></a> Flush\(\)

```csharp
void Flush()
```

### <a id="Void_Proxy_Api_Network_Streams_IMessageStreamBase_FlushAsync_System_Threading_CancellationToken_"></a> FlushAsync\(CancellationToken\)

```csharp
ValueTask FlushAsync(CancellationToken cancellationToken = default)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

