# <a id="Void_Proxy_Api_Events_Services_IEventService"></a> Interface IEventService

Namespace: [Void.Proxy.Api.Events.Services](Void.Proxy.Api.Events.Services.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IEventService
```

## Properties

### <a id="Void_Proxy_Api_Events_Services_IEventService_Listeners"></a> Listeners

```csharp
IEnumerable<IEventListener> Listeners { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[IEventListener](Void.Proxy.Api.Events.IEventListener.md)\>

## Methods

### <a id="Void_Proxy_Api_Events_Services_IEventService_RegisterListeners_System_Collections_Generic_IEnumerable_Void_Proxy_Api_Events_IEventListener__System_Threading_CancellationToken_"></a> RegisterListeners\(IEnumerable<IEventListener\>, CancellationToken\)

```csharp
void RegisterListeners(IEnumerable<IEventListener> listeners, CancellationToken cancellationToken = default)
```

#### Parameters

`listeners` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[IEventListener](Void.Proxy.Api.Events.IEventListener.md)\>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

### <a id="Void_Proxy_Api_Events_Services_IEventService_RegisterListeners_System_Threading_CancellationToken_Void_Proxy_Api_Events_IEventListener___"></a> RegisterListeners\(CancellationToken, params IEventListener\[\]\)

```csharp
void RegisterListeners(CancellationToken cancellationToken = default, params IEventListener[] listeners)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

`listeners` [IEventListener](Void.Proxy.Api.Events.IEventListener.md)\[\]

### <a id="Void_Proxy_Api_Events_Services_IEventService_RegisterListeners_Void_Proxy_Api_Events_IEventListener___"></a> RegisterListeners\(params IEventListener\[\]\)

```csharp
void RegisterListeners(params IEventListener[] listeners)
```

#### Parameters

`listeners` [IEventListener](Void.Proxy.Api.Events.IEventListener.md)\[\]

### <a id="Void_Proxy_Api_Events_Services_IEventService_ThrowAsync__1_System_Threading_CancellationToken_"></a> ThrowAsync<T\>\(CancellationToken\)

```csharp
ValueTask ThrowAsync<T>(CancellationToken cancellationToken = default) where T : IEvent, new()
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

#### Type Parameters

`T` 

### <a id="Void_Proxy_Api_Events_Services_IEventService_ThrowAsync__1___0_System_Threading_CancellationToken_"></a> ThrowAsync<T\>\(T, CancellationToken\)

```csharp
ValueTask ThrowAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent
```

#### Parameters

`event` T

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

#### Type Parameters

`T` 

### <a id="Void_Proxy_Api_Events_Services_IEventService_ThrowWithResultAsync__1_Void_Proxy_Api_Events_IEventWithResult___0__System_Threading_CancellationToken_"></a> ThrowWithResultAsync<TResult\>\(IEventWithResult<TResult\>, CancellationToken\)

```csharp
ValueTask<TResult?> ThrowWithResultAsync<TResult>(IEventWithResult<TResult> @event, CancellationToken cancellationToken = default)
```

#### Parameters

`event` [IEventWithResult](Void.Proxy.Api.Events.IEventWithResult\-1.md)<TResult\>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<TResult?\>

#### Type Parameters

`TResult` 

### <a id="Void_Proxy_Api_Events_Services_IEventService_ThrowWithResultAsync__2_System_Threading_CancellationToken_"></a> ThrowWithResultAsync<T, TResult\>\(CancellationToken\)

```csharp
ValueTask<TResult?> ThrowWithResultAsync<T, TResult>(CancellationToken cancellationToken = default) where T : IEventWithResult<TResult?>, new()
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<TResult?\>

#### Type Parameters

`T` 

`TResult` 

### <a id="Void_Proxy_Api_Events_Services_IEventService_UnregisterListeners_System_Collections_Generic_IEnumerable_Void_Proxy_Api_Events_IEventListener__"></a> UnregisterListeners\(IEnumerable<IEventListener\>\)

```csharp
void UnregisterListeners(IEnumerable<IEventListener> listeners)
```

#### Parameters

`listeners` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[IEventListener](Void.Proxy.Api.Events.IEventListener.md)\>

### <a id="Void_Proxy_Api_Events_Services_IEventService_UnregisterListeners_Void_Proxy_Api_Events_IEventListener___"></a> UnregisterListeners\(params IEventListener\[\]\)

```csharp
void UnregisterListeners(params IEventListener[] listeners)
```

#### Parameters

`listeners` [IEventListener](Void.Proxy.Api.Events.IEventListener.md)\[\]

### <a id="Void_Proxy_Api_Events_Services_IEventService_WaitAsync_System_Func_Void_Proxy_Api_Events_IEvent_System_Boolean__System_Threading_CancellationToken_"></a> WaitAsync\(Func<IEvent, bool\>, CancellationToken\)

```csharp
ValueTask WaitAsync(Func<IEvent, bool> condition, CancellationToken cancellationToken = default)
```

#### Parameters

`condition` [Func](https://learn.microsoft.com/dotnet/api/system.func\-2)<[IEvent](Void.Proxy.Api.Events.IEvent.md), [bool](https://learn.microsoft.com/dotnet/api/system.boolean)\>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

