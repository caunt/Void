# <a id="Void_Proxy_Api_Configurations_IConfigurationService"></a> Interface IConfigurationService

Namespace: [Void.Proxy.Api.Configurations](Void.Proxy.Api.Configurations.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IConfigurationService : IHostedService
```

#### Implements

[IHostedService](https://learn.microsoft.com/dotnet/api/microsoft.extensions.hosting.ihostedservice)

## Methods

### <a id="Void_Proxy_Api_Configurations_IConfigurationService_GetAsync__1_System_Threading_CancellationToken_"></a> GetAsync<TConfiguration\>\(CancellationToken\)

Retrieves a configuration instance of type <code class="typeparamref">TConfiguration</code>.
The returned instance is fully self-managed: any changes made to the instance are automatically saved to disk, 
and any changes from disk are automatically loaded into the instance.

```csharp
ValueTask<TConfiguration> GetAsync<TConfiguration>(CancellationToken cancellationToken = default) where TConfiguration : notnull
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token used to signal the cancellation of the asynchronous operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<TConfiguration\>

A <xref href="System.Threading.Tasks.ValueTask%601" data-throw-if-not-resolved="false"></xref> that resolves to the requested configuration.

#### Type Parameters

`TConfiguration` 

The type of configuration being retrieved. It must not be null.

### <a id="Void_Proxy_Api_Configurations_IConfigurationService_GetAsync__1_System_String_System_Threading_CancellationToken_"></a> GetAsync<TConfiguration\>\(string, CancellationToken\)

Retrieves a configuration instance of type <code class="typeparamref">TConfiguration</code>.
The returned instance is fully self-managed: any changes made to the instance are automatically saved to disk, 
and any changes from disk are automatically loaded into the instance.

```csharp
ValueTask<TConfiguration> GetAsync<TConfiguration>(string key, CancellationToken cancellationToken = default) where TConfiguration : notnull
```

#### Parameters

`key` [string](https://learn.microsoft.com/dotnet/api/system.string)

Specifies the identifier for the configuration value to be fetched.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token used to signal the cancellation of the asynchronous operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<TConfiguration\>

A <xref href="System.Threading.Tasks.ValueTask%601" data-throw-if-not-resolved="false"></xref> that resolves to the requested configuration.

#### Type Parameters

`TConfiguration` 

The type of configuration being retrieved. It must not be null.

