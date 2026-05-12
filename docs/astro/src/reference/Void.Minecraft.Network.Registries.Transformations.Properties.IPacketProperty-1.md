# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_IPacketProperty_1"></a> Interface IPacketProperty<TPacketProperty\>

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IPacketProperty<TPacketProperty> : IPacketProperty where TPacketProperty : IPacketProperty<TPacketProperty>
```

#### Type Parameters

`TPacketProperty` 

#### Implements

[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md)

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_IPacketProperty_1_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static abstract TPacketProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 TPacketProperty

