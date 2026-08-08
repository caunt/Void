using System.Collections.Generic;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey;

/// <summary>Contains a parsed registry-key string.</summary>
/// <param name="Value">The parsed value.</param>
public record RegistryKeyArgumentValue(string Value) : IArgumentValue;
/// <summary>Parses a registry key while retaining the registry identifier required by the protocol declaration.</summary>
/// <param name="Identifier">The registry identifier serialized with the command node.</param>
public record RegistryKeyArgumentType(string Identifier) : IArgumentType
{
    /// <inheritdoc/>
    public IEnumerable<string> Examples => ["foo", "foo:bar", "012"];

    /// <inheritdoc/>
    public IArgumentValue Parse(StringReader reader)
    {
        return new RegistryKeyArgumentValue(reader.ReadString());
    }
}
