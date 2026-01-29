using System;
using System.Collections.Generic;
using Void.Minecraft.Buffers;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;

namespace Void.Minecraft.Commands.Brigadier.Network.Arguments;

public record ModArgumentProperty(ArgumentIdentifier Identifier, BufferMemory Data) : IArgumentType<BufferMemory>
{
    public IEnumerable<string> Examples => throw new NotSupportedException();
    public BufferMemory Parse(StringReader reader) => throw new NotSupportedException();
}
