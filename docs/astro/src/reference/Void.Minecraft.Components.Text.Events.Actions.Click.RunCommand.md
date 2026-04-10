# <a id="Void_Minecraft_Components_Text_Events_Actions_Click_RunCommand"></a> Class RunCommand

Namespace: [Void.Minecraft.Components.Text.Events.Actions.Click](Void.Minecraft.Components.Text.Events.Actions.Click.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record RunCommand : IClickEventAction, IEquatable<RunCommand>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[RunCommand](Void.Minecraft.Components.Text.Events.Actions.Click.RunCommand.md)

#### Implements

[IClickEventAction](Void.Minecraft.Components.Text.Events.Actions.IClickEventAction.md), 
[IEquatable<RunCommand\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_RunCommand__ctor_System_String_"></a> RunCommand\(string\)

```csharp
public RunCommand(string Command)
```

#### Parameters

`Command` [string](https://learn.microsoft.com/dotnet/api/system.string)

## Properties

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_RunCommand_ActionName"></a> ActionName

```csharp
public string ActionName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_RunCommand_Command"></a> Command

```csharp
public string Command { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

