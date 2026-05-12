# <a id="Void_Minecraft_Commands_Brigadier_Serializers_Passthrough_BytePassthroughArgumentValue"></a> Class BytePassthroughArgumentValue

Namespace: [Void.Minecraft.Commands.Brigadier.Serializers.Passthrough](Void.Minecraft.Commands.Brigadier.Serializers.Passthrough.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record BytePassthroughArgumentValue : IPassthroughArgumentValue, IArgumentType, IAnyArgumentType, IEquatable<BytePassthroughArgumentValue>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BytePassthroughArgumentValue](Void.Minecraft.Commands.Brigadier.Serializers.Passthrough.BytePassthroughArgumentValue.md)

#### Implements

[IPassthroughArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IPassthroughArgumentValue.md), 
[IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md), 
[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md), 
[IEquatable<BytePassthroughArgumentValue\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_Passthrough_BytePassthroughArgumentValue__ctor_Void_Minecraft_Commands_Brigadier_Serializers_IArgumentSerializer_System_Byte_"></a> BytePassthroughArgumentValue\(IArgumentSerializer, byte\)

```csharp
public BytePassthroughArgumentValue(IArgumentSerializer Serializer, byte Value)
```

#### Parameters

`Serializer` [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

`Value` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_Passthrough_BytePassthroughArgumentValue_Serializer"></a> Serializer

```csharp
public IArgumentSerializer Serializer { get; init; }
```

#### Property Value

 [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_Passthrough_BytePassthroughArgumentValue_Value"></a> Value

```csharp
public byte Value { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

