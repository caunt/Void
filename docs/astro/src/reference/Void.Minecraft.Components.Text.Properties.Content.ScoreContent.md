# <a id="Void_Minecraft_Components_Text_Properties_Content_ScoreContent"></a> Class ScoreContent

Namespace: [Void.Minecraft.Components.Text.Properties.Content](Void.Minecraft.Components.Text.Properties.Content.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ScoreContent : IContent, IEquatable<ScoreContent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ScoreContent](Void.Minecraft.Components.Text.Properties.Content.ScoreContent.md)

#### Implements

[IContent](Void.Minecraft.Components.Text.Properties.Content.IContent.md), 
[IEquatable<ScoreContent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Properties_Content_ScoreContent__ctor_System_String_System_String_"></a> ScoreContent\(string, string\)

```csharp
public ScoreContent(string Name, string Objective)
```

#### Parameters

`Name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Objective` [string](https://learn.microsoft.com/dotnet/api/system.string)

## Properties

### <a id="Void_Minecraft_Components_Text_Properties_Content_ScoreContent_Name"></a> Name

```csharp
public string Name { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Properties_Content_ScoreContent_Objective"></a> Objective

```csharp
public string Objective { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Properties_Content_ScoreContent_Type"></a> Type

```csharp
public string Type { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

