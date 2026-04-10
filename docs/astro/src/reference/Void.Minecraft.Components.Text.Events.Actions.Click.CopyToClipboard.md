# <a id="Void_Minecraft_Components_Text_Events_Actions_Click_CopyToClipboard"></a> Class CopyToClipboard

Namespace: [Void.Minecraft.Components.Text.Events.Actions.Click](Void.Minecraft.Components.Text.Events.Actions.Click.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record CopyToClipboard : IClickEventAction, IEquatable<CopyToClipboard>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CopyToClipboard](Void.Minecraft.Components.Text.Events.Actions.Click.CopyToClipboard.md)

#### Implements

[IClickEventAction](Void.Minecraft.Components.Text.Events.Actions.IClickEventAction.md), 
[IEquatable<CopyToClipboard\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_CopyToClipboard__ctor_System_String_"></a> CopyToClipboard\(string\)

```csharp
public CopyToClipboard(string Value)
```

#### Parameters

`Value` [string](https://learn.microsoft.com/dotnet/api/system.string)

## Properties

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_CopyToClipboard_ActionName"></a> ActionName

```csharp
public string ActionName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_CopyToClipboard_Value"></a> Value

```csharp
public string Value { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

