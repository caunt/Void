# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_VarLongProperty"></a> Class VarLongProperty

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record VarLongProperty : IPacketProperty<VarLongProperty>, IPacketProperty, IEquatable<VarLongProperty>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[VarLongProperty](Void.Minecraft.Network.Registries.Transformations.Properties.VarLongProperty.md)

#### Implements

[IPacketProperty<VarLongProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<VarLongProperty\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_VarLongProperty__ctor_System_ReadOnlyMemory_System_Byte__"></a> VarLongProperty\(ReadOnlyMemory<byte\>\)

```csharp
public VarLongProperty(ReadOnlyMemory<byte> Value)
```

#### Parameters

`Value` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_VarLongProperty_AsPrimitive"></a> AsPrimitive

```csharp
public long AsPrimitive { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_VarLongProperty_Value"></a> Value

```csharp
public ReadOnlyMemory<byte> Value { get; init; }
```

#### Property Value

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_VarLongProperty_FromPrimitive_System_Int64_"></a> FromPrimitive\(long\)

```csharp
public static VarLongProperty FromPrimitive(long value)
```

#### Parameters

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

#### Returns

 [VarLongProperty](Void.Minecraft.Network.Registries.Transformations.Properties.VarLongProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_VarLongProperty_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static VarLongProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [VarLongProperty](Void.Minecraft.Network.Registries.Transformations.Properties.VarLongProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_VarLongProperty_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

