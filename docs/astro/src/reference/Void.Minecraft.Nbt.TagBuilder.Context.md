# <a id="Void_Minecraft_Nbt_TagBuilder_Context"></a> Class TagBuilder.Context

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Represents the context of a single "level" of depth into a <xref href="Void.Minecraft.Nbt.TagBuilder" data-throw-if-not-resolved="false"></xref> AST.

```csharp
public class TagBuilder.Context : IDisposable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TagBuilder.Context](Void.Minecraft.Nbt.TagBuilder.Context.md)

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Remarks

Implements <xref href="System.IDisposable" data-throw-if-not-resolved="false"></xref> to that each node can used with <code>using</code> statements for easily distinguishable scope.

## Properties

### <a id="Void_Minecraft_Nbt_TagBuilder_Context_Tag"></a> Tag

Gets the top-level tag for this context.

```csharp
public ICollection<Tag> Tag { get; }
```

#### Property Value

 [ICollection](https://learn.microsoft.com/dotnet/api/system.collections.generic.icollection\-1)<[Tag](Void.Minecraft.Nbt.Tag.md)\>

## Methods

### <a id="Void_Minecraft_Nbt_TagBuilder_Context_Dispose"></a> Dispose\(\)

Closes this context.

```csharp
public void Dispose()
```

