# <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowText"></a> Class ShowText

Namespace: [Void.Minecraft.Components.Text.Events.Actions.Hover](Void.Minecraft.Components.Text.Events.Actions.Hover.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ShowText : IHoverEventAction, IEquatable<ShowText>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ShowText](Void.Minecraft.Components.Text.Events.Actions.Hover.ShowText.md)

#### Implements

[IHoverEventAction](Void.Minecraft.Components.Text.Events.Actions.IHoverEventAction.md), 
[IEquatable<ShowText\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowText__ctor_Void_Minecraft_Components_Text_Component_"></a> ShowText\(Component\)

```csharp
public ShowText(Component Value)
```

#### Parameters

`Value` [Component](Void.Minecraft.Components.Text.Component.md)

## Properties

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowText_ActionName"></a> ActionName

```csharp
public string ActionName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Events_Actions_Hover_ShowText_Value"></a> Value

```csharp
public Component Value { get; init; }
```

#### Property Value

 [Component](Void.Minecraft.Components.Text.Component.md)

