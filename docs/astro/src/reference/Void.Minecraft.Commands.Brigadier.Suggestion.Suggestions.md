# <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestions"></a> Class Suggestions

Namespace: [Void.Minecraft.Commands.Brigadier.Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record Suggestions : IEquatable<Suggestions>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)

#### Implements

[IEquatable<Suggestions\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestions__ctor_Void_Minecraft_Commands_Brigadier_Context_StringRange_System_Collections_Generic_List_Void_Minecraft_Commands_Brigadier_Suggestion_Suggestion__"></a> Suggestions\(StringRange, List<Suggestion\>\)

```csharp
public Suggestions(StringRange Range, List<Suggestion> All)
```

#### Parameters

`Range` [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

`All` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md)\>

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestions_All"></a> All

```csharp
public List<Suggestion> All { get; init; }
```

#### Property Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestions_Empty"></a> Empty

```csharp
public static Suggestions Empty { get; }
```

#### Property Value

 [Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestions_IsEmpty"></a> IsEmpty

```csharp
public bool IsEmpty { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestions_Range"></a> Range

```csharp
public StringRange Range { get; init; }
```

#### Property Value

 [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestions_Create_System_String_System_Collections_Generic_IEnumerable_Void_Minecraft_Commands_Brigadier_Suggestion_Suggestion__"></a> Create\(string, IEnumerable<Suggestion\>\)

```csharp
public static Suggestions Create(string command, IEnumerable<Suggestion> suggestions)
```

#### Parameters

`command` [string](https://learn.microsoft.com/dotnet/api/system.string)

`suggestions` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Suggestion](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestion.md)\>

#### Returns

 [Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestions_EmptyAsync"></a> EmptyAsync\(\)

```csharp
public static ValueTask<Suggestions> EmptyAsync()
```

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Suggestion_Suggestions_Merge_System_String_System_Collections_Generic_IEnumerable_Void_Minecraft_Commands_Brigadier_Suggestion_Suggestions__"></a> Merge\(string, IEnumerable<Suggestions\>\)

```csharp
public static Suggestions Merge(string command, IEnumerable<Suggestions> input)
```

#### Parameters

`command` [string](https://learn.microsoft.com/dotnet/api/system.string)

`input` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)\>

#### Returns

 [Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)

