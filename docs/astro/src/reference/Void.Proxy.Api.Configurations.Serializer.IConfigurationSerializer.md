# <a id="Void_Proxy_Api_Configurations_Serializer_IConfigurationSerializer"></a> Interface IConfigurationSerializer

Namespace: [Void.Proxy.Api.Configurations.Serializer](Void.Proxy.Api.Configurations.Serializer.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IConfigurationSerializer
```

## Methods

### <a id="Void_Proxy_Api_Configurations_Serializer_IConfigurationSerializer_Deserialize__1_System_String_"></a> Deserialize<TConfiguration\>\(string\)

```csharp
TConfiguration Deserialize<TConfiguration>(string source) where TConfiguration : notnull
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 TConfiguration

#### Type Parameters

`TConfiguration` 

### <a id="Void_Proxy_Api_Configurations_Serializer_IConfigurationSerializer_Deserialize_System_String_System_Type_"></a> Deserialize\(string, Type\)

```csharp
object Deserialize(string source, Type configurationType)
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

`configurationType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

#### Returns

 [object](https://learn.microsoft.com/dotnet/api/system.object)

### <a id="Void_Proxy_Api_Configurations_Serializer_IConfigurationSerializer_Serialize__1"></a> Serialize<TConfiguration\>\(\)

```csharp
string Serialize<TConfiguration>() where TConfiguration : notnull
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Type Parameters

`TConfiguration` 

### <a id="Void_Proxy_Api_Configurations_Serializer_IConfigurationSerializer_Serialize_System_Object_"></a> Serialize\(object\)

```csharp
string Serialize(object configuration)
```

#### Parameters

`configuration` [object](https://learn.microsoft.com/dotnet/api/system.object)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Proxy_Api_Configurations_Serializer_IConfigurationSerializer_Serialize__1___0_"></a> Serialize<TConfiguration\>\(TConfiguration?\)

```csharp
string Serialize<TConfiguration>(TConfiguration? configuration) where TConfiguration : notnull
```

#### Parameters

`configuration` TConfiguration?

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Type Parameters

`TConfiguration` 

### <a id="Void_Proxy_Api_Configurations_Serializer_IConfigurationSerializer_Serialize_System_Object_System_Type_"></a> Serialize\(object?, Type\)

```csharp
string Serialize(object? configuration, Type configurationType)
```

#### Parameters

`configuration` [object](https://learn.microsoft.com/dotnet/api/system.object)?

`configurationType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

