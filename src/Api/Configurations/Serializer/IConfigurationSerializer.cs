namespace Void.Proxy.Api.Configurations.Serializer;

public interface IConfigurationSerializer
{
    public string Serialize<TConfiguration>() where TConfiguration : notnull;
    public string Serialize(object configuration);
    public string Serialize<TConfiguration>(TConfiguration? configuration) where TConfiguration : notnull;
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
