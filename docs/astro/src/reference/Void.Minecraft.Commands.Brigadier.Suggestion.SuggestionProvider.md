# <a id="Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionProvider"></a> Delegate SuggestionProvider

Namespace: [Void.Minecraft.Commands.Brigadier.Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.md)  
Assembly: Void.Minecraft.dll  

```csharp
public delegate ValueTask<Suggestions> SuggestionProvider(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`builder` [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)\>

