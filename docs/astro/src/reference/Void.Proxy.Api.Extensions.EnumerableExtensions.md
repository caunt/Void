# <a id="Void_Proxy_Api_Extensions_EnumerableExtensions"></a> Class EnumerableExtensions

Namespace: [Void.Proxy.Api.Extensions](Void.Proxy.Api.Extensions.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public static class EnumerableExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[EnumerableExtensions](Void.Proxy.Api.Extensions.EnumerableExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Proxy_Api_Extensions_EnumerableExtensions_Synchronized__1_System_Collections_Generic_IEnumerable___0__Nito_AsyncEx_AsyncLock_System_Threading_CancellationToken_"></a> Synchronized<T\>\(IEnumerable<T\>, AsyncLock, CancellationToken\)

```csharp
public static IEnumerable<T> Synchronized<T>(this IEnumerable<T> source, AsyncLock @lock, CancellationToken cancellationToken)
```

#### Parameters

`source` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<T\>

`lock` AsyncLock

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<T\>

#### Type Parameters

`T` 

### <a id="Void_Proxy_Api_Extensions_EnumerableExtensions_SynchronizedAsync__1_System_Collections_Generic_IEnumerable___0__Nito_AsyncEx_AsyncLock_System_Threading_CancellationToken_"></a> SynchronizedAsync<T\>\(IEnumerable<T\>, AsyncLock, CancellationToken\)

```csharp
public static IAsyncEnumerable<T> SynchronizedAsync<T>(this IEnumerable<T> source, AsyncLock @lock, CancellationToken cancellationToken)
```

#### Parameters

`source` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<T\>

`lock` AsyncLock

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [IAsyncEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.iasyncenumerable\-1)<T\>

#### Type Parameters

`T` 

