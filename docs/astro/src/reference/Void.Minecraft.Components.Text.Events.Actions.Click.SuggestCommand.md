# <a id="Void_Minecraft_Components_Text_Events_Actions_Click_SuggestCommand"></a> Class SuggestCommand

Namespace: [Void.Minecraft.Components.Text.Events.Actions.Click](Void.Minecraft.Components.Text.Events.Actions.Click.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record SuggestCommand : IClickEventAction, IEquatable<SuggestCommand>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[SuggestCommand](Void.Minecraft.Components.Text.Events.Actions.Click.SuggestCommand.md)

#### Implements

[IClickEventAction](Void.Minecraft.Components.Text.Events.Actions.IClickEventAction.md), 
[IEquatable<SuggestCommand\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_SuggestCommand__ctor_System_String_"></a> SuggestCommand\(string\)

```csharp
public SuggestCommand(string Command)
```

#### Parameters

`Command` [string](https://learn.microsoft.com/dotnet/api/system.string)

## Properties

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_SuggestCommand_ActionName"></a> ActionName

```csharp
public string ActionName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_SuggestCommand_Command"></a> Command

```csharp
public string Command { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

