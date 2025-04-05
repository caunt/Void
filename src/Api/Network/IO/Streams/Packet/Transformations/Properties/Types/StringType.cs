using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record StringType : IPropertyType<StringValue>
{
    public StringValue Read(ref MinecraftBuffer buffer)
    {
        return StringValue.FromPrimitive(buffer.ReadString());
    }

    public void Write(ref MinecraftBuffer buffer, StringValue value)
    {
        buffer.WriteString(value.AsPrimitive);
    }
}
