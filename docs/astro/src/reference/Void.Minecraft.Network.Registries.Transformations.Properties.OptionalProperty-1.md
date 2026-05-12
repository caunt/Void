# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_OptionalProperty_1"></a> Class OptionalProperty<TPacketProperty\>

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record OptionalProperty<TPacketProperty> : IPacketProperty<OptionalProperty<TPacketProperty>>, IPacketProperty, IEquatable<OptionalProperty<TPacketProperty>> where TPacketProperty : class, IPacketProperty<TPacketProperty>
```

#### Type Parameters

`TPacketProperty` 

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[OptionalProperty<TPacketProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.OptionalProperty\-1.md)

#### Implements

[IPacketProperty<OptionalProperty<TPacketProperty\>\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<OptionalProperty<TPacketProperty\>\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_OptionalProperty_1__ctor__0_"></a> OptionalProperty\(TPacketProperty?\)

```csharp
public OptionalProperty(TPacketProperty? Value = null)
```

#### Parameters

`Value` TPacketProperty?

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_OptionalProperty_1_Value"></a> Value

```csharp
public TPacketProperty? Value { get; init; }
```

#### Property Value

 TPacketProperty?

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_OptionalProperty_1_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static OptionalProperty<TPacketProperty> Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [OptionalProperty](Void.Minecraft.Network.Registries.Transformations.Properties.OptionalProperty\-1.md)<TPacketProperty\>

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_OptionalProperty_1_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

