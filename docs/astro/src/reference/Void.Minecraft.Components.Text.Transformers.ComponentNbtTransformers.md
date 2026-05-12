# <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers"></a> Class ComponentNbtTransformers

Namespace: [Void.Minecraft.Components.Text.Transformers](Void.Minecraft.Components.Text.Transformers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class ComponentNbtTransformers
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ComponentNbtTransformers](Void.Minecraft.Components.Text.Transformers.ComponentNbtTransformers.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Apply_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_ProtocolVersion_"></a> Apply\(IMinecraftBinaryPacketWrapper, ProtocolVersion, ProtocolVersion\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static void Apply(IMinecraftBinaryPacketWrapper wrapper, ProtocolVersion from, ProtocolVersion to)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

`from` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`to` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Apply_Void_Minecraft_Network_Registries_Transformations_Properties_NamedNbtProperty_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_ProtocolVersion_"></a> Apply\(NamedNbtProperty, ProtocolVersion, ProtocolVersion\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static NamedNbtProperty Apply(NamedNbtProperty property, ProtocolVersion from, ProtocolVersion to)
```

#### Parameters

`property` [NamedNbtProperty](Void.Minecraft.Network.Registries.Transformations.Properties.NamedNbtProperty.md)

`from` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`to` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [NamedNbtProperty](Void.Minecraft.Network.Registries.Transformations.Properties.NamedNbtProperty.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Apply_Void_Minecraft_Network_Registries_Transformations_Properties_NbtProperty_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_ProtocolVersion_"></a> Apply\(NbtProperty, ProtocolVersion, ProtocolVersion\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static NbtProperty Apply(NbtProperty property, ProtocolVersion from, ProtocolVersion to)
```

#### Parameters

`property` [NbtProperty](Void.Minecraft.Network.Registries.Transformations.Properties.NbtProperty.md)

`from` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`to` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [NbtProperty](Void.Minecraft.Network.Registries.Transformations.Properties.NbtProperty.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Apply_Void_Minecraft_Nbt_NbtTag_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_ProtocolVersion_"></a> Apply\(NbtTag, ProtocolVersion, ProtocolVersion\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static NbtTag Apply(NbtTag tag, ProtocolVersion from, ProtocolVersion to)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

`from` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`to` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Downgrade_v1_12_to_v1_11_1_Void_Minecraft_Nbt_NbtTag_"></a> Downgrade\_v1\_12\_to\_v1\_11\_1\(NbtTag\)

```csharp
public static NbtTag Downgrade_v1_12_to_v1_11_1(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Downgrade_v1_16_to_v1_15_2_Void_Minecraft_Nbt_NbtTag_"></a> Downgrade\_v1\_16\_to\_v1\_15\_2\(NbtTag\)

```csharp
public static NbtTag Downgrade_v1_16_to_v1_15_2(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Downgrade_v1_20_3_to_v1_20_2_Void_Minecraft_Nbt_NbtTag_"></a> Downgrade\_v1\_20\_3\_to\_v1\_20\_2\(NbtTag\)

```csharp
public static NbtTag Downgrade_v1_20_3_to_v1_20_2(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Downgrade_v1_21_5_to_v1_21_4_Void_Minecraft_Nbt_NbtTag_"></a> Downgrade\_v1\_21\_5\_to\_v1\_21\_4\(NbtTag\)

```csharp
public static NbtTag Downgrade_v1_21_5_to_v1_21_4(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Downgrade_v1_9_to_v1_8_Void_Minecraft_Nbt_NbtTag_"></a> Downgrade\_v1\_9\_to\_v1\_8\(NbtTag\)

```csharp
public static NbtTag Downgrade_v1_9_to_v1_8(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Passthrough_v1_11_1_to_v1_12_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_11\_1\_to\_v1\_12\(IMinecraftBinaryPacketWrapper\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static void Passthrough_v1_11_1_to_v1_12(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Passthrough_v1_12_to_v1_11_1_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_12\_to\_v1\_11\_1\(IMinecraftBinaryPacketWrapper\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static void Passthrough_v1_12_to_v1_11_1(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Passthrough_v1_15_2_to_v1_16_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_15\_2\_to\_v1\_16\(IMinecraftBinaryPacketWrapper\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static void Passthrough_v1_15_2_to_v1_16(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Passthrough_v1_16_to_v1_15_2_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_16\_to\_v1\_15\_2\(IMinecraftBinaryPacketWrapper\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static void Passthrough_v1_16_to_v1_15_2(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Passthrough_v1_20_2_to_v1_20_3_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_20\_2\_to\_v1\_20\_3\(IMinecraftBinaryPacketWrapper\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static void Passthrough_v1_20_2_to_v1_20_3(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Passthrough_v1_20_3_to_v1_20_2_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_20\_3\_to\_v1\_20\_2\(IMinecraftBinaryPacketWrapper\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static void Passthrough_v1_20_3_to_v1_20_2(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Passthrough_v1_21_4_to_v1_21_5_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_21\_4\_to\_v1\_21\_5\(IMinecraftBinaryPacketWrapper\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static void Passthrough_v1_21_4_to_v1_21_5(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Passthrough_v1_21_5_to_v1_21_4_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_21\_5\_to\_v1\_21\_4\(IMinecraftBinaryPacketWrapper\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static void Passthrough_v1_21_5_to_v1_21_4(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Passthrough_v1_8_to_v1_9_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_8\_to\_v1\_9\(IMinecraftBinaryPacketWrapper\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static void Passthrough_v1_8_to_v1_9(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Passthrough_v1_9_to_v1_8_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_9\_to\_v1\_8\(IMinecraftBinaryPacketWrapper\)

```csharp
[Obsolete("Rewrite properties yourself instead.")]
public static void Passthrough_v1_9_to_v1_8(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Upgrade_v1_11_1_to_v1_12_Void_Minecraft_Nbt_NbtTag_"></a> Upgrade\_v1\_11\_1\_to\_v1\_12\(NbtTag\)

```csharp
public static NbtTag Upgrade_v1_11_1_to_v1_12(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Upgrade_v1_15_2_to_v1_16_Void_Minecraft_Nbt_NbtTag_"></a> Upgrade\_v1\_15\_2\_to\_v1\_16\(NbtTag\)

```csharp
public static NbtTag Upgrade_v1_15_2_to_v1_16(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Upgrade_v1_20_2_to_v1_20_3_Void_Minecraft_Nbt_NbtTag_"></a> Upgrade\_v1\_20\_2\_to\_v1\_20\_3\(NbtTag\)

```csharp
public static NbtTag Upgrade_v1_20_2_to_v1_20_3(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Upgrade_v1_21_4_to_v1_21_5_Void_Minecraft_Nbt_NbtTag_"></a> Upgrade\_v1\_21\_4\_to\_v1\_21\_5\(NbtTag\)

```csharp
public static NbtTag Upgrade_v1_21_4_to_v1_21_5(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentNbtTransformers_Upgrade_v1_8_to_v1_9_Void_Minecraft_Nbt_NbtTag_"></a> Upgrade\_v1\_8\_to\_v1\_9\(NbtTag\)

```csharp
public static NbtTag Upgrade_v1_8_to_v1_9(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

