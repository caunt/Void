# <a id="Void_Minecraft_Network_Definitions_ArgumentParserDefinition"></a> Class ArgumentParserDefinition

Namespace: [Void.Minecraft.Network.Definitions](Void.Minecraft.Network.Definitions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ArgumentParserDefinition : IEquatable<ArgumentParserDefinition>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ArgumentParserDefinition](Void.Minecraft.Network.Definitions.ArgumentParserDefinition.md)

#### Implements

[IEquatable<ArgumentParserDefinition\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Definitions_ArgumentParserDefinition__ctor_Void_Minecraft_Commands_Brigadier_Serializers_IArgumentSerializer_System_Type_Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_"></a> ArgumentParserDefinition\(IArgumentSerializer, Type?, ArgumentSerializerMapping\)

```csharp
public ArgumentParserDefinition(IArgumentSerializer Serializer, Type? ArgumentType, ArgumentSerializerMapping Mapping)
```

#### Parameters

`Serializer` [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

`ArgumentType` [Type](https://learn.microsoft.com/dotnet/api/system.type)?

`Mapping` [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

## Properties

### <a id="Void_Minecraft_Network_Definitions_ArgumentParserDefinition_ArgumentType"></a> ArgumentType

```csharp
public Type? ArgumentType { get; init; }
```

#### Property Value

 [Type](https://learn.microsoft.com/dotnet/api/system.type)?

### <a id="Void_Minecraft_Network_Definitions_ArgumentParserDefinition_Mapping"></a> Mapping

```csharp
public ArgumentSerializerMapping Mapping { get; init; }
```

#### Property Value

 [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

### <a id="Void_Minecraft_Network_Definitions_ArgumentParserDefinition_Serializer"></a> Serializer

```csharp
public IArgumentSerializer Serializer { get; init; }
```

#### Property Value

 [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

## Methods

### <a id="Void_Minecraft_Network_Definitions_ArgumentParserDefinition_From_Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_"></a> From\(ArgumentSerializerMapping\)

```csharp
public static ArgumentParserDefinition From(ArgumentSerializerMapping mapping)
```

#### Parameters

`mapping` [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

#### Returns

 [ArgumentParserDefinition](Void.Minecraft.Network.Definitions.ArgumentParserDefinition.md)

### <a id="Void_Minecraft_Network_Definitions_ArgumentParserDefinition_From_Void_Minecraft_Commands_Brigadier_Serializers_IArgumentSerializer_Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_"></a> From\(IArgumentSerializer, ArgumentSerializerMapping\)

```csharp
public static ArgumentParserDefinition From(IArgumentSerializer serializer, ArgumentSerializerMapping mapping)
```

#### Parameters

`serializer` [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

`mapping` [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

#### Returns

 [ArgumentParserDefinition](Void.Minecraft.Network.Definitions.ArgumentParserDefinition.md)

### <a id="Void_Minecraft_Network_Definitions_ArgumentParserDefinition_From__1_Void_Minecraft_Commands_Brigadier_Serializers_IArgumentSerializer_Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_"></a> From<TArgumentType\>\(IArgumentSerializer, ArgumentSerializerMapping\)

```csharp
public static ArgumentParserDefinition From<TArgumentType>(IArgumentSerializer serializer, ArgumentSerializerMapping mapping)
```

#### Parameters

`serializer` [IArgumentSerializer](Void.Minecraft.Commands.Brigadier.Serializers.IArgumentSerializer.md)

`mapping` [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

#### Returns

 [ArgumentParserDefinition](Void.Minecraft.Network.Definitions.ArgumentParserDefinition.md)

#### Type Parameters

`TArgumentType` 

