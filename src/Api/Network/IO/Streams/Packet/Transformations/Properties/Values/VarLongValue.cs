using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record VarLongValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public long AsPrimitive => new MinecraftBuffer(Value.Span).ReadVarInt();

    public static VarLongValue FromPrimitive(int value)
    {
        return new VarLongValue(MinecraftBuffer.EnumerateVarInt(value).ToArray());
    }
}
