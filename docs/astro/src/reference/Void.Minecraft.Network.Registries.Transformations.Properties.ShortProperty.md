# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ShortProperty"></a> Class ShortProperty

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ShortProperty : IPacketProperty<ShortProperty>, IPacketProperty, IEquatable<ShortProperty>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ShortProperty](Void.Minecraft.Network.Registries.Transformations.Properties.ShortProperty.md)

#### Implements

[IPacketProperty<ShortProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<ShortProperty\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ShortProperty__ctor_System_ReadOnlyMemory_System_Byte__"></a> ShortProperty\(ReadOnlyMemory<byte\>\)

```csharp
public ShortProperty(ReadOnlyMemory<byte> Value)
```

#### Parameters

`Value` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ShortProperty_AsPrimitive"></a> AsPrimitive

```csharp
public short AsPrimitive { get; }
```

#### Property Value

 [short](https://learn.microsoft.com/dotnet/api/system.int16)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ShortProperty_Value"></a> Value

```csharp
public ReadOnlyMemory<byte> Value { get; init; }
```

#### Property Value

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ShortProperty_FromPrimitive_System_Int16_"></a> FromPrimitive\(short\)

```csharp
public static ShortProperty FromPrimitive(short value)
```

#### Parameters

`value` [short](https://learn.microsoft.com/dotnet/api/system.int16)

#### Returns

 [ShortProperty](Void.Minecraft.Network.Registries.Transformations.Properties.ShortProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ShortProperty_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static ShortProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [ShortProperty](Void.Minecraft.Network.Registries.Transformations.Properties.ShortProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ShortProperty_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

