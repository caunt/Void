namespace Void.Proxy.Api.Configurations.Exceptions;

/// <summary>
/// Represents an error raised when configuration data cannot be serialized, parsed, or mapped to the expected configuration type.
/// </summary>
/// <param name="message">The detail message describing the configuration failure.</param>
public class InvalidConfigurationException(string message) : Exception(message);
