namespace Void.Proxy.Api.Configurations.Serializer;

public interface IConfigurationSerializer
{
    /// <summary>
    /// Serializes a default-value instance of <typeparamref name="TConfiguration" /> to configuration text.
    /// </summary>
    /// <typeparam name="TConfiguration">
    /// The configuration type whose default representation is serialized.
    /// Must be a non-nullable reference or value type.
    /// </typeparam>
    /// <returns>
    /// A <see cref="string" /> containing the serialized configuration text for a freshly constructed
    /// <typeparamref name="TConfiguration" /> instance populated with its default property values.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This overload is a convenience wrapper that calls
    /// <see cref="Serialize{TConfiguration}(TConfiguration)" /> with <see langword="null" /> as the
    /// configuration argument, which in turn delegates to
    /// <see cref="Serialize(object, Type)" /> with <see langword="typeof" />(<typeparamref name="TConfiguration" />).
    /// </para>
    /// <para>
    /// Because no existing instance is provided, the underlying serializer constructs a fresh
    /// <typeparamref name="TConfiguration" /> instance using its default constructor and fills
    /// all properties with their default values before serializing. The result is therefore
    /// useful for generating template or scaffold configuration files.
    /// </para>
    /// </remarks>
    /// <exception cref="Void.Proxy.Api.Configurations.Exceptions.InvalidConfigurationException">
    /// Thrown when the serializer cannot construct a default <typeparamref name="TConfiguration" />
    /// instance or cannot convert it to the target configuration format.
    /// </exception>
    /// <example>
    /// <code>
    /// // Produce a template TOML configuration file with all default values.
    /// string template = serializer.Serialize&lt;NetworkConfiguration&gt;();
    /// await File.WriteAllTextAsync("network.toml", template);
    /// </code>
    /// </example>
    /// <seealso cref="Serialize{TConfiguration}(TConfiguration)" />
    /// <seealso cref="Serialize(object, Type)" />
    public string Serialize<TConfiguration>() where TConfiguration : notnull;
    public string Serialize(object configuration);
    public string Serialize<TConfiguration>(TConfiguration? configuration) where TConfiguration : notnull;
    /// <summary>
    /// Serializes the given configuration object — or a default instance of
    /// <paramref name="configurationType" /> when <paramref name="configuration" /> is
    /// <see langword="null" /> — to configuration text.
    /// </summary>
    /// <param name="configuration">
    /// The configuration object to serialize, or <see langword="null" /> to serialize a freshly
    /// constructed default instance of <paramref name="configurationType" />.
    /// </param>
    /// <param name="configurationType">
    /// The <see cref="Type" /> that describes the configuration structure.
    /// When <paramref name="configuration" /> is non-null, this must match or be a base type of the
    /// runtime type of <paramref name="configuration" />.
    /// </param>
    /// <returns>
    /// A <see cref="string" /> containing the serialized TOML representation of the configuration object.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is the canonical serialization overload to which all other <c>Serialize</c> overloads
    /// ultimately delegate. It performs the following steps:
    /// </para>
    /// <para>
    /// <list type="number">
    ///   <item>
    ///     <description>
    ///     If <paramref name="configuration" /> is <see langword="null" />, a new instance of
    ///     <paramref name="configurationType" /> is created with all properties set to their default
    ///     values.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     The object is mapped to a <c>TomlDocument</c> together with any inline or preceding comments
    ///     declared via <see cref="Void.Proxy.Api.Configurations.Attributes.ConfigurationAttribute" />.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     The serialized TOML text is returned via <c>TomlDocument.SerializedValue</c>.
    ///     </description>
    ///   </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <exception cref="Void.Proxy.Api.Configurations.Exceptions.InvalidConfigurationException">
    /// Thrown when the underlying TOML library raises a <c>TomlException</c> during object mapping
    /// or serialization. The exception message includes the original TOML error details.
    /// </exception>
    /// <example>
    /// <code>
    /// // Serialize an existing configuration instance with an explicit type.
    /// string toml = serializer.Serialize(myConfig, typeof(NetworkConfiguration));
    ///
    /// // Serialize a default instance by passing null for the configuration argument.
    /// string defaults = serializer.Serialize(null, typeof(NetworkConfiguration));
    /// </code>
    /// </example>
    /// <seealso cref="Serialize{TConfiguration}()" />
    /// <seealso cref="Serialize{TConfiguration}(TConfiguration)" />
    /// <seealso cref="Deserialize(string, Type)" />
    public string Serialize(object? configuration, Type configurationType);
    /// <summary>
    /// Deserializes configuration text into an instance of <typeparamref name="TConfiguration" />.
    /// </summary>
    /// <typeparam name="TConfiguration">
    /// The expected configuration type.
    /// </typeparam>
    /// <param name="source">
    /// The serialized configuration text to parse and map; this value must not be <see langword="null" />.
    /// </param>
    /// <returns>
    /// A configuration instance cast to <typeparamref name="TConfiguration" />.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This member is a typed wrapper around <see cref="Deserialize(string, Type)" /> that passes
    /// <see langword="typeof" />(<typeparamref name="TConfiguration" />) as the target type.
    /// </para>
    /// <para>
    /// The built-in serializer implementation parses TOML and maps it with configured options, then
    /// validates that the resulting object can be cast to <typeparamref name="TConfiguration" />.
    /// </para>
    /// </remarks>
    /// <exception cref="Void.Proxy.Api.Configurations.Exceptions.InvalidConfigurationException">
    /// Thrown when <paramref name="source" /> cannot be parsed, when it cannot be mapped to
    /// <typeparamref name="TConfiguration" />, or when the mapped result is not assignable to
    /// <typeparamref name="TConfiguration" />.
    /// </exception>
    /// <example>
    /// <code>
    /// var value = serializer.Deserialize&lt;NetworkConfiguration&gt;(sourceText);
    /// </code>
    /// </example>
    /// <seealso cref="Deserialize(string, Type)" />
    public TConfiguration Deserialize<TConfiguration>(string source) where TConfiguration : notnull;
    /// <summary>
    /// Deserializes configuration text into an instance of a runtime-specified configuration type.
    /// </summary>
    /// <param name="source">
    /// The serialized configuration text to parse and map; this value must not be <see langword="null" />.
    /// </param>
    /// <param name="configurationType">
    /// The target configuration <see cref="Type" /> to materialize.
    /// </param>
    /// <returns>
    /// The deserialized configuration object typed as <see cref="object" />.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The built-in serializer implementation first parses <paramref name="source" /> as TOML, then maps
    /// the parsed document to <paramref name="configurationType" /> using serializer options.
    /// </para>
    /// <para>
    /// This overload performs no compile-time type checks, so callers are responsible for casting
    /// the returned value to the expected runtime type.
    /// </para>
    /// </remarks>
    /// <exception cref="Void.Proxy.Api.Configurations.Exceptions.InvalidConfigurationException">
    /// Thrown when parsing fails or when the serialized content cannot be converted to
    /// <paramref name="configurationType" />.
    /// </exception>
    /// <example>
    /// <code>
    /// var value = serializer.Deserialize(sourceText, typeof(NetworkConfiguration));
    /// </code>
    /// </example>
    /// <seealso cref="Deserialize{TConfiguration}(string)" />
    /// <seealso cref="Type" />
    public object Deserialize(string source, Type configurationType);
}
