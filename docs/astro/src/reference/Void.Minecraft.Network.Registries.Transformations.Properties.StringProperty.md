# <a id="Void_Minecraft_Network_Registries_Transformations_Properties_StringProperty"></a> Class StringProperty

Namespace: [Void.Minecraft.Network.Registries.Transformations.Properties](Void.Minecraft.Network.Registries.Transformations.Properties.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record StringProperty : IPacketProperty<StringProperty>, IPacketProperty, IEquatable<StringProperty>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[StringProperty](Void.Minecraft.Network.Registries.Transformations.Properties.StringProperty.md)

#### Implements

[IPacketProperty<StringProperty\>](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty\-1.md), 
[IPacketProperty](Void.Minecraft.Network.Registries.Transformations.Properties.IPacketProperty.md), 
[IEquatable<StringProperty\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_StringProperty__ctor_System_ReadOnlyMemory_System_Byte__"></a> StringProperty\(ReadOnlyMemory<byte\>\)

```csharp
public StringProperty(ReadOnlyMemory<byte> Value)
```

#### Parameters

`Value` [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_StringProperty_AsJsonNode"></a> AsJsonNode

```csharp
public JsonNode AsJsonNode { get; }
```

#### Property Value

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_StringProperty_AsPrimitive"></a> AsPrimitive

```csharp
public string AsPrimitive { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_StringProperty_Value"></a> Value

```csharp
public ReadOnlyMemory<byte> Value { get; init; }
```

#### Property Value

 [ReadOnlyMemory](https://learn.microsoft.com/dotnet/api/system.readonlymemory\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_StringProperty_FromJsonNode_System_Text_Json_Nodes_JsonNode_System_Text_Json_JsonSerializerOptions_"></a> FromJsonNode\(JsonNode, JsonSerializerOptions?\)

```csharp
public static StringProperty FromJsonNode(JsonNode value, JsonSerializerOptions? jsonSerializerOptions = null)
```

#### Parameters

`value` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

`jsonSerializerOptions` [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)?

#### Returns

 [StringProperty](Void.Minecraft.Network.Registries.Transformations.Properties.StringProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_StringProperty_FromPrimitive_System_ReadOnlySpan_System_Char__"></a> FromPrimitive\(ReadOnlySpan<char\>\)

```csharp
public static StringProperty FromPrimitive(ReadOnlySpan<char> value)
```

#### Parameters

`value` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[char](https://learn.microsoft.com/dotnet/api/system.char)\>

#### Returns

 [StringProperty](Void.Minecraft.Network.Registries.Transformations.Properties.StringProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_StringProperty_Read_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Read\(ref MinecraftBuffer\)

```csharp
public static StringProperty Read(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

#### Returns

 [StringProperty](Void.Minecraft.Network.Registries.Transformations.Properties.StringProperty.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_StringProperty_ToJsonNode_System_Nullable_System_Text_Json_Nodes_JsonNodeOptions__System_Text_Json_JsonDocumentOptions_"></a> ToJsonNode\(JsonNodeOptions?, JsonDocumentOptions\)

```csharp
public JsonNode ToJsonNode(JsonNodeOptions? jsonNodeOptions = null, JsonDocumentOptions jsonDocumentOptions = default)
```

#### Parameters

`jsonNodeOptions` [JsonNodeOptions](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnodeoptions)?

`jsonDocumentOptions` [JsonDocumentOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsondocumentoptions)

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

### <a id="Void_Minecraft_Network_Registries_Transformations_Properties_StringProperty_Write_Void_Minecraft_Buffers_MinecraftBuffer__"></a> Write\(ref MinecraftBuffer\)

```csharp
public void Write(ref MinecraftBuffer buffer)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

