# <a id="Void_Minecraft_Commands_Brigadier_Exceptions_Dynamic2CommandExceptionType"></a> Class Dynamic2CommandExceptionType

Namespace: [Void.Minecraft.Commands.Brigadier.Exceptions](Void.Minecraft.Commands.Brigadier.Exceptions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class Dynamic2CommandExceptionType : ICommandExceptionType
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Dynamic2CommandExceptionType](Void.Minecraft.Commands.Brigadier.Exceptions.Dynamic2CommandExceptionType.md)

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

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_Dynamic2CommandExceptionType__ctor_System_Func_System_Object_System_Object_Void_Minecraft_Commands_Brigadier_IMessage__"></a> Dynamic2CommandExceptionType\(Func<object, object, IMessage\>\)

```csharp
public Dynamic2CommandExceptionType(Func<object, object, IMessage> function)
```

#### Parameters

`function` [Func](https://learn.microsoft.com/dotnet/api/system.func\-3)<[object](https://learn.microsoft.com/dotnet/api/system.object), [object](https://learn.microsoft.com/dotnet/api/system.object), [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)\>

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_Dynamic2CommandExceptionType_Create_System_Object_System_Object_"></a> Create\(object, object\)

```csharp
public CommandSyntaxException Create(object a, object b)
```

#### Parameters

`a` [object](https://learn.microsoft.com/dotnet/api/system.object)

`b` [object](https://learn.microsoft.com/dotnet/api/system.object)

#### Returns

 [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_Dynamic2CommandExceptionType_CreateWithContext_Void_Minecraft_Commands_Brigadier_IImmutableStringReader_System_Object_System_Object_"></a> CreateWithContext\(IImmutableStringReader, object, object\)

```csharp
public CommandSyntaxException CreateWithContext(IImmutableStringReader reader, object a, object b)
```

#### Parameters

`reader` [IImmutableStringReader](Void.Minecraft.Commands.Brigadier.IImmutableStringReader.md)

`a` [object](https://learn.microsoft.com/dotnet/api/system.object)

`b` [object](https://learn.microsoft.com/dotnet/api/system.object)

#### Returns

 [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)

