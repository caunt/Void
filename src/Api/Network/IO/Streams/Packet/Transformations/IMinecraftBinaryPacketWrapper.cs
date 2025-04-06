using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;

public interface IMinecraftBinaryPacketWrapper
{
    public bool TryGet<TPropertyValue>(int index, [MaybeNullWhen(false)] out TPropertyValue value) where TPropertyValue : class, IPacketProperty<TPropertyValue>;
    public TPropertyValue Get<TPropertyValue>(int index) where TPropertyValue : class, IPacketProperty<TPropertyValue>;
    public bool TrySet<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : class, IPacketProperty<TPropertyValue>;
    public void Set<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : class, IPacketProperty<TPropertyValue>;
    public TPropertyValue Read<TPropertyValue>() where TPropertyValue : class, IPacketProperty<TPropertyValue>;
    public void Write<TPropertyValue>(TPropertyValue value) where TPropertyValue : class, IPacketProperty<TPropertyValue>;
    public TPropertyValue Passthrough<TPropertyValue>() where TPropertyValue : class, IPacketProperty<TPropertyValue>;
    public void WriteProcessedValues(ref MinecraftBuffer buffer);
    public void Reset();
}
