using System;
using System.Collections.Generic;
using Void.Minecraft.Buffers;
using Void.Minecraft.Commands.Brigadier.Registry;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

// public record CrossStitchModArgumentValue(BufferMemory Data) : IArgumentValue;
public record CrossStitchModArgumentType(ArgumentSerializerMapping Mapping, BufferMemory Data) : IArgumentType
{
    public IEnumerable<string> Examples => throw new NotSupportedException();

    public IArgumentValue Parse(StringReader reader) => throw new NotImplementedException();
}
