using System.Diagnostics.CodeAnalysis;
using Void.Common.Network;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Minecraft.Network.Registries.Transformations.Mappings;

public interface IMinecraftBinaryPacketWrapper
{
    public Side Origin { get; }

    public bool TryGet<TPropertyValue>(int index, [MaybeNullWhen(false)] out TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>;
    public TPropertyValue Get<TPropertyValue>(int index) where TPropertyValue : IPacketProperty<TPropertyValue>;
    public bool TrySet<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>;
    public void Set<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>;
    public TPropertyValue Read<TPropertyValue>() where TPropertyValue : IPacketProperty<TPropertyValue>;
    public void Write<TPropertyValue>(TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>;
    public TPropertyValue Passthrough<TPropertyValue>() where TPropertyValue : IPacketProperty<TPropertyValue>;
    public void WriteProcessedValues(ref MinecraftBuffer buffer);
    public void Reset();
}
