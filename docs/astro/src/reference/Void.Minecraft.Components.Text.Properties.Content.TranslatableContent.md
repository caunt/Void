# <a id="Void_Minecraft_Components_Text_Properties_Content_TranslatableContent"></a> Class TranslatableContent

Namespace: [Void.Minecraft.Components.Text.Properties.Content](Void.Minecraft.Components.Text.Properties.Content.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record TranslatableContent : IContent, IEquatable<TranslatableContent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TranslatableContent](Void.Minecraft.Components.Text.Properties.Content.TranslatableContent.md)

#### Implements

[IContent](Void.Minecraft.Components.Text.Properties.Content.IContent.md), 
[IEquatable<TranslatableContent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Properties_Content_TranslatableContent__ctor_System_String_System_String_System_Collections_Generic_IEnumerable_Void_Minecraft_Components_Text_Component__"></a> TranslatableContent\(string, string?, IEnumerable<Component\>?\)

```csharp
public TranslatableContent(string Translate, string? Fallback = null, IEnumerable<Component>? With = null)
```

#### Parameters

`Translate` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Fallback` [string](https://learn.microsoft.com/dotnet/api/system.string)?

`With` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Component](Void.Minecraft.Components.Text.Component.md)\>?

## Properties

### <a id="Void_Minecraft_Components_Text_Properties_Content_TranslatableContent_Fallback"></a> Fallback

```csharp
public string? Fallback { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Minecraft_Components_Text_Properties_Content_TranslatableContent_Translate"></a> Translate

```csharp
public string Translate { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Properties_Content_TranslatableContent_Type"></a> Type

```csharp
public string Type { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Properties_Content_TranslatableContent_With"></a> With

```csharp
public IEnumerable<Component>? With { get; init; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Component](Void.Minecraft.Components.Text.Component.md)\>?

