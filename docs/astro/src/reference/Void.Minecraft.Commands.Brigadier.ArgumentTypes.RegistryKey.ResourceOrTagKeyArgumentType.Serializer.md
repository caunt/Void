# <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_RegistryKey_ResourceOrTagKeyArgumentType_Serializer"></a> Class ResourceOrTagKeyArgumentType.Serializer

Namespace: [Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey](Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class ResourceOrTagKeyArgumentType.Serializer : IArgumentSerializer
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ResourceOrTagKeyArgumentType.Serializer](Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey.ResourceOrTagKeyArgumentType.Serializer.md)

#### Implements

[IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_RegistryKey_ResourceOrTagKeyArgumentType_Serializer_Instance"></a> Instance

```csharp
public static IArgumentSerializer Instance { get; }
```

#### Property Value

 [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_RegistryKey_ResourceOrTagKeyArgumentType_Serializer_Deserialize_Void_Minecraft_Buffers_BufferSpan__Void_Minecraft_Network_ProtocolVersion_"></a> Deserialize\(ref BufferSpan, ProtocolVersion\)

```csharp
public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
```

#### Parameters

`buffer` [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_RegistryKey_ResourceOrTagKeyArgumentType_Serializer_Serialize_Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentType_Void_Minecraft_Buffers_BufferSpan__Void_Minecraft_Network_ProtocolVersion_"></a> Serialize\(IArgumentType, ref BufferSpan, ProtocolVersion\)

```csharp
public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
```

#### Parameters

`value` [IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md)

`buffer` [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

