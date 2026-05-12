# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ByteProperty"></a> Class ByteProperty

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ByteProperty : IPacketProperty<ByteProperty>, IPacketProperty, IEquatable<ByteProperty>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ByteProperty](Void.Minecraft.Network.Registries.Transformations.Properties.ByteProperty.md)

#### Implements

[IPacketProperty<ByteProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<ByteProperty\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ByteProperty__ctor_System_ReadOnlyMemory_System_Byte__"></a> ByteProperty\(ReadOnlyMemory<byte\>\)

```csharp
public ByteProperty(ReadOnlyMemory<byte> Value)
```

#### Parameters

`Value` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ByteProperty_AsPrimitive"></a> AsPrimitive

```csharp
public byte AsPrimitive { get; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ByteProperty_Value"></a> Value

```csharp
public ReadOnlyMemory<byte> Value { get; init; }
```

#### Property Value

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ByteProperty_FromPrimitive_System_Byte_"></a> FromPrimitive\(byte\)

```csharp
public static ByteProperty FromPrimitive(byte value)
```

#### Parameters

`value` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

#### Returns

 [ByteProperty](Void.Minecraft.Network.Registries.Transformations.Properties.ByteProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ByteProperty_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static ByteProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [ByteProperty](Void.Minecraft.Network.Registries.Transformations.Properties.ByteProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ByteProperty_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

