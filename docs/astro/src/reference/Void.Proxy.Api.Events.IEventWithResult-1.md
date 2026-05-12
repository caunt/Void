# <a id="Void_Proxy_Api_Events_IEventWithResult_1"></a> Interface IEventWithResult<T\>

Namespace: [Void.Proxy.Api.Events](Void.Proxy.Api.Events.md)  
Assembly: Void.Proxy.Api.dll  

Represents an event whose listeners can communicate an outcome by setting <xref href="Void.Proxy.Api.Events.IEventWithResult%601.Result" data-throw-if-not-resolved="false"></xref>.

```csharp
public interface IEventWithResult<T> : IEvent
```

#### Type Parameters

`T` 

The type of value produced by listeners for this event.

#### Implements

[IEvent](Void.Proxy.Api.Events.IEvent.md)

## Remarks

The event service publishes the event to listeners first and then returns the final <xref href="Void.Proxy.Api.Events.IEventWithResult%601.Result" data-throw-if-not-resolved="false"></xref> value to the caller.

## Properties

### <a id="Void_Proxy_Api_Events_IEventWithResult_1_Result"></a> Result

Gets or sets the value produced while handling the event.

```csharp
T? Result { get; set; }
```

#### Property Value

 T?

