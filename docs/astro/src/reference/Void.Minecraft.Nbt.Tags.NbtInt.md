# <a id="Void_Minecraft_Nbt_Tags_NbtInt"></a> Class NbtInt

Namespace: [Void.Minecraft.Nbt.Tags](Void.Minecraft.Nbt.Tags.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record NbtInt : NbtTag, IEquatable<NbtTag>, IEquatable<NbtInt>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtTag](Void.Minecraft.Nbt.NbtTag.md) ← 
[NbtInt](Void.Minecraft.Nbt.Tags.NbtInt.md)

#### Implements

[IEquatable<NbtTag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IEquatable<NbtInt\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

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

### <a id="Void_Minecraft_Nbt_Tags_NbtInt__ctor_System_Int32_"></a> NbtInt\(int\)

```csharp
public NbtInt(int Value)
```

#### Parameters

`Value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Void_Minecraft_Nbt_Tags_NbtInt_Value"></a> Value

```csharp
public int Value { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Void_Minecraft_Nbt_Tags_NbtInt_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

## Operators

### <a id="Void_Minecraft_Nbt_Tags_NbtInt_op_Implicit_Void_Minecraft_Nbt_IntTag__Void_Minecraft_Nbt_Tags_NbtInt"></a> implicit operator NbtInt\(IntTag\)

```csharp
public static implicit operator NbtInt(IntTag tag)
```

#### Parameters

`tag` [IntTag](Void.Minecraft.Nbt.IntTag.md)

#### Returns

 [NbtInt](Void.Minecraft.Nbt.Tags.NbtInt.md)

### <a id="Void_Minecraft_Nbt_Tags_NbtInt_op_Implicit_Void_Minecraft_Nbt_Tags_NbtInt__Void_Minecraft_Nbt_IntTag"></a> implicit operator IntTag\(NbtInt\)

```csharp
public static implicit operator IntTag(NbtInt tag)
```

#### Parameters

`tag` [NbtInt](Void.Minecraft.Nbt.Tags.NbtInt.md)

#### Returns

 [IntTag](Void.Minecraft.Nbt.IntTag.md)

