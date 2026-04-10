# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NbtProperty"></a> Class NbtProperty

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record NbtProperty : IPacketProperty<NbtProperty>, IPacketProperty, IEquatable<NbtProperty>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtProperty](Void.Minecraft.Network.Registries.Transformations.Properties.NbtProperty.md)

#### Implements

[IPacketProperty<NbtProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<NbtProperty\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NbtProperty__ctor_System_ReadOnlyMemory_System_Byte__"></a> NbtProperty\(ReadOnlyMemory<byte\>\)

```csharp
public NbtProperty(ReadOnlyMemory<byte> Value)
```

#### Parameters

`Value` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NbtProperty_AsNbtTag"></a> AsNbtTag

```csharp
public NbtTag AsNbtTag { get; }
```

#### Property Value

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NbtProperty_Value"></a> Value

```csharp
public ReadOnlyMemory<byte> Value { get; init; }
```

#### Property Value

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NbtProperty_FromNbtTag_Void_Minecraft_Nbt_NbtTag_"></a> FromNbtTag\(NbtTag\)

```csharp
public static NbtProperty FromNbtTag(NbtTag value)
```

#### Parameters

`value` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NbtProperty](Void.Minecraft.Network.Registries.Transformations.Properties.NbtProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NbtProperty_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static NbtProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [NbtProperty](Void.Minecraft.Network.Registries.Transformations.Properties.NbtProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NbtProperty_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

