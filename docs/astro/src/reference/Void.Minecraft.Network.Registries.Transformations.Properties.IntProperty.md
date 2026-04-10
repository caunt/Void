# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_IntProperty"></a> Class IntProperty

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record IntProperty : IPacketProperty<IntProperty>, IPacketProperty, IEquatable<IntProperty>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[IntProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IntProperty.md)

#### Implements

[IPacketProperty<IntProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<IntProperty\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_IntProperty__ctor_System_ReadOnlyMemory_System_Byte__"></a> IntProperty\(ReadOnlyMemory<byte\>\)

```csharp
public IntProperty(ReadOnlyMemory<byte> Value)
```

#### Parameters

`Value` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_IntProperty_AsPrimitive"></a> AsPrimitive

```csharp
public int AsPrimitive { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_IntProperty_Value"></a> Value

```csharp
public ReadOnlyMemory<byte> Value { get; init; }
```

#### Property Value

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_IntProperty_FromPrimitive_System_Int32_"></a> FromPrimitive\(int\)

```csharp
public static IntProperty FromPrimitive(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [IntProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IntProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_IntProperty_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static IntProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [IntProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IntProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_IntProperty_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

