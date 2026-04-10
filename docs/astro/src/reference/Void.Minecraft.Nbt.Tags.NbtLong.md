# <a id="Void_Minecraft_Nbt_Tags_NbtLong"></a> Class NbtLong

Namespace: [Void.Minecraft.Nbt.Tags](Void.Minecraft.Nbt.Tags.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record NbtLong : NbtTag, IEquatable<NbtTag>, IEquatable<NbtLong>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtTag](Void.Minecraft.Nbt.NbtTag.md) ← 
[NbtLong](Void.Minecraft.Nbt.Tags.NbtLong.md)

#### Implements

[IEquatable<NbtTag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IEquatable<NbtLong\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

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

### <a id="Void_Minecraft_Nbt_Tags_NbtLong__ctor_System_Int64_"></a> NbtLong\(long\)

```csharp
public NbtLong(long Value)
```

#### Parameters

`Value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Properties

### <a id="Void_Minecraft_Nbt_Tags_NbtLong_Value"></a> Value

```csharp
public long Value { get; init; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Void_Minecraft_Nbt_Tags_NbtLong_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

## Operators

### <a id="Void_Minecraft_Nbt_Tags_NbtLong_op_Implicit_Void_Minecraft_Nbt_LongTag__Void_Minecraft_Nbt_Tags_NbtLong"></a> implicit operator NbtLong\(LongTag\)

```csharp
public static implicit operator NbtLong(LongTag tag)
```

#### Parameters

`tag` [LongTag](Void.Minecraft.Nbt.LongTag.md)

#### Returns

 [NbtLong](Void.Minecraft.Nbt.Tags.NbtLong.md)

### <a id="Void_Minecraft_Nbt_Tags_NbtLong_op_Implicit_Void_Minecraft_Nbt_Tags_NbtLong__Void_Minecraft_Nbt_LongTag"></a> implicit operator LongTag\(NbtLong\)

```csharp
public static implicit operator LongTag(NbtLong tag)
```

#### Parameters

`tag` [NbtLong](Void.Minecraft.Nbt.Tags.NbtLong.md)

#### Returns

 [LongTag](Void.Minecraft.Nbt.LongTag.md)

