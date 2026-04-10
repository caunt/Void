# <a id="Void_Minecraft_Profiles_Serializers_UuidJsonConverter"></a> Class UuidJsonConverter

Namespace: [Void.Minecraft.Profiles.Serializers](Void.Minecraft.Profiles.Serializers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class UuidJsonConverter : JsonConverter<Uuid>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[JsonConverter](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter) ← 
[JsonConverter<Uuid\>](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1) ← 
[UuidJsonConverter](Void.Minecraft.Profiles.Serializers.UuidJsonConverter.md)

#### Inherited Members

[JsonConverter<Uuid\>.CanConvert\(Type\)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.canconvert), 
[JsonConverter<Uuid\>.Read\(ref Utf8JsonReader, Type, JsonSerializerOptions\)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.read), 
[JsonConverter<Uuid\>.ReadAsPropertyName\(ref Utf8JsonReader, Type, JsonSerializerOptions\)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.readaspropertyname), 
[JsonConverter<Uuid\>.Write\(Utf8JsonWriter, Uuid, JsonSerializerOptions\)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.write), 
[JsonConverter<Uuid\>.WriteAsPropertyName\(Utf8JsonWriter, Uuid, JsonSerializerOptions\)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.writeaspropertyname), 
[JsonConverter<Uuid\>.HandleNull](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.handlenull), 
[JsonConverter<Uuid\>.Type](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter\-1.type), 
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

### <a id="Void_Minecraft_Profiles_Serializers_UuidJsonConverter_Read_System_Text_Json_Utf8JsonReader__System_Type_System_Text_Json_JsonSerializerOptions_"></a> Read\(ref Utf8JsonReader, Type, JsonSerializerOptions\)

Reads and converts the JSON to type <xref href="Void.Minecraft.Profiles.Uuid" data-throw-if-not-resolved="false"></xref>.

```csharp
public override Uuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`reader` [Utf8JsonReader](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonreader)

The reader.

`typeToConvert` [Type](https://learn.microsoft.com/dotnet/api/system.type)

The type to convert.

`options` [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)

An object that specifies serialization options to use.

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

The converted value.

### <a id="Void_Minecraft_Profiles_Serializers_UuidJsonConverter_ReadAsPropertyName_System_Text_Json_Utf8JsonReader__System_Type_System_Text_Json_JsonSerializerOptions_"></a> ReadAsPropertyName\(ref Utf8JsonReader, Type, JsonSerializerOptions\)

Reads a dictionary key from a JSON property name.

```csharp
public override Uuid ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`reader` [Utf8JsonReader](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonreader)

The <xref href="System.Text.Json.Utf8JsonReader" data-throw-if-not-resolved="false"></xref> to read from.

`typeToConvert` [Type](https://learn.microsoft.com/dotnet/api/system.type)

The type to convert.

`options` [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)

The options to use when reading the value.

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

The value that was converted.

### <a id="Void_Minecraft_Profiles_Serializers_UuidJsonConverter_Write_System_Text_Json_Utf8JsonWriter_Void_Minecraft_Profiles_Uuid_System_Text_Json_JsonSerializerOptions_"></a> Write\(Utf8JsonWriter, Uuid, JsonSerializerOptions\)

Writes a specified value as JSON.

```csharp
public override void Write(Utf8JsonWriter writer, Uuid value, JsonSerializerOptions options)
```

#### Parameters

`writer` [Utf8JsonWriter](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonwriter)

The writer to write to.

`value` [Uuid](Void.Minecraft.Profiles.Uuid.md)

The value to convert to JSON.

`options` [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)

An object that specifies serialization options to use.

### <a id="Void_Minecraft_Profiles_Serializers_UuidJsonConverter_WriteAsPropertyName_System_Text_Json_Utf8JsonWriter_Void_Minecraft_Profiles_Uuid_System_Text_Json_JsonSerializerOptions_"></a> WriteAsPropertyName\(Utf8JsonWriter, Uuid, JsonSerializerOptions\)

Writes a dictionary key as a JSON property name.

```csharp
public override void WriteAsPropertyName(Utf8JsonWriter writer, Uuid value, JsonSerializerOptions options)
```

#### Parameters

`writer` [Utf8JsonWriter](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonwriter)

The <xref href="System.Text.Json.Utf8JsonWriter" data-throw-if-not-resolved="false"></xref> to write to.

`value` [Uuid](Void.Minecraft.Profiles.Uuid.md)

The value to convert. The value of <xref href="System.Text.Json.Serialization.JsonConverter%601.HandleNull" data-throw-if-not-resolved="false"></xref> determines if the converter handles <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> values.

`options` [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)

The options to use when writing the value.

