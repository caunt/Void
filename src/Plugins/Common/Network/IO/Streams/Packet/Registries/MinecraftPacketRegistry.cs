using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Registries;

public class MinecraftPacketRegistry : IMinecraftPacketRegistry
{
    private readonly Dictionary<int, Type> _mappings = [];
    private readonly Dictionary<Type, int> _reverseMappings = [];

    public IEnumerable<Type> PacketTypes => _mappings.Values;
    public bool IsEmpty => this is { _mappings.Count: 0, _reverseMappings.Count: 0 };

    public bool Contains<T>() where T : IMinecraftPacket
    {
        return Contains(typeof(T));
    }

    public bool Contains(IMinecraftMessage message)
    {
        return Contains(message.GetType());
    }

    public bool Contains(Type type)
    {
        return _reverseMappings.Keys.Any(type.IsAssignableFrom);
    }

    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out MinecraftPacketDecoder<IMinecraftPacket> packet)
    {
        packet = null;

        if (!_mappings.TryGetValue(id, out var type))
            return false;

        var decodeMethod = type.GetMethod(nameof(IMinecraftPacket<IMinecraftPacket>.Decode));

        if (decodeMethod is null)
            return false;

        packet = decodeMethod.CreateDelegate<MinecraftPacketDecoder<IMinecraftPacket>>();
        return true;
    }

    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id)
    {
        return _reverseMappings.TryGetValue(packet.GetType(), out id);
    }

    public IMinecraftPacketRegistry ReplacePackets(IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> registry, ProtocolVersion protocolVersion)
    {
        Clear();
        AddPackets(registry, protocolVersion);

        return this;
    }

    public IMinecraftPacketRegistry AddPackets(IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> registry, ProtocolVersion protocolVersion)
    {
        foreach (var (mappings, type) in registry)
        {
            if (mappings.Length == 0)
                continue;

            for (var i = 0; i < mappings.Length; i++)
            {
                var current = mappings[i];
                var next = i + 1 < mappings.Length ? mappings[i + 1] : current;

                var from = current.ProtocolVersion;
                var to = current.LastValidProtocolVersion ?? (current.Equals(next) ? ProtocolVersion.Latest : next.ProtocolVersion - 1);

                if (from.CompareTo(to) > 0)
                    throw new ArgumentException($"Next mapping version ({to}) should be lower than the current ({from})");

                if (!ProtocolVersion.Range(from, to).Contains(protocolVersion))
                    continue;

                if (!_mappings.TryAdd(current.Id, type) || !_reverseMappings.TryAdd(type, current.Id))
                    throw new ArgumentException($"{type} cannot be registered with packet Id {current.Id}, because there is already {_mappings[current.Id].FullName}");
            }
        }

        return this;
    }

    public void Clear()
    {
        _mappings.Clear();
        _reverseMappings.Clear();
    }
}