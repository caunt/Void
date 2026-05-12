# <a id="Void_Proxy_Api_Configurations_Serializer_IConfigurationSerializer"></a> Interface IConfigurationSerializer

Namespace: [Void.Proxy.Api.Configurations.Serializer](Void.Proxy.Api.Configurations.Serializer.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public interface IConfigurationSerializer
```

## Methods

### <a id="Void_Proxy_Api_Configurations_Serializer_IConfigurationSerializer_Deserialize__1_System_String_"></a> Deserialize<TConfiguration\>\(string\)

Deserializes configuration text into an instance of <code class="typeparamref">TConfiguration</code>.

```csharp
TConfiguration Deserialize<TConfiguration>(string source) where TConfiguration : notnull
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

The serialized configuration text to parse and map; this value must not be <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

#### Returns

 TConfiguration

A configuration instance cast to <code class="typeparamref">TConfiguration</code>.

#### Type Parameters

`TConfiguration` 

The expected configuration type.

#### Examples

<pre><code class="lang-csharp">var value = serializer.Deserialize&lt;NetworkConfiguration&gt;(sourceText);</code></pre>

#### Remarks

<p>
This member is a typed wrapper around <xref href="Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.Deserialize(System.String%2cSystem.Type)" data-throw-if-not-resolved="false"></xref> that passes
<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/operators/type-testing-and-cast#typeof-operator">typeof</a>(<code class="typeparamref">TConfiguration</code>) as the target type.
</p>
<p>
The built-in serializer implementation parses TOML and maps it with configured options, then
validates that the resulting object can be cast to <code class="typeparamref">TConfiguration</code>.
</p>

#### Exceptions

 [InvalidConfigurationException](Void.Proxy.Api.Configurations.Exceptions.InvalidConfigurationException.md)

Thrown when <code class="paramref">source</code> cannot be parsed, when it cannot be mapped to
<code class="typeparamref">TConfiguration</code>, or when the mapped result is not assignable to
<code class="typeparamref">TConfiguration</code>.

#### See Also

[IConfigurationSerializer](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md).[Deserialize](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md\#Void\_Proxy\_Api\_Configurations\_Serializer\_IConfigurationSerializer\_Deserialize\_System\_String\_System\_Type\_)\([string](https://learn.microsoft.com/dotnet/api/system.string), [Type](https://learn.microsoft.com/dotnet/api/system.type)\)

### <a id="Void_Proxy_Api_Configurations_Serializer_IConfigurationSerializer_Deserialize_System_String_System_Type_"></a> Deserialize\(string, Type\)

Deserializes configuration text into an instance of a runtime-specified configuration type.

```csharp
object Deserialize(string source, Type configurationType)
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

The serialized configuration text to parse and map; this value must not be <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

`configurationType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

The target configuration <xref href="System.Type" data-throw-if-not-resolved="false"></xref> to materialize.

#### Returns

 [object](https://learn.microsoft.com/dotnet/api/system.object)

The deserialized configuration object typed as <xref href="System.Object" data-throw-if-not-resolved="false"></xref>.

#### Examples

<pre><code class="lang-csharp">var value = serializer.Deserialize(sourceText, typeof(NetworkConfiguration));</code></pre>

#### Remarks

<p>
The built-in serializer implementation first parses <code class="paramref">source</code> as TOML, then maps
the parsed document to <code class="paramref">configurationType</code> using serializer options.
</p>
<p>
This overload performs no compile-time type checks, so callers are responsible for casting
the returned value to the expected runtime type.
</p>

#### Exceptions

 [InvalidConfigurationException](Void.Proxy.Api.Configurations.Exceptions.InvalidConfigurationException.md)

Thrown when parsing fails or when the serialized content cannot be converted to
<code class="paramref">configurationType</code>.

#### See Also

[IConfigurationSerializer](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md).[Deserialize](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md\#Void\_Proxy\_Api\_Configurations\_Serializer\_IConfigurationSerializer\_Deserialize\_\_1\_System\_String\_)<TConfiguration\>\([string](https://learn.microsoft.com/dotnet/api/system.string)\), 
[Type](https://learn.microsoft.com/dotnet/api/system.type)

### <a id="Void_Proxy_Api_Configurations_Serializer_IConfigurationSerializer_Serialize__1"></a> Serialize<TConfiguration\>\(\)

Serializes a default-value instance of <code class="typeparamref">TConfiguration</code> to configuration text.

```csharp
string Serialize<TConfiguration>() where TConfiguration : notnull
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A <xref href="System.String" data-throw-if-not-resolved="false"></xref> containing the serialized configuration text for a freshly constructed
<code class="typeparamref">TConfiguration</code> instance populated with its default property values.

#### Type Parameters

`TConfiguration` 

The configuration type whose default representation is serialized.
Must be a non-nullable reference or value type.

#### Examples

<pre><code class="lang-csharp">// Produce a template TOML configuration file with all default values.
string template = serializer.Serialize&lt;NetworkConfiguration&gt;();
await File.WriteAllTextAsync("network.toml", template);</code></pre>

#### Remarks

<p>
This overload is a convenience wrapper that calls
<xref href="Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.Serialize%60%601(%60%600)" data-throw-if-not-resolved="false"></xref> with <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> as the
configuration argument, which in turn delegates to
<xref href="Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.Serialize(System.Object%2cSystem.Type)" data-throw-if-not-resolved="false"></xref> with <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/operators/type-testing-and-cast#typeof-operator">typeof</a>(<code class="typeparamref">TConfiguration</code>).
</p>
<p>
Because no existing instance is provided, the underlying serializer constructs a fresh
<code class="typeparamref">TConfiguration</code> instance using its default constructor and fills
all properties with their default values before serializing. The result is therefore
useful for generating template or scaffold configuration files.
</p>

#### Exceptions

 [InvalidConfigurationException](Void.Proxy.Api.Configurations.Exceptions.InvalidConfigurationException.md)

Thrown when the serializer cannot construct a default <code class="typeparamref">TConfiguration</code>
instance or cannot convert it to the target configuration format.

#### See Also

[IConfigurationSerializer](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md).[Serialize](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md\#Void\_Proxy\_Api\_Configurations\_Serializer\_IConfigurationSerializer\_Serialize\_\_1\_\_\_0\_)<TConfiguration\>\(TConfiguration?\), 
[IConfigurationSerializer](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md).[Serialize](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md\#Void\_Proxy\_Api\_Configurations\_Serializer\_IConfigurationSerializer\_Serialize\_System\_Object\_System\_Type\_)\([object](https://learn.microsoft.com/dotnet/api/system.object)?, [Type](https://learn.microsoft.com/dotnet/api/system.type)\)

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

Serializes the given configuration object — or a default instance of
<code class="paramref">configurationType</code> when <code class="paramref">configuration</code> is
<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> — to configuration text.

```csharp
string Serialize(object? configuration, Type configurationType)
```

#### Parameters

`configuration` [object](https://learn.microsoft.com/dotnet/api/system.object)?

The configuration object to serialize, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> to serialize a freshly
constructed default instance of <code class="paramref">configurationType</code>.

`configurationType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

The <xref href="System.Type" data-throw-if-not-resolved="false"></xref> that describes the configuration structure.
When <code class="paramref">configuration</code> is non-null, this must match or be a base type of the
runtime type of <code class="paramref">configuration</code>.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A <xref href="System.String" data-throw-if-not-resolved="false"></xref> containing the serialized TOML representation of the configuration object.

#### Examples

<pre><code class="lang-csharp">// Serialize an existing configuration instance with an explicit type.
string toml = serializer.Serialize(myConfig, typeof(NetworkConfiguration));

// Serialize a default instance by passing null for the configuration argument.
string defaults = serializer.Serialize(null, typeof(NetworkConfiguration));</code></pre>

#### Remarks

<p>
This is the canonical serialization overload to which all other <code>Serialize</code> overloads
ultimately delegate. It performs the following steps:
</p>
<p>
<ol><li>
    If <code class="paramref">configuration</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>, a new instance of
    <code class="paramref">configurationType</code> is created with all properties set to their default
    values.
    </li><li>
    The object is mapped to a <code>TomlDocument</code> together with any inline or preceding comments
    declared via <xref href="Void.Proxy.Api.Configurations.Attributes.ConfigurationAttribute" data-throw-if-not-resolved="false"></xref>.
    </li><li>
    The serialized TOML text is returned via <code>TomlDocument.SerializedValue</code>.
    </li></ol>
</p>

#### Exceptions

 [InvalidConfigurationException](Void.Proxy.Api.Configurations.Exceptions.InvalidConfigurationException.md)

Thrown when the underlying TOML library raises a <code>TomlException</code> during object mapping
or serialization. The exception message includes the original TOML error details.

#### See Also

[IConfigurationSerializer](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md).[Serialize](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md\#Void\_Proxy\_Api\_Configurations\_Serializer\_IConfigurationSerializer\_Serialize\_\_1)<TConfiguration\>\(\), 
[IConfigurationSerializer](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md).[Serialize](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md\#Void\_Proxy\_Api\_Configurations\_Serializer\_IConfigurationSerializer\_Serialize\_\_1\_\_\_0\_)<TConfiguration\>\(TConfiguration?\), 
[IConfigurationSerializer](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md).[Deserialize](Void.Proxy.Api.Configurations.Serializer.IConfigurationSerializer.md\#Void\_Proxy\_Api\_Configurations\_Serializer\_IConfigurationSerializer\_Deserialize\_System\_String\_System\_Type\_)\([string](https://learn.microsoft.com/dotnet/api/system.string), [Type](https://learn.microsoft.com/dotnet/api/system.type)\)

