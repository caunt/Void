using System.Collections.Generic;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey;

public record RegistryKeyArgumentValue(string Value) : IArgumentValue;
public record RegistryKeyArgumentType(string Identifier) : IArgumentType
{
    public IEnumerable<string> Examples => ["foo", "foo:bar", "012"];

    public IArgumentValue Parse(StringReader reader)
    {
        return new RegistryKeyArgumentValue(reader.ReadString());
    }
}
