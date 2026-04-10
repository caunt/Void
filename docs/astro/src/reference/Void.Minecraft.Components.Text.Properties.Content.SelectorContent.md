# <a id="Void_Minecraft_Components_Text_Properties_Content_SelectorContent"></a> Class SelectorContent

Namespace: [Void.Minecraft.Components.Text.Properties.Content](Void.Minecraft.Components.Text.Properties.Content.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record SelectorContent : IContent, IEquatable<SelectorContent>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[SelectorContent](Void.Minecraft.Components.Text.Properties.Content.SelectorContent.md)

#### Implements

[IContent](Void.Minecraft.Components.Text.Properties.Content.IContent.md), 
[IEquatable<SelectorContent\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Properties_Content_SelectorContent__ctor_System_String_Void_Minecraft_Components_Text_Component_"></a> SelectorContent\(string, Component?\)

```csharp
public SelectorContent(string Value, Component? Separator = null)
```

#### Parameters

`Value` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Separator` [Component](Void.Minecraft.Components.Text.Component.md)?

## Properties

### <a id="Void_Minecraft_Components_Text_Properties_Content_SelectorContent_Separator"></a> Separator

```csharp
public Component? Separator { get; init; }
```

#### Property Value

 [Component](Void.Minecraft.Components.Text.Component.md)?

### <a id="Void_Minecraft_Components_Text_Properties_Content_SelectorContent_Type"></a> Type

```csharp
public string Type { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Properties_Content_SelectorContent_Value"></a> Value

```csharp
public string Value { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

