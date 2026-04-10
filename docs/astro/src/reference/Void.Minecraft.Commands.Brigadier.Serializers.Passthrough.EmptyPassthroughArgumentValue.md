# <a id="Void_Minecraft_Commands_Brigadier_Serializers_Passthrough_EmptyPassthroughArgumentValue"></a> Class EmptyPassthroughArgumentValue

Namespace: [Void.Minecraft.Commands.Brigadier.Serializers.Passthrough](Void.Minecraft.Commands.Brigadier.Serializers.Passthrough.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record EmptyPassthroughArgumentValue : IPassthroughArgumentValue, IArgumentType, IAnyArgumentType, IEquatable<EmptyPassthroughArgumentValue>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[EmptyPassthroughArgumentValue](Void.Minecraft.Commands.Brigadier.Serializers.Passthrough.EmptyPassthroughArgumentValue.md)

#### Implements

[IPassthroughArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IPassthroughArgumentValue.md), 
[IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md), 
[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md), 
[IEquatable<EmptyPassthroughArgumentValue\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_Passthrough_EmptyPassthroughArgumentValue__ctor_Void_Minecraft_Commands_Brigadier_Serializers_IArgumentSerializer_"></a> EmptyPassthroughArgumentValue\(IArgumentSerializer\)

```csharp
public EmptyPassthroughArgumentValue(IArgumentSerializer Serializer)
```

#### Parameters

`Serializer` [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_Passthrough_EmptyPassthroughArgumentValue_Serializer"></a> Serializer

```csharp
public IArgumentSerializer Serializer { get; init; }
```

#### Property Value

 [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

