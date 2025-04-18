using Nito.Collections;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Messages.Binary;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;
using Void.Proxy.Api.Network;

namespace Void.Proxy.Plugins.Common.Network.Registries.Transformations.Mappings;

public class MinecraftBinaryPacketWrapper(IMinecraftBinaryMessage message, Side origin) : IMinecraftBinaryPacketWrapper
{
    private readonly Deque<IPacketProperty> _read = [];
    private readonly List<IPacketProperty> _write = [];

    public Side Origin => origin;

    public TPropertyValue Get<TPropertyValue>(int index) where TPropertyValue : IPacketProperty<TPropertyValue>
    {
        if (!TryGet<TPropertyValue>(index, out var value))
            throw new InvalidOperationException($"Property value of type {typeof(TPropertyValue)} at index {index} not found in packet.");

        return value;
    }

    public bool TryGet<TPropertyValue>(int index, [MaybeNullWhen(false)] out TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>
    {
        foreach (var property in _write)
        {
            if (property is not TPropertyValue)
                continue;

            if (--index > 0)
                continue;

            value = property.As<TPropertyValue>();
            return true;
        }

        value = default;
        return false;
    }

    public void Set<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>
    {
        if (!TrySet(index, value))
            throw new InvalidOperationException($"Property value of type {typeof(TPropertyValue)} at index {index} not found in packet.");
    }

    public bool TrySet<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>
    {
        for (var i = 0; i < _write.Count; i++)
        {
            var property = _write[i];

            if (property is not TPropertyValue)
                continue;

            if (--index > 0)
                continue;

            _write[i] = value;
            return true;
        }

        return false;
    }

    public TPropertyValue Passthrough<TPropertyValue>() where TPropertyValue : IPacketProperty<TPropertyValue>
    {
        var property = ReadProperty<TPropertyValue>();
        _write.Add(property);

        return property.As<TPropertyValue>();
    }

    public TPropertyValue Read<TPropertyValue>() where TPropertyValue : IPacketProperty<TPropertyValue>
    {
        return ReadProperty<TPropertyValue>().As<TPropertyValue>();
    }

    public void Write<TPropertyValue>(TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>
    {
        _write.Add(value);
    }

    public void Reset()
    {
        for (var i = _write.Count - 1; i >= 0; i--)
            _read.AddToFront(_write[i]);

        _write.Clear();
    }

    public void WriteProcessedValues(ref MinecraftBuffer buffer)
    {
        foreach (var property in _read)
            property.Write(ref buffer);

        if (_read.Count is 0)
            buffer.Write(message.Stream);
    }

    private TPropertyValue ReadProperty<TPropertyValue>() where TPropertyValue : IPacketProperty<TPropertyValue>
    {
        return _read.Count switch
        {
            0 => ReadFromBuffer<TPropertyValue>(),
            _ => (TPropertyValue?)TakeFromRead() ?? throw new InvalidOperationException($"Property value of type {typeof(TPropertyValue)} not found in packet.")
        };
    }

    private TPropertyValue ReadFromBuffer<TPropertyValue>() where TPropertyValue : IPacketProperty<TPropertyValue>
    {
        var buffer = GetBuffer();
        return TPropertyValue.Read(ref buffer);
    }

    private IPacketProperty TakeFromRead()
    {
        return _read.RemoveFromFront();
    }

    private MinecraftBuffer GetBuffer()
    {
        return new MinecraftBuffer(message.Stream);
    }
}
