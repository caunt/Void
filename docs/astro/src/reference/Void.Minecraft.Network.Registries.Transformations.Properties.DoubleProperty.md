# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_DoubleProperty"></a> Class DoubleProperty

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record DoubleProperty : IPacketProperty<DoubleProperty>, IPacketProperty, IEquatable<DoubleProperty>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[DoubleProperty](Void.Minecraft.Network.Registries.Transformations.Properties.DoubleProperty.md)

#### Implements

[IPacketProperty<DoubleProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<DoubleProperty\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_DoubleProperty__ctor_System_ReadOnlyMemory_System_Byte__"></a> DoubleProperty\(ReadOnlyMemory<byte\>\)

```csharp
public DoubleProperty(ReadOnlyMemory<byte> Value)
```

#### Parameters

`Value` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_DoubleProperty_AsPrimitive"></a> AsPrimitive

```csharp
public double AsPrimitive { get; }
```

#### Property Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_DoubleProperty_Value"></a> Value

```csharp
public ReadOnlyMemory<byte> Value { get; init; }
```

#### Property Value

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_DoubleProperty_FromPrimitive_System_Double_"></a> FromPrimitive\(double\)

```csharp
public static DoubleProperty FromPrimitive(double value)
```

#### Parameters

`value` [double](https://learn.microsoft.com/dotnet/api/system.double)

#### Returns

 [DoubleProperty](Void.Minecraft.Network.Registries.Transformations.Properties.DoubleProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_DoubleProperty_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static DoubleProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [DoubleProperty](Void.Minecraft.Network.Registries.Transformations.Properties.DoubleProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_DoubleProperty_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

