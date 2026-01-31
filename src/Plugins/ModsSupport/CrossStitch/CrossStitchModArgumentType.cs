using Void.Minecraft.Buffers;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Registry;
using StringReader = Void.Minecraft.Commands.Brigadier.StringReader;

namespace Void.Proxy.Plugins.ModsSupport.CrossStitch;

public record CrossStitchModArgumentType(ArgumentSerializerMapping Mapping, BufferMemory Data) : IArgumentType
{
    public IEnumerable<string> Examples => throw new NotSupportedException();

    public IArgumentValue Parse(StringReader reader) => throw new NotImplementedException();
}
