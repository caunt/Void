# <a id="Void_Minecraft_Nbt_Serializers_Json_NbtJsonSerializer"></a> Class NbtJsonSerializer

Namespace: [Void.Minecraft.Nbt.Serializers.Json](Void.Minecraft.Nbt.Serializers.Json.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class NbtJsonSerializer
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtJsonSerializer](Void.Minecraft.Nbt.Serializers.Json.NbtJsonSerializer.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Fields

### <a id="Void_Minecraft_Nbt_Serializers_Json_NbtJsonSerializer_Options"></a> Options

```csharp
public static readonly JsonSerializerOptions Options
```

#### Field Value

 [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)

## Methods

### <a id="Void_Minecraft_Nbt_Serializers_Json_NbtJsonSerializer_Deserialize_System_String_"></a> Deserialize\(string\)

```csharp
public static NbtTag Deserialize(string value)
```

#### Parameters

`value` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Nbt_Serializers_Json_NbtJsonSerializer_Deserialize_System_Text_Json_Nodes_JsonNode_"></a> Deserialize\(JsonNode\)

```csharp
public static NbtTag Deserialize(JsonNode node)
```

#### Parameters

`node` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Nbt_Serializers_Json_NbtJsonSerializer_Serialize_Void_Minecraft_Nbt_NbtTag_"></a> Serialize\(NbtTag\)

```csharp
public static JsonNode Serialize(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

