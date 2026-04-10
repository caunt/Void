# <a id="Void_Proxy_Api_Extensions_TaskExtensions"></a> Class TaskExtensions

Namespace: [Void.Proxy.Api.Extensions](Void.Proxy.Api.Extensions.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public static class TaskExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TaskExtensions](Void.Proxy.Api.Extensions.TaskExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Proxy_Api_Extensions_TaskExtensions_CatchExceptions_System_Threading_Tasks_Task_Microsoft_Extensions_Logging_ILogger_System_String_"></a> CatchExceptions\(Task, ILogger, string\)

```csharp
public static Task CatchExceptions(this Task task, ILogger logger, string message)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)

`logger` [ILogger](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger)

`message` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)

### <a id="Void_Proxy_Api_Extensions_TaskExtensions_CatchExceptions_System_Threading_Tasks_ValueTask_Microsoft_Extensions_Logging_ILogger_System_String_"></a> CatchExceptions\(ValueTask, ILogger, string\)

```csharp
public static ValueTask CatchExceptions(this ValueTask task, ILogger logger, string message)
```

#### Parameters

`task` [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

`logger` [ILogger](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger)

`message` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

