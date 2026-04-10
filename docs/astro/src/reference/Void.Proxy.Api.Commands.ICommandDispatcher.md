# <a id="Void_Proxy_Api_Commands_ICommandDispatcher"></a> Interface ICommandDispatcher

Namespace: [Void.Proxy.Api.Commands](Void.Proxy.Api.Commands.md)  
Assembly: Void.Proxy.Api.dll  

Manages the Brigadier command tree by registering top-level command nodes.
The concrete implementation is <code>CommandDispatcher</code>, which roots all nodes under a single <code>RootCommandNode</code>.

```csharp
public interface ICommandDispatcher
```

## Methods

### <a id="Void_Proxy_Api_Commands_ICommandDispatcher_Add_Void_Proxy_Api_Commands_ICommandNode_"></a> Add\(ICommandNode\)

Registers <code class="paramref">node</code> as a direct child of the root command node,
making it available for dispatch and tab-completion.

```csharp
void Add(ICommandNode node)
```

#### Parameters

`node` [ICommandNode](Void.Proxy.Api.Commands.ICommandNode.md)

The command node to add. Must be a concrete <code>CommandNode</code> instance;
any other implementation throws <xref href="System.ArgumentException" data-throw-if-not-resolved="false"></xref>.

