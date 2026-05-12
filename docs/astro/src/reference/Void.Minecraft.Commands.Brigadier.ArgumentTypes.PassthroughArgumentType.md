# <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_PassthroughArgumentType"></a> Class PassthroughArgumentType

Namespace: [Void.Minecraft.Commands.Brigadier.ArgumentTypes](Void.Minecraft.Commands.Brigadier.ArgumentTypes.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record PassthroughArgumentType : IArgumentType, IAnyArgumentType, IEquatable<PassthroughArgumentType>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PassthroughArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.PassthroughArgumentType.md)

#### Implements

[IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md), 
[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md), 
[IEquatable<PassthroughArgumentType\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_PassthroughArgumentType__ctor_Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_Void_Minecraft_Commands_Brigadier_ArgumentTypes_IPassthroughArgumentValue_"></a> PassthroughArgumentType\(ArgumentSerializerMapping, IPassthroughArgumentValue\)

```csharp
public PassthroughArgumentType(ArgumentSerializerMapping Mappings, IPassthroughArgumentValue Value)
```

#### Parameters

`Mappings` [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

`Value` [IPassthroughArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IPassthroughArgumentValue.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_PassthroughArgumentType_Examples"></a> Examples

```csharp
public IEnumerable<string> Examples { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_PassthroughArgumentType_Mappings"></a> Mappings

```csharp
public ArgumentSerializerMapping Mappings { get; init; }
```

#### Property Value

 [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_PassthroughArgumentType_Value"></a> Value

```csharp
public IPassthroughArgumentValue Value { get; init; }
```

#### Property Value

 [IPassthroughArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IPassthroughArgumentValue.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_PassthroughArgumentType_Parse_Void_Minecraft_Commands_Brigadier_StringReader_"></a> Parse\(StringReader\)

```csharp
public IArgumentValue Parse(StringReader reader)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

#### Returns

 [IArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentValue.md)

