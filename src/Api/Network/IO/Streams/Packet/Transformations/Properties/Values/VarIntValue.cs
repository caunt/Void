using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record VarIntValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public int AsPrimitive => new MinecraftBuffer(Value.Span).ReadVarInt();

    public static VarIntValue FromPrimitive(int value)
    {
        return new VarIntValue(MinecraftBuffer.EnumerateVarInt(value).ToArray());
    }
}
