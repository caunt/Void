# <a id="Void_Minecraft_Commands_Brigadier_Serializers_IArgumentSerializer"></a> Interface IArgumentSerializer

Namespace: [Void.Minecraft.Commands.Brigadier.Serializers](Void.Minecraft.Commands.Brigadier.Serializers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IArgumentSerializer
```

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_IArgumentSerializer_Deserialize_Void_Minecraft_Buffers_BufferSpan__Void_Minecraft_Network_ProtocolVersion_"></a> Deserialize\(ref BufferSpan, ProtocolVersion\)

```csharp
IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
```

#### Parameters

`buffer` [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_IArgumentSerializer_Serialize_Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentType_Void_Minecraft_Buffers_BufferSpan__Void_Minecraft_Network_ProtocolVersion_"></a> Serialize\(IArgumentType, ref BufferSpan, ProtocolVersion\)

```csharp
void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
```

#### Parameters

`value` [IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md)

`buffer` [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

