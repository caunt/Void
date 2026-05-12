# <a id="Void_Minecraft_Commands_Brigadier_Suggestion_IntegerSuggestion"></a> Class IntegerSuggestion

Namespace: [Void.Minecraft.Commands.Brigadier.Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record IntegerSuggestion : Suggestion, IComparable<Suggestion>, IEquatable<Suggestion>, IEquatable<IntegerSuggestion>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md) ← 
[IntegerSuggestion](Void.Minecraft.Commands.Brigadier.Suggestion.IntegerSuggestion.md)

#### Implements

[IComparable<Suggestion\>](https://learn.microsoft.com/dotnet/api/system.icomparable\-1), 
[IEquatable<Suggestion\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IEquatable<IntegerSuggestion\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[Suggestion.Range](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md\#Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_Suggestion\_Range), 
[Suggestion.Text](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md\#Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_Suggestion\_Text), 
[Suggestion.Tooltip](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md\#Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_Suggestion\_Tooltip), 
[Suggestion.Apply\(string\)](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md\#Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_Suggestion\_Apply\_System\_String\_), 
[Suggestion.CompareTo\(Suggestion?\)](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md\#Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_Suggestion\_CompareTo\_Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_Suggestion\_), 
[Suggestion.Expand\(string, StringRange\)](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md\#Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_Suggestion\_Expand\_System\_String\_Void\_Minecraft\_Commands\_Brigadier\_Context\_StringRange\_), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_IntegerSuggestion__ctor_Void_Minecraft_Commands_Brigadier_Context_StringRange_System_Int32_Void_Minecraft_Commands_Brigadier_IMessage_"></a> IntegerSuggestion\(StringRange, int, IMessage?\)

```csharp
public IntegerSuggestion(StringRange Range, int Value, IMessage? Tooltip = null)
```

#### Parameters

`Range` [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

`Value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`Tooltip` [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)?

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_IntegerSuggestion_Value"></a> Value

```csharp
public int Value { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

