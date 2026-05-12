# <a id="Void_Minecraft_Commands_Brigadier_Serializers_FloatArgumentSerializer"></a> Class FloatArgumentSerializer

Namespace: [Void.Minecraft.Commands.Brigadier.Serializers](Void.Minecraft.Commands.Brigadier.Serializers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class FloatArgumentSerializer : IArgumentSerializer
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[FloatArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.FloatArgumentSerializer.md)

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

## Fields

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_FloatArgumentSerializer_HAS_MAXIMUM"></a> HAS\_MAXIMUM

```csharp
public const byte HAS_MAXIMUM = 2
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_FloatArgumentSerializer_HAS_MINIMUM"></a> HAS\_MINIMUM

```csharp
public const byte HAS_MINIMUM = 1
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_FloatArgumentSerializer_Instance"></a> Instance

```csharp
public static IArgumentSerializer Instance { get; }
```

#### Property Value

 [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_FloatArgumentSerializer_Deserialize_Void_Minecraft_Buffers_BufferSpan__Void_Minecraft_Network_ProtocolVersion_"></a> Deserialize\(ref BufferSpan, ProtocolVersion\)

```csharp
public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
```

#### Parameters

`buffer` [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_FloatArgumentSerializer_Serialize_Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentType_Void_Minecraft_Buffers_BufferSpan__Void_Minecraft_Network_ProtocolVersion_"></a> Serialize\(IArgumentType, ref BufferSpan, ProtocolVersion\)

```csharp
public void Serialize(IArgumentType argumentType, ref BufferSpan buffer, ProtocolVersion protocolVersion)
```

#### Parameters

`argumentType` [IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md)

`buffer` [BufferSpan](Void.Minecraft.Buffers.BufferSpan.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

