# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_FloatProperty"></a> Class FloatProperty

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record FloatProperty : IPacketProperty<FloatProperty>, IPacketProperty, IEquatable<FloatProperty>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[FloatProperty](Void.Minecraft.Network.Registries.Transformations.Properties.FloatProperty.md)

#### Implements

[IPacketProperty<FloatProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<FloatProperty\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_FloatProperty__ctor_System_ReadOnlyMemory_System_Byte__"></a> FloatProperty\(ReadOnlyMemory<byte\>\)

```csharp
public FloatProperty(ReadOnlyMemory<byte> Value)
```

#### Parameters

`Value` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_FloatProperty_AsPrimitive"></a> AsPrimitive

```csharp
public float AsPrimitive { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_FloatProperty_Value"></a> Value

```csharp
public ReadOnlyMemory<byte> Value { get; init; }
```

#### Property Value

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_FloatProperty_FromPrimitive_System_Single_"></a> FromPrimitive\(float\)

```csharp
public static FloatProperty FromPrimitive(float value)
```

#### Parameters

`value` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [FloatProperty](Void.Minecraft.Network.Registries.Transformations.Properties.FloatProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_FloatProperty_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static FloatProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [FloatProperty](Void.Minecraft.Network.Registries.Transformations.Properties.FloatProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_FloatProperty_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

