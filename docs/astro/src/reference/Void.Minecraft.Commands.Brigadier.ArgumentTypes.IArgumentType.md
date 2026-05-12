# <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentType"></a> Interface IArgumentType

Namespace: [Void.Minecraft.Commands.Brigadier.ArgumentTypes](Void.Minecraft.Commands.Brigadier.ArgumentTypes.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IArgumentType : IAnyArgumentType
```

#### Implements

[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentType_Examples"></a> Examples

```csharp
IEnumerable<string> Examples { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentType_As__1"></a> As<T\>\(\)

```csharp
T As<T>() where T : IAnyArgumentType
```

#### Returns

 T

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentType_ListSuggestionsAsync_Void_Minecraft_Commands_Brigadier_Context_CommandContext_Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_System_Threading_CancellationToken_"></a> ListSuggestionsAsync\(CommandContext, SuggestionsBuilder, CancellationToken\)

```csharp
ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`builder` [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentType_Parse_Void_Minecraft_Commands_Brigadier_StringReader_"></a> Parse\(StringReader\)

```csharp
IArgumentValue Parse(StringReader reader)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

#### Returns

 [IArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentValue.md)

