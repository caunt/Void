using System.Diagnostics.CodeAnalysis;
using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.Plugins.Common.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Network.Protocol.Packets;

namespace Void.Proxy.Plugins.Common.Registries.Packets;

public class PacketRegistry : IPacketRegistry
{
    private readonly Dictionary<int, Type> _mappings = [];
    private readonly Dictionary<Type, int> _reverseMappings = [];

    public IEnumerable<Type> PacketTypes => _mappings.Values;
    public bool IsEmpty => this is { _mappings.Count: 0, _reverseMappings.Count: 0 };

    public bool Contains<T>() where T : IMinecraftPacket
    {
        return _reverseMappings.Keys.Any(typeof(T).IsAssignableFrom);
    }

    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out PacketDecoder<IMinecraftPacket> packet)
    {
        packet = _mappings.TryGetValue(id, out var type) ? type.GetMethod(nameof(IMinecraftPacket<IMinecraftPacket>.Decode))!.CreateDelegate<PacketDecoder<IMinecraftPacket>>() : null;
        return packet != null;
    }

    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id)
    {
        return _reverseMappings.TryGetValue(packet.GetType(), out id);
    }

    public IPacketRegistry ReplacePackets(IReadOnlyDictionary<PacketMapping[], Type> registry, ProtocolVersion protocolVersion)
    {
        Clear();
        AddPackets(registry, protocolVersion);

        return this;
    }

    public IPacketRegistry AddPackets(IReadOnlyDictionary<PacketMapping[], Type> registry, ProtocolVersion protocolVersion)
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