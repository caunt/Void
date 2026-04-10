# <a id="Void_Minecraft_Nbt_Tags_NbtIntArray"></a> Class NbtIntArray

Namespace: [Void.Minecraft.Nbt.Tags](Void.Minecraft.Nbt.Tags.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record NbtIntArray : NbtTag, IEquatable<NbtTag>, IEquatable<NbtIntArray>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtTag](Void.Minecraft.Nbt.NbtTag.md) ← 
[NbtIntArray](Void.Minecraft.Nbt.Tags.NbtIntArray.md)

#### Implements

[IEquatable<NbtTag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IEquatable<NbtIntArray\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

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

### <a id="Void_Minecraft_Nbt_Tags_NbtIntArray__ctor_System_Int32___"></a> NbtIntArray\(int\[\]\)

```csharp
public NbtIntArray(int[] Data)
```

#### Parameters

`Data` [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]

## Properties

### <a id="Void_Minecraft_Nbt_Tags_NbtIntArray_Data"></a> Data

```csharp
public int[] Data { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]

## Methods

### <a id="Void_Minecraft_Nbt_Tags_NbtIntArray_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

## Operators

### <a id="Void_Minecraft_Nbt_Tags_NbtIntArray_op_Implicit_Void_Minecraft_Nbt_IntArrayTag__Void_Minecraft_Nbt_Tags_NbtIntArray"></a> implicit operator NbtIntArray\(IntArrayTag\)

```csharp
public static implicit operator NbtIntArray(IntArrayTag tag)
```

#### Parameters

`tag` [IntArrayTag](Void.Minecraft.Nbt.IntArrayTag.md)

#### Returns

 [NbtIntArray](Void.Minecraft.Nbt.Tags.NbtIntArray.md)

### <a id="Void_Minecraft_Nbt_Tags_NbtIntArray_op_Implicit_Void_Minecraft_Nbt_Tags_NbtIntArray__Void_Minecraft_Nbt_IntArrayTag"></a> implicit operator IntArrayTag\(NbtIntArray\)

```csharp
public static implicit operator IntArrayTag(NbtIntArray tag)
```

#### Parameters

`tag` [NbtIntArray](Void.Minecraft.Nbt.Tags.NbtIntArray.md)

#### Returns

 [IntArrayTag](Void.Minecraft.Nbt.IntArrayTag.md)

