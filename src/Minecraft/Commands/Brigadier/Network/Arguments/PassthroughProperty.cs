using System;
using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Network.Serializers;

namespace Void.Minecraft.Commands.Brigadier.Network.Arguments;

public record PassthroughProperty<T>(string Identifier, IArgumentPropertySerializer<T> Serializer, T Result) : IArgumentType<T>
{
    public IEnumerable<string> Examples => throw new NotSupportedException();

    public T Parse(StringReader reader) => throw new NotSupportedException();
}
