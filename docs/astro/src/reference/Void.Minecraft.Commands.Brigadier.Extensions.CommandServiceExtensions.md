# <a id="Void_Minecraft_Commands_Brigadier_Extensions_CommandServiceExtensions"></a> Class CommandServiceExtensions

Namespace: [Void.Minecraft.Commands.Brigadier.Extensions](Void.Minecraft.Commands.Brigadier.Extensions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class CommandServiceExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CommandServiceExtensions](Void.Minecraft.Commands.Brigadier.Extensions.CommandServiceExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Extensions_CommandServiceExtensions_Register_Void_Proxy_Api_Commands_ICommandService_System_Func_Void_Minecraft_Commands_Brigadier_IArgumentContext_Void_Minecraft_Commands_Brigadier_Builder_LiteralArgumentBuilder__"></a> Register\(ICommandService, Func<IArgumentContext, LiteralArgumentBuilder\>\)

```csharp
public static void Register(this ICommandService commands, Func<IArgumentContext, LiteralArgumentBuilder> configure)
```

#### Parameters

`commands` [ICommandService](Void.Proxy.Api.Commands.ICommandService.md)

`configure` [Func](https://learn.microsoft.com/dotnet/api/system.func\-2)<[IArgumentContext](Void.Minecraft.Commands.Brigadier.IArgumentContext.md), [LiteralArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.LiteralArgumentBuilder.md)\>

