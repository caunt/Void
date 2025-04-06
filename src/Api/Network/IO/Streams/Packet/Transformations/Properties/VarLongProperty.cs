using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties;

public record VarLongProperty(ReadOnlyMemory<byte> Value) : IPacketProperty
{
    public long AsPrimitive => new MinecraftBuffer(Value.Span).ReadVarInt();

    public static VarLongProperty FromPrimitive(int value)
    {
        return new VarLongProperty(MinecraftBuffer.EnumerateVarInt(value).ToArray());
    }

    public static VarLongProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadVarInt());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarLong(AsPrimitive);
    }
}
