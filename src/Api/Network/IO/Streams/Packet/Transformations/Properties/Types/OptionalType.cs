using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record OptionalType<TPropertyValue>(IPropertyType<TPropertyValue> Type) : IOptionalPropertyType<TPropertyValue> where TPropertyValue : IPropertyValue
{
    public TPropertyValue? Read(ref MinecraftBuffer buffer)
    {
        var isPresent = buffer.ReadBoolean();
        if (!isPresent)
            return default;
        
        return Type.Read(ref buffer);
    }

    public void Write(ref MinecraftBuffer buffer, TPropertyValue? value)
    {
        buffer.WriteBoolean(value is not null);
        if (value is null)
            return;
        
        Type.Write(ref buffer, value);
    }
}
