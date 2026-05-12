# <a id="Void_Minecraft_Nbt_Tags_NbtLongArray"></a> Class NbtLongArray

Namespace: [Void.Minecraft.Nbt.Tags](Void.Minecraft.Nbt.Tags.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record NbtLongArray : NbtTag, IEquatable<NbtTag>, IEquatable<NbtLongArray>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtTag](Void.Minecraft.Nbt.NbtTag.md) ← 
[NbtLongArray](Void.Minecraft.Nbt.Tags.NbtLongArray.md)

#### Implements

[IEquatable<NbtTag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IEquatable<NbtLongArray\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

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

### <a id="Void_Minecraft_Nbt_Tags_NbtLongArray__ctor_System_Int64___"></a> NbtLongArray\(long\[\]\)

```csharp
public NbtLongArray(long[] Data)
```

#### Parameters

`Data` [long](https://learn.microsoft.com/dotnet/api/system.int64)\[\]

## Properties

### <a id="Void_Minecraft_Nbt_Tags_NbtLongArray_Data"></a> Data

```csharp
public long[] Data { get; init; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)\[\]

## Methods

### <a id="Void_Minecraft_Nbt_Tags_NbtLongArray_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

## Operators

### <a id="Void_Minecraft_Nbt_Tags_NbtLongArray_op_Implicit_Void_Minecraft_Nbt_LongArrayTag__Void_Minecraft_Nbt_Tags_NbtLongArray"></a> implicit operator NbtLongArray\(LongArrayTag\)

```csharp
public static implicit operator NbtLongArray(LongArrayTag tag)
```

#### Parameters

`tag` [LongArrayTag](Void.Minecraft.Nbt.LongArrayTag.md)

#### Returns

 [NbtLongArray](Void.Minecraft.Nbt.Tags.NbtLongArray.md)

### <a id="Void_Minecraft_Nbt_Tags_NbtLongArray_op_Implicit_Void_Minecraft_Nbt_Tags_NbtLongArray__Void_Minecraft_Nbt_LongArrayTag"></a> implicit operator LongArrayTag\(NbtLongArray\)

```csharp
public static implicit operator LongArrayTag(NbtLongArray tag)
```

#### Parameters

`tag` [NbtLongArray](Void.Minecraft.Nbt.Tags.NbtLongArray.md)

#### Returns

 [LongArrayTag](Void.Minecraft.Nbt.LongArrayTag.md)

