using System.Diagnostics.CodeAnalysis;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;
using Void.Proxy.Common.Network.Protocol;

namespace Void.Proxy.Common.Registries.Packets;

public class PacketRegistry : IPacketRegistry
{
    private readonly Dictionary<int, Type> _mappings = [];
    private readonly Dictionary<Type, int> _reverseMappings = [];

    public bool IsEmpty => this is { _mappings.Count: 0, _reverseMappings.Count: 0 };

    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out PacketDecoder<IMinecraftPacket> packet)
    {
        packet = _mappings.TryGetValue(id, out var type) ? type.GetMethod(nameof(IMinecraftPacket<IMinecraftPacket>.Decode))!.CreateDelegate<PacketDecoder<IMinecraftPacket>>() : null;
        return packet != null;
    }

    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id)
    {
        return _reverseMappings.TryGetValue(packet.GetType(), out id);
    }

    public IPacketRegistry ReplacePackets(IReadOnlyDictionary<PacketMapping[], Type> registry, ProtocolVersion? maximumSupportedProtocolVersion = null)
    {
        Clear();
        AddPackets(registry, maximumSupportedProtocolVersion);

        return this;
    }

    public IPacketRegistry AddPackets(IReadOnlyDictionary<PacketMapping[], Type> registry, ProtocolVersion? maximumSupportedProtocolVersion = null)
    {
        maximumSupportedProtocolVersion ??= ProtocolVersion.Latest;

        foreach (var (mappings, type) in registry)
        {
            if (mappings.Length == 0)
                continue;

            for (var i = 0; i < mappings.Length; i++)
            {
                var current = mappings[i];
                var next = i + 1 < mappings.Length ? mappings[i + 1] : current;

                var from = current.ProtocolVersion;
                var lastValid = current.LastValidProtocolVersion;

                if (lastValid != null)
                {
                    if (!next.Equals(current))
                        throw new ArgumentException("Cannot add a mapping after the last valid mapping");

                    if (from.CompareTo(lastValid) > 0)
                        throw new ArgumentException("Last mapping version cannot be higher than the highest mapping version");
                }

                var to = current.Equals(next) ? lastValid ?? maximumSupportedProtocolVersion : next.ProtocolVersion;
                var lastInList = lastValid ?? maximumSupportedProtocolVersion;

                if (from.CompareTo(to) >= 0 && from != lastInList)
                    throw new ArgumentException($"Next mapping version ({to}) should be lower than the current ({from})");

                // if (from > ProtocolVersion || to < ProtocolVersion)
                //     continue;

                if (!_mappings.TryAdd(current.Id, type))
                    throw new ArgumentException($"{type} is already registered for packet Id {current.Id}");

                _reverseMappings.Add(type, current.Id);
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