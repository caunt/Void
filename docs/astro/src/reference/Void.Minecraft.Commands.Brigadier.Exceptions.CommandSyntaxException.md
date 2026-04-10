# <a id="Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException"></a> Class CommandSyntaxException

Namespace: [Void.Minecraft.Commands.Brigadier.Exceptions](Void.Minecraft.Commands.Brigadier.Exceptions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class CommandSyntaxException : Exception, ISerializable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Exception](https://learn.microsoft.com/dotnet/api/system.exception) ← 
[CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)

#### Implements

[ISerializable](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)

#### Inherited Members

[Exception.GetBaseException\(\)](https://learn.microsoft.com/dotnet/api/system.exception.getbaseexception), 
[Exception.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.exception.gettype), 
[Exception.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.exception.tostring), 
[Exception.Data](https://learn.microsoft.com/dotnet/api/system.exception.data), 
[Exception.HelpLink](https://learn.microsoft.com/dotnet/api/system.exception.helplink), 
[Exception.HResult](https://learn.microsoft.com/dotnet/api/system.exception.hresult), 
[Exception.InnerException](https://learn.microsoft.com/dotnet/api/system.exception.innerexception), 
[Exception.Message](https://learn.microsoft.com/dotnet/api/system.exception.message), 
[Exception.Source](https://learn.microsoft.com/dotnet/api/system.exception.source), 
[Exception.StackTrace](https://learn.microsoft.com/dotnet/api/system.exception.stacktrace), 
[Exception.TargetSite](https://learn.microsoft.com/dotnet/api/system.exception.targetsite), 
[Exception.SerializeObjectState](https://learn.microsoft.com/dotnet/api/system.exception.serializeobjectstate), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException__ctor_Void_Minecraft_Commands_Brigadier_Exceptions_ICommandExceptionType_Void_Minecraft_Commands_Brigadier_IMessage_System_String_System_Int32_"></a> CommandSyntaxException\(ICommandExceptionType, IMessage, string, int\)

```csharp
public CommandSyntaxException(ICommandExceptionType type, IMessage message, string input = "", int cursor = -1)
```

#### Parameters

`type` [ICommandExceptionType](Void.Minecraft.Commands.Brigadier.Exceptions.ICommandExceptionType.md)

`message` [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)

`input` [string](https://learn.microsoft.com/dotnet/api/system.string)

`cursor` [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Fields

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException_ContextAmount"></a> ContextAmount

```csharp
public const int ContextAmount = 10
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException_BuiltInExceptions"></a> BuiltInExceptions

```csharp
public static IBuiltInExceptionProvider BuiltInExceptions { get; set; }
```

#### Property Value

 [IBuiltInExceptionProvider](Void.Minecraft.Commands.Brigadier.Exceptions.IBuiltInExceptionProvider.md)

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException_Cursor"></a> Cursor

```csharp
public int Cursor { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException_Input"></a> Input

```csharp
public string Input { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException_Message"></a> Message

Gets a message that describes the current exception.

```csharp
public override string Message { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException_RawMessage"></a> RawMessage

```csharp
public IMessage RawMessage { get; }
```

#### Property Value

 [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException_Type"></a> Type

```csharp
public ICommandExceptionType Type { get; }
```

#### Property Value

 [ICommandExceptionType](Void.Minecraft.Commands.Brigadier.Exceptions.ICommandExceptionType.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException_GetContext"></a> GetContext\(\)

```csharp
public string? GetContext()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException_GetMessage"></a> GetMessage\(\)

```csharp
public string GetMessage()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

