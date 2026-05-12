# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ListProperty_1"></a> Class ListProperty<TPacketProperty\>

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ListProperty<TPacketProperty> : IPacketProperty<ListProperty<TPacketProperty>>, IPacketProperty, IEquatable<ListProperty<TPacketProperty>> where TPacketProperty : IPacketProperty<TPacketProperty>
```

#### Type Parameters

`TPacketProperty` 

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ListProperty<TPacketProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.ListProperty\-1.md)

#### Implements

[IPacketProperty<ListProperty<TPacketProperty\>\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<ListProperty<TPacketProperty\>\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ListProperty_1__ctor_System_Collections_Generic_List__0__"></a> ListProperty\(List<TPacketProperty\>\)

```csharp
public ListProperty(List<TPacketProperty> Values)
```

#### Parameters

`Values` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<TPacketProperty\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ListProperty_1_Values"></a> Values

```csharp
public List<TPacketProperty> Values { get; init; }
```

#### Property Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<TPacketProperty\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ListProperty_1_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static ListProperty<TPacketProperty> Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [ListProperty](Void.Minecraft.Network.Registries.Transformations.Properties.ListProperty\-1.md)<TPacketProperty\>

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ListProperty_1_Read_Void_Minecraft_Buffers_MinecraftBuffer__System_Int32_"></a> Read\(ref MinecraftBuffer, int\)

```csharp
public static ListProperty<TPacketProperty> Read(ref MinecraftBuffer buffer, int size)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

`size` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [ListProperty](Void.Minecraft.Network.Registries.Transformations.Properties.ListProperty\-1.md)<TPacketProperty\>

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_ListProperty_1_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

