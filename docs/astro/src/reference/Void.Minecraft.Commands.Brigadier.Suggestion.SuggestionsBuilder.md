# <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder"></a> Class SuggestionsBuilder

Namespace: [Void.Minecraft.Commands.Brigadier.Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class SuggestionsBuilder
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder__ctor_System_String_System_Int32_"></a> SuggestionsBuilder\(string, int\)

```csharp
public SuggestionsBuilder(string Input, int Start)
```

#### Parameters

`Input` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Start` [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_InputLowerCase"></a> InputLowerCase

```csharp
public string InputLowerCase { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_Remaining"></a> Remaining

```csharp
public string Remaining { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_RemainingLowerCase"></a> RemainingLowerCase

```csharp
public string RemainingLowerCase { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_Add_Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_"></a> Add\(SuggestionsBuilder\)

```csharp
public SuggestionsBuilder Add(SuggestionsBuilder other)
```

#### Parameters

`other` [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

#### Returns

 [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_Build"></a> Build\(\)

```csharp
public Suggestions Build()
```

#### Returns

 [Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_BuildAsync_System_Threading_CancellationToken_"></a> BuildAsync\(CancellationToken\)

```csharp
public ValueTask<Suggestions> BuildAsync(CancellationToken _)
```

#### Parameters

`_` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_CreateOffset_System_Int32_"></a> CreateOffset\(int\)

```csharp
public SuggestionsBuilder CreateOffset(int start)
```

#### Parameters

`start` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_Restart"></a> Restart\(\)

```csharp
public SuggestionsBuilder Restart()
```

#### Returns

 [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_Suggest_System_String_"></a> Suggest\(string\)

```csharp
public SuggestionsBuilder Suggest(string text)
```

#### Parameters

`text` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_Suggest_System_String_Void_Minecraft_Commands_Brigadier_IMessage_"></a> Suggest\(string, IMessage\)

```csharp
public SuggestionsBuilder Suggest(string text, IMessage tooltip)
```

#### Parameters

`text` [string](https://learn.microsoft.com/dotnet/api/system.string)

`tooltip` [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)

#### Returns

 [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_Suggest_System_Int32_"></a> Suggest\(int\)

```csharp
public SuggestionsBuilder Suggest(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_Suggest_System_Int32_Void_Minecraft_Commands_Brigadier_IMessage_"></a> Suggest\(int, IMessage\)

```csharp
public SuggestionsBuilder Suggest(int value, IMessage tooltip)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`tooltip` [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)

#### Returns

 [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

