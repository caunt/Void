# <a id="Void_Minecraft_Components_Text_Events_Actions_Click_ChangePage"></a> Class ChangePage

Namespace: [Void.Minecraft.Components.Text.Events.Actions.Click](Void.Minecraft.Components.Text.Events.Actions.Click.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ChangePage : IClickEventAction, IEquatable<ChangePage>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ChangePage](Void.Minecraft.Components.Text.Events.Actions.Click.ChangePage.md)

#### Implements

[IClickEventAction](Void.Minecraft.Components.Text.Events.Actions.IClickEventAction.md), 
[IEquatable<ChangePage\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_ChangePage__ctor_System_Int32_"></a> ChangePage\(int\)

```csharp
public ChangePage(int Page)
```

#### Parameters

`Page` [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_ChangePage_ActionName"></a> ActionName

```csharp
public string ActionName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_ChangePage_Page"></a> Page

```csharp
public int Page { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

