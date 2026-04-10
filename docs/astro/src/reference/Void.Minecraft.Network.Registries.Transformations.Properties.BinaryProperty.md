# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_BinaryProperty"></a> Class BinaryProperty

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record BinaryProperty : IPacketProperty<BinaryProperty>, IPacketProperty, IEquatable<BinaryProperty>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BinaryProperty](Void.Minecraft.Network.Registries.Transformations.Properties.BinaryProperty.md)

#### Implements

[IPacketProperty<BinaryProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<BinaryProperty\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_BinaryProperty__ctor_System_ReadOnlyMemory_System_Byte__"></a> BinaryProperty\(ReadOnlyMemory<byte\>\)

```csharp
public BinaryProperty(ReadOnlyMemory<byte> Value)
```

#### Parameters

`Value` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_BinaryProperty_AsSpan"></a> AsSpan

```csharp
public ReadOnlySpan<byte> AsSpan { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_BinaryProperty_Value"></a> Value

```csharp
public ReadOnlyMemory<byte> Value { get; init; }
```

#### Property Value

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_BinaryProperty_FromStream_System_IO_MemoryStream_"></a> FromStream\(MemoryStream\)

```csharp
public static BinaryProperty FromStream(MemoryStream value)
```

#### Parameters

`value` [MemoryStream](https://learn.microsoft.com/dotnet/api/system.io.memorystream)

#### Returns

 [BinaryProperty](Void.Minecraft.Network.Registries.Transformations.Properties.BinaryProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_BinaryProperty_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static BinaryProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [BinaryProperty](Void.Minecraft.Network.Registries.Transformations.Properties.BinaryProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_BinaryProperty_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

