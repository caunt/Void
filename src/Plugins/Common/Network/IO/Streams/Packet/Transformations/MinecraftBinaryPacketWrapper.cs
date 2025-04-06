using Nito.Collections;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Binary;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Transformations;

public class MinecraftBinaryPacketWrapper(IMinecraftBinaryMessage message) : IMinecraftBinaryPacketWrapper
{
    private readonly Deque<IPacketProperty> _read = [];
    private readonly List<IPacketProperty> _write = [];

    public TPropertyValue Get<TPropertyValue>(int index) where TPropertyValue : class, IPacketProperty<TPropertyValue>
    {
        if (!TryGet<TPropertyValue>(index, out var value))
            throw new InvalidOperationException($"Property value of type {typeof(TPropertyValue)} at index {index} not found in packet.");

        return value;
    }

    public bool TryGet<TPropertyValue>(int index, [MaybeNullWhen(false)] out TPropertyValue value) where TPropertyValue : class, IPacketProperty<TPropertyValue>
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

        value = null;
        return false;
    }

    public void Set<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : class, IPacketProperty<TPropertyValue>
    {
        if (!TrySet(index, value))
            throw new InvalidOperationException($"Property value of type {typeof(TPropertyValue)} at index {index} not found in packet.");
    }

    public bool TrySet<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : class, IPacketProperty<TPropertyValue>
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

    public TPropertyValue Passthrough<TPropertyValue>() where TPropertyValue : class, IPacketProperty<TPropertyValue>
    {
        var property = ReadProperty<TPropertyValue>();
        _write.Add(property);

        return property.As<TPropertyValue>();
    }

    public TPropertyValue Read<TPropertyValue>() where TPropertyValue : class, IPacketProperty<TPropertyValue>
    {
        return ReadProperty<TPropertyValue>().As<TPropertyValue>();
    }

    public void Write<TPropertyValue>(TPropertyValue value) where TPropertyValue : class, IPacketProperty<TPropertyValue>
    {
        _write.Add(value);
    }

    public void ResetReader()
    {
        for (var i = _write.Count - 1; i >= 0; i--)
            _read.AddToFront(_write[i]);

        _write.Clear();
    }

    public void WriteProcessedValues(MinecraftBuffer buffer)
    {
        if (_read.Count == 0)
        {
            message.Stream.Position = 0;
            buffer.Write(message.Stream);
            return;
        }

        foreach (var item in _read)
            item.Write(ref buffer);
    }

    private TPropertyValue ReadProperty<TPropertyValue>() where TPropertyValue : class, IPacketProperty<TPropertyValue>
    {
        return _read.Count switch
        {
            0 => ReadFromBuffer<TPropertyValue>(),
            _ => TakeFromRead() as TPropertyValue ?? throw new InvalidOperationException($"Property value of type {typeof(TPropertyValue)} not found in packet.")
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
