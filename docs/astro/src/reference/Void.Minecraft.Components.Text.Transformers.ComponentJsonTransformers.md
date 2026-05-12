# <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers"></a> Class ComponentJsonTransformers

Namespace: [Void.Minecraft.Components.Text.Transformers](Void.Minecraft.Components.Text.Transformers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class ComponentJsonTransformers
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ComponentJsonTransformers](Void.Minecraft.Components.Text.Transformers.ComponentJsonTransformers.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Apply_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_ProtocolVersion_"></a> Apply\(IMinecraftBinaryPacketWrapper, ProtocolVersion, ProtocolVersion\)

```csharp
public static void Apply(IMinecraftBinaryPacketWrapper wrapper, ProtocolVersion from, ProtocolVersion to)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

`from` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`to` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Apply_Void_Minecraft_Network_Registries_Transformations_Properties_StringProperty_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_ProtocolVersion_"></a> Apply\(StringProperty, ProtocolVersion, ProtocolVersion\)

```csharp
public static StringProperty Apply(StringProperty property, ProtocolVersion from, ProtocolVersion to)
```

#### Parameters

`property` [StringProperty](Void.Minecraft.Network.Registries.Transformations.Properties.StringProperty.md)

`from` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`to` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [StringProperty](Void.Minecraft.Network.Registries.Transformations.Properties.StringProperty.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Apply_System_String_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_ProtocolVersion_"></a> Apply\(string, ProtocolVersion, ProtocolVersion\)

```csharp
public static string Apply(string value, ProtocolVersion from, ProtocolVersion to)
```

#### Parameters

`value` [string](https://learn.microsoft.com/dotnet/api/system.string)

`from` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`to` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Apply_System_Text_Json_Nodes_JsonNode_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_ProtocolVersion_"></a> Apply\(JsonNode, ProtocolVersion, ProtocolVersion\)

```csharp
public static JsonNode Apply(JsonNode node, ProtocolVersion from, ProtocolVersion to)
```

#### Parameters

`node` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

`from` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`to` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Downgrade_v1_12_to_v1_11_1_System_Text_Json_Nodes_JsonNode_"></a> Downgrade\_v1\_12\_to\_v1\_11\_1\(JsonNode\)

```csharp
public static JsonNode Downgrade_v1_12_to_v1_11_1(JsonNode node)
```

#### Parameters

`node` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Downgrade_v1_16_to_v1_15_2_System_Text_Json_Nodes_JsonNode_"></a> Downgrade\_v1\_16\_to\_v1\_15\_2\(JsonNode\)

```csharp
public static JsonNode Downgrade_v1_16_to_v1_15_2(JsonNode node)
```

#### Parameters

`node` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Downgrade_v1_9_to_v1_8_System_Text_Json_Nodes_JsonNode_"></a> Downgrade\_v1\_9\_to\_v1\_8\(JsonNode\)

```csharp
public static JsonNode Downgrade_v1_9_to_v1_8(JsonNode node)
```

#### Parameters

`node` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Passthrough_v1_11_1_to_v1_12_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_11\_1\_to\_v1\_12\(IMinecraftBinaryPacketWrapper\)

```csharp
public static void Passthrough_v1_11_1_to_v1_12(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Passthrough_v1_12_to_v1_11_1_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_12\_to\_v1\_11\_1\(IMinecraftBinaryPacketWrapper\)

```csharp
public static void Passthrough_v1_12_to_v1_11_1(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Passthrough_v1_15_2_to_v1_16_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_15\_2\_to\_v1\_16\(IMinecraftBinaryPacketWrapper\)

```csharp
public static void Passthrough_v1_15_2_to_v1_16(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Passthrough_v1_16_to_v1_15_2_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_16\_to\_v1\_15\_2\(IMinecraftBinaryPacketWrapper\)

```csharp
public static void Passthrough_v1_16_to_v1_15_2(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Passthrough_v1_8_to_v1_9_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_8\_to\_v1\_9\(IMinecraftBinaryPacketWrapper\)

```csharp
public static void Passthrough_v1_8_to_v1_9(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Passthrough_v1_9_to_v1_8_Void_Minecraft_Network_Registries_Transformations_Mappings_IMinecraftBinaryPacketWrapper_"></a> Passthrough\_v1\_9\_to\_v1\_8\(IMinecraftBinaryPacketWrapper\)

```csharp
public static void Passthrough_v1_9_to_v1_8(IMinecraftBinaryPacketWrapper wrapper)
```

#### Parameters

`wrapper` [IMinecraftBinaryPacketWrapper](Void.Minecraft.Network.Registries.Transformations.Mappings.IMinecraftBinaryPacketWrapper.md)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Upgrade_v1_11_1_to_v1_12_System_Text_Json_Nodes_JsonNode_"></a> Upgrade\_v1\_11\_1\_to\_v1\_12\(JsonNode\)

```csharp
public static JsonNode Upgrade_v1_11_1_to_v1_12(JsonNode node)
```

#### Parameters

`node` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Upgrade_v1_15_2_to_v1_16_System_Text_Json_Nodes_JsonNode_"></a> Upgrade\_v1\_15\_2\_to\_v1\_16\(JsonNode\)

```csharp
public static JsonNode Upgrade_v1_15_2_to_v1_16(JsonNode node)
```

#### Parameters

`node` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

### <a id="Void_Minecraft_Components_Text_Transformers_ComponentJsonTransformers_Upgrade_v1_8_to_v1_9_System_Text_Json_Nodes_JsonNode_"></a> Upgrade\_v1\_8\_to\_v1\_9\(JsonNode\)

```csharp
public static JsonNode Upgrade_v1_8_to_v1_9(JsonNode node)
```

#### Parameters

`node` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

