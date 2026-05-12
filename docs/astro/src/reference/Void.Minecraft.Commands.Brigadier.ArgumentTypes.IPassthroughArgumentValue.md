# <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IPassthroughArgumentValue"></a> Interface IPassthroughArgumentValue

Namespace: [Void.Minecraft.Commands.Brigadier.ArgumentTypes](Void.Minecraft.Commands.Brigadier.ArgumentTypes.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IPassthroughArgumentValue : IArgumentType, IAnyArgumentType
```

#### Implements

[IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md), 
[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IPassthroughArgumentValue_Serializer"></a> Serializer

```csharp
IArgumentSerializer Serializer { get; }
```

#### Property Value

 [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IPassthroughArgumentValue_Serialize_Void_Minecraft_Buffers_BufferSpan__Void_Minecraft_Network_ProtocolVersion_"></a> Serialize\(ref BufferSpan, ProtocolVersion\)

```csharp
void Serialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
```

#### Parameters

`buffer` [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

