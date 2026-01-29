using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;

namespace Void.Minecraft.Commands.Brigadier.Network.Arguments;

public record RegistryKeyArgument(string Identifier) : IArgumentType<string>
{
    public IEnumerable<string> Examples => ["foo", "foo:bar", "012"];

    public string Parse(StringReader reader)
    {
        return reader.ReadString();
    }
}
