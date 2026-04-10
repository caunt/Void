# <a id="Void_Minecraft_Commands_Brigadier_Exceptions_DynamicCommandExceptionType"></a> Class DynamicCommandExceptionType

Namespace: [Void.Minecraft.Commands.Brigadier.Exceptions](Void.Minecraft.Commands.Brigadier.Exceptions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class DynamicCommandExceptionType : ICommandExceptionType
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[DynamicCommandExceptionType](Void.Minecraft.Commands.Brigadier.Exceptions.DynamicCommandExceptionType.md)

#### Implements

[ICommandExceptionType](Void.Minecraft.Commands.Brigadier.Exceptions.ICommandExceptionType.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_DynamicCommandExceptionType__ctor_System_Func_System_Object_Void_Minecraft_Commands_Brigadier_IMessage__"></a> DynamicCommandExceptionType\(Func<object, IMessage\>\)

```csharp
public DynamicCommandExceptionType(Func<object, IMessage> function)
```

#### Parameters

`function` [Func](https://learn.microsoft.com/dotnet/api/system.func\-2)<[object](https://learn.microsoft.com/dotnet/api/system.object), [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)\>

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_DynamicCommandExceptionType_Create_System_Object_"></a> Create\(object\)

```csharp
public CommandSyntaxException Create(object value)
```

#### Parameters

`value` [object](https://learn.microsoft.com/dotnet/api/system.object)

#### Returns

 [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_DynamicCommandExceptionType_CreateWithContext_Void_Minecraft_Commands_Brigadier_StringReader_System_Object_"></a> CreateWithContext\(StringReader, object\)

```csharp
public CommandSyntaxException CreateWithContext(StringReader reader, object value)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

`value` [object](https://learn.microsoft.com/dotnet/api/system.object)

#### Returns

 [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)

