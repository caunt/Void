# <a id="Void_Minecraft_Nbt_Tags_NbtList"></a> Class NbtList

Namespace: [Void.Minecraft.Nbt.Tags](Void.Minecraft.Nbt.Tags.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record NbtList : NbtTag, IEquatable<NbtTag>, IEquatable<NbtList>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtTag](Void.Minecraft.Nbt.NbtTag.md) ← 
[NbtList](Void.Minecraft.Nbt.Tags.NbtList.md)

#### Implements

[IEquatable<NbtTag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IEquatable<NbtList\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

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

### <a id="Void_Minecraft_Nbt_Tags_NbtList__ctor_System_Collections_Generic_IEnumerable_Void_Minecraft_Nbt_NbtTag__Void_Minecraft_Nbt_NbtTagType_"></a> NbtList\(IEnumerable<NbtTag\>, NbtTagType\)

```csharp
public NbtList(IEnumerable<NbtTag> Data, NbtTagType DataType)
```

#### Parameters

`Data` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[NbtTag](Void.Minecraft.Nbt.NbtTag.md)\>

`DataType` [NbtTagType](Void.Minecraft.Nbt.NbtTagType.md)

## Properties

### <a id="Void_Minecraft_Nbt_Tags_NbtList_Data"></a> Data

```csharp
public IEnumerable<NbtTag> Data { get; init; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[NbtTag](Void.Minecraft.Nbt.NbtTag.md)\>

### <a id="Void_Minecraft_Nbt_Tags_NbtList_DataType"></a> DataType

```csharp
public NbtTagType DataType { get; init; }
```

#### Property Value

 [NbtTagType](Void.Minecraft.Nbt.NbtTagType.md)

## Methods

### <a id="Void_Minecraft_Nbt_Tags_NbtList_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

## Operators

### <a id="Void_Minecraft_Nbt_Tags_NbtList_op_Implicit_Void_Minecraft_Nbt_ListTag__Void_Minecraft_Nbt_Tags_NbtList"></a> implicit operator NbtList\(ListTag\)

```csharp
public static implicit operator NbtList(ListTag tag)
```

#### Parameters

`tag` [ListTag](Void.Minecraft.Nbt.ListTag.md)

#### Returns

 [NbtList](Void.Minecraft.Nbt.Tags.NbtList.md)

### <a id="Void_Minecraft_Nbt_Tags_NbtList_op_Implicit_Void_Minecraft_Nbt_Tags_NbtList__Void_Minecraft_Nbt_ListTag"></a> implicit operator ListTag\(NbtList\)

```csharp
public static implicit operator ListTag(NbtList tag)
```

#### Parameters

`tag` [NbtList](Void.Minecraft.Nbt.Tags.NbtList.md)

#### Returns

 [ListTag](Void.Minecraft.Nbt.ListTag.md)

