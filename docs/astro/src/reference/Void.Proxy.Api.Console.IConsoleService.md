# <a id="Void_Proxy_Api_Console_IConsoleService"></a> Interface IConsoleService

Namespace: [Void.Proxy.Api.Console](Void.Proxy.Api.Console.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IConsoleService : ICommandSource
```

#### Implements

[ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

## Methods

### <a id="Void_Proxy_Api_Console_IConsoleService_EnsureOptionDiscovered_System_CommandLine_Option_"></a> EnsureOptionDiscovered\(Option\)

Ensures that the specified command-line option is discovered by platform.

```csharp
void EnsureOptionDiscovered(Option option)
```

#### Parameters

`option` [Option](https://learn.microsoft.com/dotnet/api/system.commandline.option)

The option to be marked as discovered. Cannot be <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

#### Remarks

This method is typically used to track options that have been identified during
    processing.

### <a id="Void_Proxy_Api_Console_IConsoleService_GetOptionValue__1_System_CommandLine_Option___0__"></a> GetOptionValue<TValue\>\(Option<TValue\>\)

Retrieves the value associated with the specified option from the parsed command-line arguments.

```csharp
TValue? GetOptionValue<TValue>(Option<TValue> option)
```

#### Parameters

`option` [Option](https://learn.microsoft.com/dotnet/api/system.commandline.option\-1)<TValue\>

The option whose value is to be retrieved. Cannot be <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

#### Returns

 TValue?

The value of the specified option if it is present; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

#### Type Parameters

`TValue` 

The type of the value associated with the option.

#### Remarks

Use this method to access the value of a specific option after parsing command-line
    arguments. If the option is not present in the arguments, the method returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

### <a id="Void_Proxy_Api_Console_IConsoleService_GetRequiredOptionValue__1_System_CommandLine_Option___0__"></a> GetRequiredOptionValue<TValue\>\(Option<TValue\>\)

Retrieves the value of the specified option, ensuring that it is set from the parsed command-line arguments.

```csharp
TValue GetRequiredOptionValue<TValue>(Option<TValue> option)
```

#### Parameters

`option` [Option](https://learn.microsoft.com/dotnet/api/system.commandline.option\-1)<TValue\>

The option whose value is to be retrieved. Cannot be <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

#### Returns

 TValue

The value of the specified option.

#### Type Parameters

`TValue` 

The type of the value associated with the option.

### <a id="Void_Proxy_Api_Console_IConsoleService_HandleCommandsAsync_System_Threading_CancellationToken_"></a> HandleCommandsAsync\(CancellationToken\)

Asynchronously processes a single incoming command in the terminal until cancellation is requested. Do not call this method directly.

```csharp
ValueTask HandleCommandsAsync(CancellationToken cancellationToken = default)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to monitor for cancellation requests. The operation will stop processing current command if the token is
    canceled.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

A <xref href="System.Threading.Tasks.ValueTask" data-throw-if-not-resolved="false"></xref> representing the asynchronous operation.

### <a id="Void_Proxy_Api_Console_IConsoleService_TryGetOptionValue__1_System_CommandLine_Option___0____0__"></a> TryGetOptionValue<TValue\>\(Option<TValue\>, out TValue\)

Attempts to retrieve the value associated with the specified option from the parsed command-line arguments.

```csharp
bool TryGetOptionValue<TValue>(Option<TValue> option, out TValue value)
```

#### Parameters

`option` [Option](https://learn.microsoft.com/dotnet/api/system.commandline.option\-1)<TValue\>

The option for which to retrieve the value. Cannot be <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

`value` TValue

When this method returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>, contains the value associated with the specified option. When
    this method returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>, the value is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the value for the specified option was successfully retrieved; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

#### Type Parameters

`TValue` 

The type of the value associated with the option.

