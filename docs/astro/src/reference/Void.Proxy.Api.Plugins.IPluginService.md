# <a id="Void_Proxy_Api_Plugins_IPluginService"></a> Interface IPluginService

Namespace: [Void.Proxy.Api.Plugins](Void.Proxy.Api.Plugins.md)  
Assembly: Void.Proxy.Api.dll  

Manages the lifecycle of proxy plugins, including discovery, loading, and unloading
of plugin containers backed by isolated <xref href="System.Runtime.Loader.AssemblyLoadContext" data-throw-if-not-resolved="false"></xref> instances.

```csharp
public interface IPluginService
```

#### Extension Methods

[PluginExtensions.GetPluginFromType<T\>\(IPluginService\)](Void.Proxy.Api.Plugins.Extensions.PluginExtensions.md\#Void\_Proxy\_Api\_Plugins\_Extensions\_PluginExtensions\_GetPluginFromType\_\_1\_Void\_Proxy\_Api\_Plugins\_IPluginService\_), 
[PluginExtensions.GetPluginFromType\(IPluginService, Type\)](Void.Proxy.Api.Plugins.Extensions.PluginExtensions.md\#Void\_Proxy\_Api\_Plugins\_Extensions\_PluginExtensions\_GetPluginFromType\_Void\_Proxy\_Api\_Plugins\_IPluginService\_System\_Type\_), 
[PluginExtensions.TryGetPluginFromType\(IPluginService, Type, out IPlugin\)](Void.Proxy.Api.Plugins.Extensions.PluginExtensions.md\#Void\_Proxy\_Api\_Plugins\_Extensions\_PluginExtensions\_TryGetPluginFromType\_Void\_Proxy\_Api\_Plugins\_IPluginService\_System\_Type\_Void\_Proxy\_Api\_Plugins\_IPlugin\_\_)

## Properties

### <a id="Void_Proxy_Api_Plugins_IPluginService_All"></a> All

Gets all plugin instances currently active across all loaded containers.

```csharp
IEnumerable<IPlugin> All { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)\>

### <a id="Void_Proxy_Api_Plugins_IPluginService_Containers"></a> Containers

Gets the names of all currently loaded plugin containers.

```csharp
IEnumerable<string> Containers { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

## Methods

### <a id="Void_Proxy_Api_Plugins_IPluginService_GetPlugins_System_Reflection_Assembly_"></a> GetPlugins\(Assembly\)

Returns all non-abstract types in <code class="paramref">assembly</code> that implement <xref href="Void.Proxy.Api.Plugins.IPlugin" data-throw-if-not-resolved="false"></xref>.

```csharp
IEnumerable<Type> GetPlugins(Assembly assembly)
```

#### Parameters

`assembly` [Assembly](https://learn.microsoft.com/dotnet/api/system.reflection.assembly)

The assembly to scan.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Type](https://learn.microsoft.com/dotnet/api/system.type)\>

The discovered <xref href="Void.Proxy.Api.Plugins.IPlugin" data-throw-if-not-resolved="false"></xref> implementation types.

### <a id="Void_Proxy_Api_Plugins_IPluginService_LoadContainer_System_String_System_IO_Stream_System_Boolean_"></a> LoadContainer\(string, Stream, bool\)

Reads an assembly from <code class="paramref">stream</code> into an isolated load context,
discovers all non-abstract <xref href="Void.Proxy.Api.Plugins.IPlugin" data-throw-if-not-resolved="false"></xref> implementations, and returns those types.

```csharp
IEnumerable<Type> LoadContainer(string name, Stream stream, bool ignoreEmpty = false)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

A label used to identify the container in logs and the <xref href="Void.Proxy.Api.Plugins.IPluginService.Containers" data-throw-if-not-resolved="false"></xref> list.

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

A stream containing the assembly bytes to load.

`ignoreEmpty` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

When <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>, logs a trace-level message instead of a warning
when the assembly contains no <xref href="Void.Proxy.Api.Plugins.IPlugin" data-throw-if-not-resolved="false"></xref> implementations.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Type](https://learn.microsoft.com/dotnet/api/system.type)\>

The non-abstract types that implement <xref href="Void.Proxy.Api.Plugins.IPlugin" data-throw-if-not-resolved="false"></xref> found in the loaded assembly.

### <a id="Void_Proxy_Api_Plugins_IPluginService_LoadDirectoryPluginsAsync_System_String_System_Threading_CancellationToken_"></a> LoadDirectoryPluginsAsync\(string, CancellationToken\)

Loads all <code>.dll</code> files found in <code class="paramref">path</code> as plugin containers.
Creates the directory when it does not already exist.

```csharp
ValueTask LoadDirectoryPluginsAsync(string path = "plugins", CancellationToken cancellationToken = default)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

Relative or absolute path to the plugins directory. Defaults to <code>"plugins"</code>.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_Plugins_IPluginService_LoadEmbeddedPluginsAsync_System_Threading_CancellationToken_"></a> LoadEmbeddedPluginsAsync\(CancellationToken\)

Loads plugins embedded as manifest resources in the executing assembly.
Resources whose names contain <code>Plugins</code> are treated as plugin assemblies.

```csharp
ValueTask LoadEmbeddedPluginsAsync(CancellationToken cancellationToken = default)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_Plugins_IPluginService_LoadEnvironmentPluginsAsync_System_Threading_CancellationToken_"></a> LoadEnvironmentPluginsAsync\(CancellationToken\)

Loads plugins specified via the <code>--plugin</code>/<code>-p</code> command-line arguments
or the <code>VOID_PLUGINS</code> environment variable.
Each entry may be a local file path, a directory path, or an HTTP/HTTPS URL.

```csharp
ValueTask LoadEnvironmentPluginsAsync(CancellationToken cancellationToken = default)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_Plugins_IPluginService_LoadPluginAsync_System_Type_System_Threading_CancellationToken_"></a> LoadPluginAsync\(Type, CancellationToken\)

Instantiates a single plugin type within its existing container, fires
<code>PluginLoadingEvent</code> and <code>PluginLoadedEvent</code>, and registers the plugin.

```csharp
ValueTask LoadPluginAsync(Type pluginType, CancellationToken cancellationToken = default)
```

#### Parameters

`pluginType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

The concrete type implementing <xref href="Void.Proxy.Api.Plugins.IPlugin" data-throw-if-not-resolved="false"></xref> to load.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

#### Exceptions

 [Exception](https://learn.microsoft.com/dotnet/api/system.exception)

Thrown when no container is found for <code class="paramref">pluginType</code>'s assembly.

### <a id="Void_Proxy_Api_Plugins_IPluginService_LoadPluginsAsync_System_String_System_Threading_CancellationToken_"></a> LoadPluginsAsync\(string, CancellationToken\)

Sequentially loads environment plugins (from command-line arguments and environment variables),
embedded plugins, and directory plugins from <code class="paramref">path</code>.

```csharp
ValueTask LoadPluginsAsync(string path = "plugins", CancellationToken cancellationToken = default)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

Relative or absolute path to the directory scanned for plugin <code>.dll</code> files. Defaults to <code>"plugins"</code>.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_Plugins_IPluginService_LoadPluginsAsync_System_Collections_Generic_IEnumerable_System_Type__System_Threading_CancellationToken_"></a> LoadPluginsAsync\(IEnumerable<Type\>, CancellationToken\)

Instantiates and registers each of the supplied plugin types as live plugins.
Types implementing <xref href="Void.Proxy.Api.Plugins.IApiPlugin" data-throw-if-not-resolved="false"></xref> are loaded before all other plugin types.

```csharp
ValueTask LoadPluginsAsync(IEnumerable<Type> plugins, CancellationToken cancellationToken = default)
```

#### Parameters

`plugins` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Type](https://learn.microsoft.com/dotnet/api/system.type)\>

The plugin types to load.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

### <a id="Void_Proxy_Api_Plugins_IPluginService_UnloadContainerAsync_System_String_System_Threading_CancellationToken_"></a> UnloadContainerAsync\(string, CancellationToken\)

Fires <code>PluginUnloadingEvent</code> and <code>PluginUnloadedEvent</code> for every plugin in the named
container, cancels the container's cancellation token, initiates assembly unload, and then forces
garbage collection until the weak reference is collected or a 10-second timeout expires.

```csharp
ValueTask UnloadContainerAsync(string name, CancellationToken cancellationToken = default)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

The name of the container to unload, as returned by <xref href="Void.Proxy.Api.Plugins.IPluginService.Containers" data-throw-if-not-resolved="false"></xref>.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

#### Exceptions

 [Exception](https://learn.microsoft.com/dotnet/api/system.exception)

Thrown when the container is not found, is already unloaded, or refuses to unload within the timeout.

### <a id="Void_Proxy_Api_Plugins_IPluginService_UnloadContainersAsync_System_Threading_CancellationToken_"></a> UnloadContainersAsync\(CancellationToken\)

Unloads all active plugin containers, firing unload events for every plugin in each container.

```csharp
ValueTask UnloadContainersAsync(CancellationToken cancellationToken = default)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

