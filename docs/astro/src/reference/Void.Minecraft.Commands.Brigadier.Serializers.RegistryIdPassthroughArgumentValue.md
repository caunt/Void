# <a id="Void_Minecraft_Commands_Brigadier_Serializers_RegistryIdPassthroughArgumentValue"></a> Class RegistryIdPassthroughArgumentValue

Namespace: [Void.Minecraft.Commands.Brigadier.Serializers](Void.Minecraft.Commands.Brigadier.Serializers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record RegistryIdPassthroughArgumentValue : IPassthroughArgumentValue, IArgumentType, IAnyArgumentType, IEquatable<RegistryIdPassthroughArgumentValue>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[RegistryIdPassthroughArgumentValue](Void.Minecraft.Commands.Brigadier.Serializers.RegistryIdPassthroughArgumentValue.md)

#### Implements

[IPassthroughArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IPassthroughArgumentValue.md), 
[IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md), 
[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md), 
[IEquatable<RegistryIdPassthroughArgumentValue\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_RegistryIdPassthroughArgumentValue__ctor_Void_Minecraft_Commands_Brigadier_Serializers_IArgumentSerializer_System_Int32_"></a> RegistryIdPassthroughArgumentValue\(IArgumentSerializer, int\)

```csharp
public RegistryIdPassthroughArgumentValue(IArgumentSerializer Serializer, int Value)
```

#### Parameters

`Serializer` [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

`Value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_RegistryIdPassthroughArgumentValue_Serializer"></a> Serializer

```csharp
public IArgumentSerializer Serializer { get; init; }
```

#### Property Value

 [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

### <a id="Void_Minecraft_Commands_Brigadier_Serializers_RegistryIdPassthroughArgumentValue_Value"></a> Value

```csharp
public int Value { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

