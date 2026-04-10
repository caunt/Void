# <a id="Void_Minecraft_Commands_Brigadier_Exceptions_DynamicNCommandExceptionType"></a> Class DynamicNCommandExceptionType

Namespace: [Void.Minecraft.Commands.Brigadier.Exceptions](Void.Minecraft.Commands.Brigadier.Exceptions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class DynamicNCommandExceptionType : ICommandExceptionType
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[DynamicNCommandExceptionType](Void.Minecraft.Commands.Brigadier.Exceptions.DynamicNCommandExceptionType.md)

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

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_DynamicNCommandExceptionType__ctor_System_Func_System_Object___Void_Minecraft_Commands_Brigadier_IMessage__"></a> DynamicNCommandExceptionType\(Func<object\[\], IMessage\>\)

```csharp
public DynamicNCommandExceptionType(Func<object[], IMessage> function)
```

#### Parameters

`function` [Func](https://learn.microsoft.com/dotnet/api/system.func\-2)<[object](https://learn.microsoft.com/dotnet/api/system.object)\[\], [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)\>

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_DynamicNCommandExceptionType_Create_System_Object___"></a> Create\(params object\[\]\)

```csharp
public CommandSyntaxException Create(params object[] objects)
```

#### Parameters

`objects` [object](https://learn.microsoft.com/dotnet/api/system.object)\[\]

#### Returns

 [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_DynamicNCommandExceptionType_CreateWithContext_Void_Minecraft_Commands_Brigadier_IImmutableStringReader_System_Object___"></a> CreateWithContext\(IImmutableStringReader, params object\[\]\)

```csharp
public CommandSyntaxException CreateWithContext(IImmutableStringReader reader, params object[] objects)
```

#### Parameters

`reader` [IImmutableStringReader](Void.Minecraft.Commands.Brigadier.IImmutableStringReader.md)

`objects` [object](https://learn.microsoft.com/dotnet/api/system.object)\[\]

#### Returns

 [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)

