# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_UuidProperty"></a> Class UuidProperty

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record UuidProperty : IPacketProperty<UuidProperty>, IPacketProperty, IEquatable<UuidProperty>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[UuidProperty](Void.Minecraft.Network.Registries.Transformations.Properties.UuidProperty.md)

#### Implements

[IPacketProperty<UuidProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<UuidProperty\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_UuidProperty__ctor_System_ReadOnlyMemory_System_Byte__"></a> UuidProperty\(ReadOnlyMemory<byte\>\)

```csharp
public UuidProperty(ReadOnlyMemory<byte> Value)
```

#### Parameters

`Value` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_UuidProperty_AsUuid"></a> AsUuid

```csharp
public Uuid AsUuid { get; }
```

#### Property Value

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_UuidProperty_Empty"></a> Empty

```csharp
public static UuidProperty Empty { get; }
```

#### Property Value

 [UuidProperty](Void.Minecraft.Network.Registries.Transformations.Properties.UuidProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_UuidProperty_Value"></a> Value

```csharp
public ReadOnlyMemory<byte> Value { get; init; }
```

#### Property Value

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_UuidProperty_FromUuid_Void_Minecraft_Profiles_Uuid_"></a> FromUuid\(Uuid\)

```csharp
public static UuidProperty FromUuid(Uuid value)
```

#### Parameters

`value` [Uuid](Void.Minecraft.Profiles.Uuid.md)

#### Returns

 [UuidProperty](Void.Minecraft.Network.Registries.Transformations.Properties.UuidProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_UuidProperty_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static UuidProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [UuidProperty](Void.Minecraft.Network.Registries.Transformations.Properties.UuidProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_UuidProperty_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

