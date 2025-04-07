using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Streams.Packet.Transformations.Properties;

public interface IPacketProperty
{
    public void Write(ref MinecraftBuffer buffer);

    public virtual TCastValue As<TCastValue>() where TCastValue : IPacketProperty
    {
        if (this is not TCastValue value)
            throw new InvalidCastException($"Property value {this} cannot be casted to {typeof(TCastValue)}");

        return value;
    }
}

public interface IPacketProperty<TPacketProperty> : IPacketProperty where TPacketProperty : IPacketProperty<TPacketProperty>
{
    public static abstract TPacketProperty Read(ref MinecraftBuffer buffer);
}
