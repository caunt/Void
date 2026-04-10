# <a id="Void_Minecraft_Nbt_Serializers_Json_Adapter_NbtTagNumberAdapter"></a> Class NbtTagNumberAdapter

Namespace: [Void.Minecraft.Nbt.Serializers.Json.Adapter](Void.Minecraft.Nbt.Serializers.Json.Adapter.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class NbtTagNumberAdapter
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NbtTagNumberAdapter](Void.Minecraft.Nbt.Serializers.Json.Adapter.NbtTagNumberAdapter.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Nbt_Serializers_Json_Adapter_NbtTagNumberAdapter_AlignNumbers_System_Collections_Generic_List_Void_Minecraft_Nbt_NbtTag__"></a> AlignNumbers\(List<NbtTag\>\)

```csharp
public static void AlignNumbers(List<NbtTag> tags)
```

#### Parameters

`tags` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[NbtTag](Void.Minecraft.Nbt.NbtTag.md)\>

### <a id="Void_Minecraft_Nbt_Serializers_Json_Adapter_NbtTagNumberAdapter_DeserializeNumber_System_Text_Json_Utf8JsonReader__"></a> DeserializeNumber\(ref Utf8JsonReader\)

```csharp
public static NbtTag DeserializeNumber(ref Utf8JsonReader reader)
```

#### Parameters

`reader` [Utf8JsonReader](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonreader)

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

### <a id="Void_Minecraft_Nbt_Serializers_Json_Adapter_NbtTagNumberAdapter_GetTagsType_System_Collections_Generic_List_Void_Minecraft_Nbt_NbtTag__"></a> GetTagsType\(List<NbtTag\>\)

```csharp
public static NbtTagType GetTagsType(List<NbtTag> tags)
```

#### Parameters

`tags` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[NbtTag](Void.Minecraft.Nbt.NbtTag.md)\>

#### Returns

 [NbtTagType](Void.Minecraft.Nbt.NbtTagType.md)

