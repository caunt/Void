# <a id="Void_Minecraft_Components_Text_Events_Actions_Click_OpenFile"></a> Class OpenFile

Namespace: [Void.Minecraft.Components.Text.Events.Actions.Click](Void.Minecraft.Components.Text.Events.Actions.Click.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record OpenFile : IClickEventAction, IEquatable<OpenFile>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[OpenFile](Void.Minecraft.Components.Text.Events.Actions.Click.OpenFile.md)

#### Implements

[IClickEventAction](Void.Minecraft.Components.Text.Events.Actions.IClickEventAction.md), 
[IEquatable<OpenFile\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_OpenFile__ctor_System_String_"></a> OpenFile\(string\)

```csharp
public OpenFile(string File)
```

#### Parameters

`File` [string](https://learn.microsoft.com/dotnet/api/system.string)

## Properties

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_OpenFile_ActionName"></a> ActionName

```csharp
public string ActionName { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Events_Actions_Click_OpenFile_File"></a> File

```csharp
public string File { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

