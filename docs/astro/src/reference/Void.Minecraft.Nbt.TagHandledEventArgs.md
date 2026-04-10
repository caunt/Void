# <a id="Void_Minecraft_Nbt_TagHandledEventArgs"></a> Class TagHandledEventArgs

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Arguments supplied when an event that can be handled by an event subscriber.

```csharp
public class TagHandledEventArgs : HandledEventArgs
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs) ← 
[HandledEventArgs](https://learn.microsoft.com/dotnet/api/system.componentmodel.handledeventargs) ← 
[TagHandledEventArgs](Void.Minecraft.Nbt.TagHandledEventArgs.md)

#### Inherited Members

[HandledEventArgs.Handled](https://learn.microsoft.com/dotnet/api/system.componentmodel.handledeventargs.handled), 
[EventArgs.Empty](https://learn.microsoft.com/dotnet/api/system.eventargs.empty), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Nbt_TagHandledEventArgs__ctor_Void_Minecraft_Nbt_TagType_System_Boolean_System_IO_Stream_"></a> TagHandledEventArgs\(TagType, bool, Stream\)

Creates a new instance of the <xref href="Void.Minecraft.Nbt.TagHandledEventArgs" data-throw-if-not-resolved="false"></xref> class.

```csharp
public TagHandledEventArgs(TagType type, bool isNamed, Stream stream)
```

#### Parameters

`type` [TagType](Void.Minecraft.Nbt.TagType.md)

A constant describing the basic NBT type of the tag.

`isNamed` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

The stream being read from, positioned at the beginning of the tag payload.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

Thrown when <code class="paramref">stream</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

## Properties

### <a id="Void_Minecraft_Nbt_TagHandledEventArgs_IsNamed"></a> IsNamed

Gets flag indicating if this tag is named, only <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> when a tag is a direct child of a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public bool IsNamed { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Nbt_TagHandledEventArgs_Result"></a> Result

Gets or sets the resulting tag from this event being handled.

```csharp
public Tag? Result { get; set; }
```

#### Property Value

 [Tag](Void.Minecraft.Nbt.Tag.md)?

#### Remarks

This property <b>must</b> set to a non-null value when <xref href="System.ComponentModel.HandledEventArgs.Handled" data-throw-if-not-resolved="false"></xref> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>.

### <a id="Void_Minecraft_Nbt_TagHandledEventArgs_Stream"></a> Stream

Gets the stream being read from, positioned at the beginning of the tag payload.

<p></p>

When handling this event, the stream position must be moved to the end of the payload, ready for the next tag to be parsed.

```csharp
public Stream Stream { get; }
```

#### Property Value

 [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

### <a id="Void_Minecraft_Nbt_TagHandledEventArgs_Type"></a> Type

Gets a constant describing the basic NBT type of the tag.

```csharp
public TagType Type { get; }
```

#### Property Value

 [TagType](Void.Minecraft.Nbt.TagType.md)

