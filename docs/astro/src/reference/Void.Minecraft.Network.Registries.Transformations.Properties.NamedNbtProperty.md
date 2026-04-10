# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NamedNbtProperty"></a> Class NamedNbtProperty

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record NamedNbtProperty : IPacketProperty<NamedNbtProperty>, IPacketProperty, IEquatable<NamedNbtProperty>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NamedNbtProperty](Void.Minecraft.Network.Registries.Transformations.Properties.NamedNbtProperty.md)

#### Implements

[IPacketProperty<NamedNbtProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<NamedNbtProperty\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NamedNbtProperty__ctor_System_ReadOnlyMemory_System_Byte__"></a> NamedNbtProperty\(ReadOnlyMemory<byte\>\)

```csharp
public NamedNbtProperty(ReadOnlyMemory<byte> Value)
```

#### Parameters

`Value` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NamedNbtProperty_AsNbtTag"></a> AsNbtTag

```csharp
public NbtTag AsNbtTag { get; }
```

#### Property Value

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NamedNbtProperty_Value"></a> Value

```csharp
public ReadOnlyMemory<byte> Value { get; init; }
```

#### Property Value

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NamedNbtProperty_FromNbtTag_Void_Minecraft_Nbt_NbtTag_"></a> FromNbtTag\(NbtTag\)

```csharp
public static NamedNbtProperty FromNbtTag(NbtTag value)
```

#### Parameters

`value` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NamedNbtProperty](Void.Minecraft.Network.Registries.Transformations.Properties.NamedNbtProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NamedNbtProperty_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static NamedNbtProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [NamedNbtProperty](Void.Minecraft.Network.Registries.Transformations.Properties.NamedNbtProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_NamedNbtProperty_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

