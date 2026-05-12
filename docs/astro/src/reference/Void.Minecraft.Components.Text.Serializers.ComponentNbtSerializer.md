# <a id="Void_Minecraft_Components_Text_Serializers_ComponentNbtSerializer"></a> Class ComponentNbtSerializer

Namespace: [Void.Minecraft.Components.Text.Serializers](Void.Minecraft.Components.Text.Serializers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class ComponentNbtSerializer
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ComponentNbtSerializer](Void.Minecraft.Components.Text.Serializers.ComponentNbtSerializer.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Components_Text_Serializers_ComponentNbtSerializer_Deserialize_Void_Minecraft_Nbt_NbtTag_"></a> Deserialize\(NbtTag\)

```csharp
public static Component Deserialize(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

### <a id="Void_Minecraft_Components_Text_Serializers_ComponentNbtSerializer_Serialize_Void_Minecraft_Components_Text_Component_"></a> Serialize\(Component\)

```csharp
public static NbtCompound Serialize(Component component)
```

#### Parameters

`component` [Component](Void.Minecraft.Components.Text.Component.md)

#### Returns

 [NbtCompound](Void.Minecraft.Nbt.Tags.NbtCompound.md)

