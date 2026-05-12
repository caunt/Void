# <a id="Void_Minecraft_Components_Text_Serializers_ComponentJsonSerializer"></a> Class ComponentJsonSerializer

Namespace: [Void.Minecraft.Components.Text.Serializers](Void.Minecraft.Components.Text.Serializers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class ComponentJsonSerializer
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ComponentJsonSerializer](Void.Minecraft.Components.Text.Serializers.ComponentJsonSerializer.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Components_Text_Serializers_ComponentJsonSerializer_Deserialize_System_String_"></a> Deserialize\(string\)

```csharp
public static Component Deserialize(string value)
```

#### Parameters

`value` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

### <a id="Void_Minecraft_Components_Text_Serializers_ComponentJsonSerializer_Deserialize_System_Text_Json_Nodes_JsonNode_"></a> Deserialize\(JsonNode\)

```csharp
public static Component Deserialize(JsonNode node)
```

#### Parameters

`node` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

### <a id="Void_Minecraft_Components_Text_Serializers_ComponentJsonSerializer_Serialize_Void_Minecraft_Components_Text_Component_"></a> Serialize\(Component\)

```csharp
public static JsonNode Serialize(Component component)
```

#### Parameters

`component` [Component](Void.Minecraft.Components.Text.Component.md)

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

