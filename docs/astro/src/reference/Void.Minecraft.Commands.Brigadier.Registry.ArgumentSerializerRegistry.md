# <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerRegistry"></a> Class ArgumentSerializerRegistry

Namespace: [Void.Minecraft.Commands.Brigadier.Registry](Void.Minecraft.Commands.Brigadier.Registry.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class ArgumentSerializerRegistry
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ArgumentSerializerRegistry](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerRegistry.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerRegistry_DecodeParserMapping_Void_Minecraft_Buffers_BufferSpan__Void_Minecraft_Network_ProtocolVersion_"></a> DecodeParserMapping\(ref BufferSpan, ProtocolVersion\)

```csharp
public static ArgumentSerializerMapping DecodeParserMapping(ref BufferSpan buffer, ProtocolVersion protocolVersion)
```

#### Parameters

`buffer` [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerRegistry_Deserialize_Void_Minecraft_Buffers_BufferSpan__Void_Minecraft_Network_ProtocolVersion_"></a> Deserialize\(ref BufferSpan, ProtocolVersion\)

```csharp
public static IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
```

#### Parameters

`buffer` [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerRegistry_Register_Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_Void_Minecraft_Commands_Brigadier_Serializers_IArgumentSerializer_"></a> Register\(ArgumentSerializerMapping, IArgumentSerializer?\)

```csharp
public static void Register(ArgumentSerializerMapping mapping, IArgumentSerializer? serializer = null)
```

#### Parameters

`mapping` [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

`serializer` [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerRegistry_Register_Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_System_Type_Void_Minecraft_Commands_Brigadier_Serializers_IArgumentSerializer_"></a> Register\(ArgumentSerializerMapping, Type?, IArgumentSerializer\)

```csharp
public static void Register(ArgumentSerializerMapping mapping, Type? argumentType, IArgumentSerializer serializer)
```

#### Parameters

`mapping` [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

`argumentType` [Type](https://learn.microsoft.com/dotnet/api/system.type)?

`serializer` [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerRegistry_Serialize_Void_Minecraft_Buffers_BufferSpan__Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentType_Void_Minecraft_Network_ProtocolVersion_"></a> Serialize\(ref BufferSpan, IArgumentType, ProtocolVersion\)

```csharp
public static void Serialize(ref BufferSpan buffer, IArgumentType argumentType, ProtocolVersion protocolVersion)
```

#### Parameters

`buffer` [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

`argumentType` [IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerRegistry_WriteParserIdentifier_Void_Minecraft_Buffers_BufferSpan__Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_Void_Minecraft_Network_ProtocolVersion_"></a> WriteParserIdentifier\(ref BufferSpan, ArgumentSerializerMapping, ProtocolVersion\)

```csharp
public static void WriteParserIdentifier(ref BufferSpan buffer, ArgumentSerializerMapping mapping, ProtocolVersion protocolVersion)
```

#### Parameters

`buffer` [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

`mapping` [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

