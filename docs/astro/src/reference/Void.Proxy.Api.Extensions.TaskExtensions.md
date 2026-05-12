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

Attaches a continuation that logs failures or cancellations from <code class="paramref">task</code> and returns the continuation task.

```csharp
public static Task CatchExceptions(this Task task, ILogger logger, string message)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)

The task to observe for non-successful completion.

`logger` [ILogger](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger)

The logger used to emit error messages.

`message` [string](https://learn.microsoft.com/dotnet/api/system.string)

A message prefix included in each error log entry.

#### Returns

 [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)

A continuation task that completes after any required logging for <code class="paramref">task</code> has finished.

#### Examples

<pre><code class="lang-csharp">_ = backgroundTask.CatchExceptions(logger, "Background processing failed");</code></pre>

#### Remarks

<p>
The continuation is registered with <xref href="System.Threading.Tasks.TaskContinuationOptions.NotOnRanToCompletion" data-throw-if-not-resolved="false"></xref>, so it runs only when the antecedent task faults or is canceled.
</p>
<p>
For faulted tasks, this method logs <code>completedTask.Exception.InnerException</code> (the first inner exception from the task's <xref href="System.AggregateException" data-throw-if-not-resolved="false"></xref>). For canceled tasks, it calls <xref href="System.Threading.Tasks.Task.Wait" data-throw-if-not-resolved="false"></xref> to materialize <xref href="System.Threading.Tasks.TaskCanceledException" data-throw-if-not-resolved="false"></xref>, then logs that exception together with the call-site stack trace captured when this method was invoked.
</p>
<p>
The returned <xref href="System.Threading.Tasks.Task" data-throw-if-not-resolved="false"></xref> represents only the logging continuation, not the original task result. Because it does not rethrow antecedent failures, awaiting the returned task usually completes successfully unless logging itself throws.
</p>

#### See Also

[Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task).[ContinueWith](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task.continuewith\#system\-threading\-tasks\-task\-continuewith\(system\-action\(\(system\-threading\-tasks\-task\)\)\-system\-threading\-tasks\-taskcontinuationoptions\))\([Action](https://learn.microsoft.com/dotnet/api/system.action\-1)<[Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)\>, [TaskContinuationOptions](https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcontinuationoptions)\), 
[TaskExtensions](Void.Proxy.Api.Extensions.TaskExtensions.md).[CatchExceptions](Void.Proxy.Api.Extensions.TaskExtensions.md\#Void\_Proxy\_Api\_Extensions\_TaskExtensions\_CatchExceptions\_System\_Threading\_Tasks\_ValueTask\_Microsoft\_Extensions\_Logging\_ILogger\_System\_String\_)\([ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask), [ILogger](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger), [string](https://learn.microsoft.com/dotnet/api/system.string)\)

### <a id="Void_Proxy_Api_Extensions_TaskExtensions_CatchExceptions_System_Threading_Tasks_ValueTask_Microsoft_Extensions_Logging_ILogger_System_String_"></a> CatchExceptions\(ValueTask, ILogger, string\)

Converts <code class="paramref">task</code> to <xref href="System.Threading.Tasks.Task" data-throw-if-not-resolved="false"></xref> and applies <xref href="Void.Proxy.Api.Extensions.TaskExtensions.CatchExceptions(System.Threading.Tasks.Task%2cMicrosoft.Extensions.Logging.ILogger%2cSystem.String)" data-throw-if-not-resolved="false"></xref>.

```csharp
public static ValueTask CatchExceptions(this ValueTask task, ILogger logger, string message)
```

#### Parameters

`task` [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

The value task to observe for non-successful completion.

`logger` [ILogger](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger)

The logger used to emit error messages.

`message` [string](https://learn.microsoft.com/dotnet/api/system.string)

A message prefix included in each error log entry.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

A <xref href="System.Threading.Tasks.ValueTask" data-throw-if-not-resolved="false"></xref> wrapping the logging continuation produced from <code class="paramref">task</code>.

#### Examples

<pre><code class="lang-csharp">await valueTask.CatchExceptions(logger, "Link stop handler failed");</code></pre>

#### Remarks

<p>
This overload delegates all logging behavior and error-handling semantics to <xref href="Void.Proxy.Api.Extensions.TaskExtensions.CatchExceptions(System.Threading.Tasks.Task%2cMicrosoft.Extensions.Logging.ILogger%2cSystem.String)" data-throw-if-not-resolved="false"></xref>.
</p>
<p>
The returned <xref href="System.Threading.Tasks.ValueTask" data-throw-if-not-resolved="false"></xref> wraps the continuation task created for logging. As a result, awaiting it waits for logging completion rather than rethrowing the original failure from <code class="paramref">task</code>.
</p>

#### See Also

[ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask).[AsTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask.astask)\(\)

