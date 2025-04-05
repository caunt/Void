using System.Diagnostics.CodeAnalysis;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;

public interface IMinecraftBinaryPacketWrapper
{
    public TPropertyValue Passthrough<TPropertyValue>(IPropertyType<TPropertyValue> type) where TPropertyValue : class, IPropertyValue;
    public TPropertyValue Read<TPropertyValue>(IPropertyType<TPropertyValue> type) where TPropertyValue : IPropertyValue;
    public bool TryGet<TPropertyValue>(IPropertyType<TPropertyValue> type, int index, [MaybeNullWhen(false)] out TPropertyValue value) where TPropertyValue : class, IPropertyValue;
    public bool TrySet<TPropertyValue>(IPropertyType<TPropertyValue> type, int index, TPropertyValue value) where TPropertyValue : class, IPropertyValue;
    public void Write<TPropertyValue>(IPropertyType<TPropertyValue> type, TPropertyValue value) where TPropertyValue : IPropertyValue;
}
