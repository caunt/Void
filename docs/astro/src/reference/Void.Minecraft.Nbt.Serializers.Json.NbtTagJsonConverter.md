# <a id="Void_Minecraft_Nbt_Serializers_Json_NbtTagJsonConverter"></a> Class NbtTagJsonConverter

Namespace: [Void.Minecraft.Nbt.Serializers.Json](Void.Minecraft.Nbt.Serializers.Json.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class NbtTagJsonConverter : JsonConverter<NbtTag>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[JsonConverter](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter) ← 
[JsonConverter<NbtTag\>](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1) ← 
[NbtTagJsonConverter](Void.Minecraft.Nbt.Serializers.Json.NbtTagJsonConverter.md)

#### Inherited Members

[JsonConverter<NbtTag\>.CanConvert\(Type\)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.canconvert), 
[JsonConverter<NbtTag\>.Read\(ref Utf8JsonReader, Type, JsonSerializerOptions\)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.read), 
[JsonConverter<NbtTag\>.ReadAsPropertyName\(ref Utf8JsonReader, Type, JsonSerializerOptions\)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.readaspropertyname), 
[JsonConverter<NbtTag\>.Write\(Utf8JsonWriter, NbtTag, JsonSerializerOptions\)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.write), 
[JsonConverter<NbtTag\>.WriteAsPropertyName\(Utf8JsonWriter, NbtTag, JsonSerializerOptions\)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.writeaspropertyname), 
[JsonConverter<NbtTag\>.HandleNull](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.handlenull), 
[JsonConverter<NbtTag\>.Type](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.type), 
[JsonConverter.CanConvert\(Type\)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter.canconvert), 
[JsonConverter.Type](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter.type), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Nbt_Serializers_Json_NbtTagJsonConverter_Read_System_Text_Json_Utf8JsonReader__System_Type_System_Text_Json_JsonSerializerOptions_"></a> Read\(ref Utf8JsonReader, Type, JsonSerializerOptions\)

Reads and converts the JSON to type <xref href="Void.Minecraft.Nbt.NbtTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public override NbtTag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`reader` [Utf8JsonReader](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonreader)

The reader.

`typeToConvert` [Type](https://learn.microsoft.com/dotnet/api/system.type)

The type to convert.

`options` [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)

An object that specifies serialization options to use.

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

The converted value.

### <a id="Void_Minecraft_Nbt_Serializers_Json_NbtTagJsonConverter_Write_System_Text_Json_Utf8JsonWriter_Void_Minecraft_Nbt_NbtTag_System_Text_Json_JsonSerializerOptions_"></a> Write\(Utf8JsonWriter, NbtTag, JsonSerializerOptions\)

Writes a specified value as JSON.

```csharp
public override void Write(Utf8JsonWriter writer, NbtTag tag, JsonSerializerOptions options)
```

#### Parameters

`writer` [Utf8JsonWriter](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonwriter)

The writer to write to.

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

`options` [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)

An object that specifies serialization options to use.

