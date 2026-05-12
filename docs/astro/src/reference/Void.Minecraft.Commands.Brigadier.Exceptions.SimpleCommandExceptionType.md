# <a id="Void_Minecraft_Commands_Brigadier_Exceptions_SimpleCommandExceptionType"></a> Class SimpleCommandExceptionType

Namespace: [Void.Minecraft.Commands.Brigadier.Exceptions](Void.Minecraft.Commands.Brigadier.Exceptions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class SimpleCommandExceptionType : ICommandExceptionType
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[SimpleCommandExceptionType](Void.Minecraft.Commands.Brigadier.Exceptions.SimpleCommandExceptionType.md)

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

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_SimpleCommandExceptionType__ctor_Void_Minecraft_Commands_Brigadier_IMessage_"></a> SimpleCommandExceptionType\(IMessage\)

```csharp
public SimpleCommandExceptionType(IMessage message)
```

#### Parameters

`message` [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_SimpleCommandExceptionType_Create"></a> Create\(\)

```csharp
public CommandSyntaxException Create()
```

#### Returns

 [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_SimpleCommandExceptionType_CreateWithContext_Void_Minecraft_Commands_Brigadier_IImmutableStringReader_"></a> CreateWithContext\(IImmutableStringReader\)

```csharp
public CommandSyntaxException CreateWithContext(IImmutableStringReader reader)
```

#### Parameters

`reader` [IImmutableStringReader](Void.Minecraft.Commands.Brigadier.IImmutableStringReader.md)

#### Returns

 [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_SimpleCommandExceptionType_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

