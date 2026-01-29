using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;

namespace Void.Minecraft.Commands.Brigadier.Network.Arguments;

public class RegistryKeyArgument(string identifier) : IArgumentType<string>
{
    public string Identifier => identifier;
    public IEnumerable<string> Examples => ["foo", "foo:bar", "012"];

    public string Parse(StringReader reader)
    {
        return reader.ReadString();
    }
}
