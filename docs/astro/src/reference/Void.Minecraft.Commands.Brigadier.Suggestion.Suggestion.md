# <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestion"></a> Class Suggestion

Namespace: [Void.Minecraft.Commands.Brigadier.Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record Suggestion : IComparable<Suggestion>, IEquatable<Suggestion>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md)

#### Derived

[IntegerSuggestion](Void.Minecraft.Commands.Brigadier.Suggestion.IntegerSuggestion.md)

#### Implements

[IComparable<Suggestion\>](https://learn.microsoft.com/dotnet/api/system.icomparable\-1), 
[IEquatable<Suggestion\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestion__ctor_Void_Minecraft_Commands_Brigadier_Context_StringRange_System_String_Void_Minecraft_Commands_Brigadier_IMessage_"></a> Suggestion\(StringRange, string, IMessage?\)

```csharp
public Suggestion(StringRange Range, string Text, IMessage? Tooltip = null)
```

#### Parameters

`Range` [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

`Text` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Tooltip` [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)?

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestion_Range"></a> Range

```csharp
public StringRange Range { get; init; }
```

#### Property Value

 [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestion_Text"></a> Text

```csharp
public string Text { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestion_Tooltip"></a> Tooltip

```csharp
public IMessage? Tooltip { get; init; }
```

#### Property Value

 [IMessage](Void.Minecraft.Commands.Brigadier.IMessage.md)?

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestion_Apply_System_String_"></a> Apply\(string\)

```csharp
public string Apply(string source)
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestion_CompareTo_Void_Minecraft_Commands_Brigadier_Suggestion_Suggestion_"></a> CompareTo\(Suggestion?\)

Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.

```csharp
public int CompareTo(Suggestion? other)
```

#### Parameters

`other` [Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md)?

An object to compare with this instance.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A value that indicates the relative order of the objects being compared. The return value has these meanings:

 <table><thead><tr><th class="term"> Value</th><th class="description"> Meaning</th></tr></thead><tbody><tr><td class="term"> Less than zero</td><td class="description"> This instance precedes <code class="paramref">other</code> in the sort order.</td></tr><tr><td class="term"> Zero</td><td class="description"> This instance occurs in the same position in the sort order as <code class="paramref">other</code>.</td></tr><tr><td class="term"> Greater than zero</td><td class="description"> This instance follows <code class="paramref">other</code> in the sort order.</td></tr></tbody></table>

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestion_Expand_System_String_Void_Minecraft_Commands_Brigadier_Context_StringRange_"></a> Expand\(string, StringRange\)

```csharp
public Suggestion Expand(string source, StringRange range)
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

`range` [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

#### Returns

 [Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md)

