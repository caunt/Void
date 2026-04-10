# <a id="Void_Minecraft_Nbt_TagEventArgs"></a> Class TagEventArgs

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Arguments supplied with tag-related events.

```csharp
public class TagEventArgs : EventArgs
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs) ← 
[TagEventArgs](Void.Minecraft.Nbt.TagEventArgs.md)

#### Inherited Members

[EventArgs.Empty](https://learn.microsoft.com/dotnet/api/system.eventargs.empty), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Nbt_TagEventArgs__ctor_Void_Minecraft_Nbt_TagType_Void_Minecraft_Nbt_Tag_"></a> TagEventArgs\(TagType, Tag\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.TagEventArgs" data-throw-if-not-resolved="false"></xref> class.

```csharp
public TagEventArgs(TagType type, Tag tag)
```

#### Parameters

`type` [TagType](Void.Minecraft.Nbt.TagType.md)

A constant describing the basic NBT type of the tag.

`tag` [Tag](Void.Minecraft.Nbt.Tag.md)

The parsed <xref href="Void.Minecraft.Nbt.TagEventArgs.Tag" data-throw-if-not-resolved="false"></xref> instance.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

Thrown when <code class="paramref">tag</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

## Properties

### <a id="Void_Minecraft_Nbt_TagEventArgs_Tag"></a> Tag

Gets the parsed <xref href="Void.Minecraft.Nbt.TagEventArgs.Tag" data-throw-if-not-resolved="false"></xref> instance.

```csharp
public Tag Tag { get; }
```

#### Property Value

 [Tag](Void.Minecraft.Nbt.Tag.md)

### <a id="Void_Minecraft_Nbt_TagEventArgs_Type"></a> Type

Gets a constant describing the basic NBT type of the tag.

```csharp
public TagType Type { get; }
```

#### Property Value

 [TagType](Void.Minecraft.Nbt.TagType.md)

