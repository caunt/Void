# <a id="Void_Proxy_Api_Plugins"></a> Namespace Void.Proxy.Api.Plugins

### Namespaces

 [Void.Proxy.Api.Plugins.Dependencies](Void.Proxy.Api.Plugins.Dependencies.md)

 [Void.Proxy.Api.Plugins.Extensions](Void.Proxy.Api.Plugins.Extensions.md)

### Interfaces

 [IApiPlugin](Void.Proxy.Api.Plugins.IApiPlugin.md)

 [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

Represents a proxy plugin. Plugins extend proxy functionality by subscribing to events
and are identified by a human-readable name.

 [IPluginService](Void.Proxy.Api.Plugins.IPluginService.md)

Manages the lifecycle of proxy plugins, including discovery, loading, and unloading
of plugin containers backed by isolated <xref href="System.Runtime.Loader.AssemblyLoadContext" data-throw-if-not-resolved="false"></xref> instances.

