# <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper"></a> Interface IMinecraftBinaryPacketWrapper

Namespace: [Void.Minecraft.Network.Registries.Transformations.Mappings](Void.Minecraft.Network.Registries.Transformations.Mappings.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftBinaryPacketWrapper
```

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_Get__1_System_Int32_"></a> Get<TPropertyValue\>\(int\)

```csharp
TPropertyValue Get<TPropertyValue>(int index) where TPropertyValue : IPacketProperty<TPropertyValue>
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 TPropertyValue

#### Type Parameters

`TPropertyValue` 

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_Passthrough__1"></a> Passthrough<TPropertyValue\>\(\)

```csharp
TPropertyValue Passthrough<TPropertyValue>() where TPropertyValue : IPacketProperty<TPropertyValue>
```

#### Returns

 TPropertyValue

#### Type Parameters

`TPropertyValue` 

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_Read__1"></a> Read<TPropertyValue\>\(\)

```csharp
TPropertyValue Read<TPropertyValue>() where TPropertyValue : IPacketProperty<TPropertyValue>
```

#### Returns

 TPropertyValue

#### Type Parameters

`TPropertyValue` 

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_Reset"></a> Reset\(\)

```csharp
void Reset()
```

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_Set__1_System_Int32___0_"></a> Set<TPropertyValue\>\(int, TPropertyValue\)

```csharp
void Set<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`value` TPropertyValue

#### Type Parameters

`TPropertyValue` 

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_TryGet__1_System_Int32___0__"></a> TryGet<TPropertyValue\>\(int, out TPropertyValue\)

```csharp
bool TryGet<TPropertyValue>(int index, out TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`value` TPropertyValue

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`TPropertyValue` 

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_TrySet__1_System_Int32___0_"></a> TrySet<TPropertyValue\>\(int, TPropertyValue\)

```csharp
bool TrySet<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`value` TPropertyValue

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`TPropertyValue` 

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_Write__1___0_"></a> Write<TPropertyValue\>\(TPropertyValue\)

```csharp
void Write<TPropertyValue>(TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>
```

#### Parameters

`value` TPropertyValue

#### Type Parameters

`TPropertyValue` 

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_WriteProcessedValues_Void_Minecraft_Buffers_MinecraftBuffer__"></a> WriteProcessedValues\(ref MinecraftBuffer\)

```csharp
void WriteProcessedValues(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

