using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record OptionalProperty<TPacketProperty>(TPacketProperty? Value = null) : IPacketProperty<OptionalProperty<TPacketProperty>> where TPacketProperty : class, IPacketProperty<TPacketProperty>
{
    public static OptionalProperty<TPacketProperty> Read(ref MinecraftBuffer buffer)
    {
        var isPresent = buffer.ReadBoolean();

        if (!isPresent)
            return new();

        return new(TPacketProperty.Read(ref buffer));
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteBoolean(Value is not null);
        Value?.Write(ref buffer);
    }
}
