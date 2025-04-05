using Nito.Collections;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Binary;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Transformations;

public class MinecraftBinaryPacketWrapper(IMinecraftBinaryMessage message) : IMinecraftBinaryPacketWrapper
{
    private readonly Deque<PacketProperty> _read = [];
    private readonly List<PacketProperty> _write = [];

    public TPropertyValue Get<TPropertyValue>(IPropertyType<TPropertyValue> type, int index) where TPropertyValue : class, IPropertyValue
    {
        if (!TryGet(type, index, out var value))
            throw new InvalidOperationException($"Property value of type {type} at index {index} not found in packet.");

        return value;
    }

    public bool TryGet<TPropertyValue>(IPropertyType<TPropertyValue> type, int index, [MaybeNullWhen(false)] out TPropertyValue value) where TPropertyValue : class, IPropertyValue
    {
        foreach (var property in _write)
        {
            if (property.Type != type)
                continue;

            if (--index > 0)
                continue;

            value = property.Value.As<TPropertyValue>();
            return true;
        }

        value = null;
        return false;
    }

    public void Set<TPropertyValue>(IPropertyType<TPropertyValue> type, int index, TPropertyValue value) where TPropertyValue : class, IPropertyValue
    {
        if (!TrySet(type, index, value))
            throw new InvalidOperationException($"Property value of type {type} at index {index} not found in packet.");
    }

    public bool TrySet<TPropertyValue>(IPropertyType<TPropertyValue> type, int index, TPropertyValue value) where TPropertyValue : class, IPropertyValue
    {
        for (var i = 0; i < _write.Count; i++)
        {
            var property = _write[i];

            if (property.Type != type)
                continue;

            if (--index > 0)
                continue;

            _write[i] = new PacketProperty(type, value);
            return true;
        }

        return false;
    }

    public TPropertyValue Passthrough<TPropertyValue>(IPropertyType<TPropertyValue> type) where TPropertyValue : class, IPropertyValue
    {
        var property = ReadProperty(type);
        _write.Add(property);

        return property.Value.As<TPropertyValue>();
    }

    public TPropertyValue Read<TPropertyValue>(IPropertyType<TPropertyValue> type) where TPropertyValue : IPropertyValue
    {
        return ReadProperty(type).Value.As<TPropertyValue>();
    }

    public void Write<TPropertyValue>(IPropertyType<TPropertyValue> type, TPropertyValue value) where TPropertyValue : IPropertyValue
    {
        _write.Add(new PacketProperty(type, value));
    }

    private PacketProperty ReadProperty<TPropertyValue>(IPropertyType<TPropertyValue> type) where TPropertyValue : IPropertyValue
    {
        return _read.Count switch
        {
            0 => new PacketProperty(type, ReadFromBuffer(type)),
            _ => TakeFromRead()
        };
    }

    private TPropertyValue ReadFromBuffer<TPropertyValue>(IPropertyType<TPropertyValue> type) where TPropertyValue : IPropertyValue
    {
        var buffer = GetBuffer();
        return type.Read(ref buffer);
    }

    private PacketProperty TakeFromRead()
    {
        return _read.RemoveFromFront();
    }

    private MinecraftBuffer GetBuffer()
    {
        return new MinecraftBuffer(message.Stream);
    }
}
