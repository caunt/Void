# <a id="Void_Minecraft_Nbt_Tags_NbtCompound"></a> Class NbtCompound

Namespace: [Void.Minecraft.Nbt.Tags](Void.Minecraft.Nbt.Tags.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record NbtCompound : NbtTag, IEquatable<NbtTag>, IEquatable<NbtCompound>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtTag](Void.Minecraft.Nbt.NbtTag.md) ← 
[NbtCompound](Void.Minecraft.Nbt.Tags.NbtCompound.md)

#### Implements

[IEquatable<NbtTag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IEquatable<NbtCompound\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[NbtTag.Name](Void.Minecraft.Nbt.NbtTag.md\#Void\_Minecraft\_Nbt\_NbtTag\_Name), 
[NbtTag.Type](Void.Minecraft.Nbt.NbtTag.md\#Void\_Minecraft\_Nbt\_NbtTag\_Type), 
[NbtTag.AsStream\(NbtFormatOptions, bool\)](Void.Minecraft.Nbt.NbtTag.md\#Void\_Minecraft\_Nbt\_NbtTag\_AsStream\_Void\_Minecraft\_Nbt\_NbtFormatOptions\_System\_Boolean\_), 
[NbtTag.AsJsonNode\(\)](Void.Minecraft.Nbt.NbtTag.md\#Void\_Minecraft\_Nbt\_NbtTag\_AsJsonNode), 
[NbtTag.ToString\(\)](Void.Minecraft.Nbt.NbtTag.md\#Void\_Minecraft\_Nbt\_NbtTag\_ToString), 
[NbtTag.ToSnbt\(\)](Void.Minecraft.Nbt.NbtTag.md\#Void\_Minecraft\_Nbt\_NbtTag\_ToSnbt), 
[NbtTag.Parse\(string\)](Void.Minecraft.Nbt.NbtTag.md\#Void\_Minecraft\_Nbt\_NbtTag\_Parse\_System\_String\_), 
[NbtTag.Parse\(ReadOnlyMemory<byte\>, out NbtTag, bool, NbtFormatOptions\)](Void.Minecraft.Nbt.NbtTag.md\#Void\_Minecraft\_Nbt\_NbtTag\_Parse\_System\_ReadOnlyMemory\_System\_Byte\_\_Void\_Minecraft\_Nbt\_NbtTag\_\_System\_Boolean\_Void\_Minecraft\_Nbt\_NbtFormatOptions\_), 
[NbtTag.Parse<T\>\(ReadOnlyMemory<byte\>, out T, bool, NbtFormatOptions\)](Void.Minecraft.Nbt.NbtTag.md\#Void\_Minecraft\_Nbt\_NbtTag\_Parse\_\_1\_System\_ReadOnlyMemory\_System\_Byte\_\_\_\_0\_\_System\_Boolean\_Void\_Minecraft\_Nbt\_NbtFormatOptions\_), 
[NbtTag.ReadFrom<TBuffer\>\(ref TBuffer, bool\)](Void.Minecraft.Nbt.NbtTag.md\#Void\_Minecraft\_Nbt\_NbtTag\_ReadFrom\_\_1\_\_\_0\_\_System\_Boolean\_), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Nbt_Tags_NbtCompound__ctor_System_Collections_Generic_Dictionary_System_String_Void_Minecraft_Nbt_NbtTag__"></a> NbtCompound\(Dictionary<string, NbtTag\>\)

```csharp
public NbtCompound(Dictionary<string, NbtTag> Fields)
```

#### Parameters

`Fields` [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [NbtTag](Void.Minecraft.Nbt.NbtTag.md)\>

### <a id="Void_Minecraft_Nbt_Tags_NbtCompound__ctor"></a> NbtCompound\(\)

```csharp
public NbtCompound()
```

## Properties

### <a id="Void_Minecraft_Nbt_Tags_NbtCompound_Fields"></a> Fields

```csharp
public Dictionary<string, NbtTag> Fields { get; init; }
```

#### Property Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [NbtTag](Void.Minecraft.Nbt.NbtTag.md)\>

### <a id="Void_Minecraft_Nbt_Tags_NbtCompound_Item_System_String_"></a> this\[string\]

```csharp
public NbtTag? this[string name] { get; set; }
```

#### Property Value

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)?

## Methods

### <a id="Void_Minecraft_Nbt_Tags_NbtCompound_ContainsKey_System_String_"></a> ContainsKey\(string\)

```csharp
public bool ContainsKey(string name)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Nbt_Tags_NbtCompound_RenameKey_System_String_System_String_"></a> RenameKey\(string, string\)

```csharp
public void RenameKey(string name, string newName)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`newName` [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Nbt_Tags_NbtCompound_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

### <a id="Void_Minecraft_Nbt_Tags_NbtCompound_TryGetValue_System_String_Void_Minecraft_Nbt_NbtTag__"></a> TryGetValue\(string, out NbtTag\)

```csharp
public bool TryGetValue(string name, out NbtTag value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`value` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Nbt_Tags_NbtCompound_TryRenameKey_System_String_System_String_"></a> TryRenameKey\(string, string\)

```csharp
public bool TryRenameKey(string name, string newName)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`newName` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Operators

### <a id="Void_Minecraft_Nbt_Tags_NbtCompound_op_Implicit_Void_Minecraft_Nbt_CompoundTag__Void_Minecraft_Nbt_Tags_NbtCompound"></a> implicit operator NbtCompound\(CompoundTag\)

```csharp
public static implicit operator NbtCompound(CompoundTag tag)
```

#### Parameters

`tag` [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

#### Returns

 [NbtCompound](Void.Minecraft.Nbt.Tags.NbtCompound.md)

### <a id="Void_Minecraft_Nbt_Tags_NbtCompound_op_Implicit_Void_Minecraft_Nbt_Tags_NbtCompound__Void_Minecraft_Nbt_CompoundTag"></a> implicit operator CompoundTag\(NbtCompound\)

```csharp
public static implicit operator CompoundTag(NbtCompound tag)
```

#### Parameters

`tag` [NbtCompound](Void.Minecraft.Nbt.Tags.NbtCompound.md)

#### Returns

 [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

