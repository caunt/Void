# <a id="Void_Minecraft_Commands_Brigadier_Exceptions_Dynamic3CommandExceptionType"></a> Class Dynamic3CommandExceptionType

Namespace: [Void.Minecraft.Commands.Brigadier.Exceptions](Void.Minecraft.Commands.Brigadier.Exceptions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class Dynamic3CommandExceptionType : ICommandExceptionType
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Dynamic3CommandExceptionType](Void.Minecraft.Commands.Brigadier.Exceptions.Dynamic3CommandExceptionType.md)

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

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_Dynamic3CommandExceptionType__ctor_System_Func_System_Object_System_Object_System_Object_Void_Minecraft_Commands_Brigadier_IMessage__"></a> Dynamic3CommandExceptionType\(Func<object, object, object, IMessage\>\)

```csharp
public Dynamic3CommandExceptionType(Func<object, object, object, IMessage> function)
```

#### Parameters

`function` [Func](https://learn.microsoft.com/dotnet/api/system.func\-4)<[object](https://learn.microsoft.com/dotnet/api/system.object), [object](https://learn.microsoft.com/dotnet/api/system.object), [object](https://learn.microsoft.com/dotnet/api/system.object), [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)\>

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_Dynamic3CommandExceptionType_Create_System_Object_System_Object_System_Object_"></a> Create\(object, object, object\)

```csharp
public CommandSyntaxException Create(object a, object b, object c)
```

#### Parameters

`a` [object](https://learn.microsoft.com/dotnet/api/system.object)

`b` [object](https://learn.microsoft.com/dotnet/api/system.object)

`c` [object](https://learn.microsoft.com/dotnet/api/system.object)

#### Returns

 [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_Dynamic3CommandExceptionType_CreateWithContext_Void_Minecraft_Commands_Brigadier_IImmutableStringReader_System_Object_System_Object_System_Object_"></a> CreateWithContext\(IImmutableStringReader, object, object, object\)

```csharp
public CommandSyntaxException CreateWithContext(IImmutableStringReader reader, object a, object b, object c)
```

#### Parameters

`reader` [IImmutableStringReader](Void.Minecraft.Commands.Brigadier.IImmutableStringReader.md)

`a` [object](https://learn.microsoft.com/dotnet/api/system.object)

`b` [object](https://learn.microsoft.com/dotnet/api/system.object)

`c` [object](https://learn.microsoft.com/dotnet/api/system.object)

#### Returns

 [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)

