using System.Diagnostics.CodeAnalysis;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;
using Void.Proxy.Common.Network.Protocol;
using Void.Proxy.Common.Registries.Packets;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

public class PacketRegistry : IPacketRegistry
{
    private readonly Dictionary<int, Type> _mappings = [];
    private readonly Dictionary<Type, int> _reverseMappings = [];

    public required IReadOnlyDictionary<PacketMapping[], Type> Mappings
    {
        set => RegisterPackets(value);
    }

    public required ProtocolVersion ProtocolVersion { get; init; }

    public bool TryCreateDecoder(int id, [MaybeNullWhen(false)] out PacketDecoder<IMinecraftPacket> packet)
    {
        packet = _mappings.TryGetValue(id, out var type) ? type.GetMethod(nameof(HandshakePacket.Decode))!.CreateDelegate<PacketDecoder<IMinecraftPacket>>() : null;
        return packet != null;
    }

    public bool TryGetPacketId(IMinecraftPacket packet, [MaybeNullWhen(false)] out int id)
    {
        return _reverseMappings.TryGetValue(packet.GetType(), out id);
    }

    public void RegisterPackets(IReadOnlyDictionary<PacketMapping[], Type> registry)
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
                var lastValid = current.LastValidProtocolVersion;

                if (lastValid != null)
                {
                    if (!next.Equals(current))
                        throw new ArgumentException("Cannot add a mapping after the last valid mapping");

                    if (from.CompareTo(lastValid) > 0)
                        throw new ArgumentException("Last mapping version cannot be higher than the highest mapping version");
                }

                var to = current.Equals(next) ? lastValid ?? Plugin.SupportedVersions.Last() : next.ProtocolVersion;
                var lastInList = lastValid ?? Plugin.SupportedVersions.Last();

                if (from.CompareTo(to) >= 0 && from != lastInList)
                    throw new ArgumentException($"Next mapping version ({to}) should be lower than the current ({from})");

                if (from > ProtocolVersion || to < ProtocolVersion)
                    continue;

                if (!_mappings.TryAdd(current.Id, type))
                    throw new ArgumentException($"{type} is already registered for packet Id {current.Id}");

                _reverseMappings.Add(type, current.Id);
            }
        }
    }
}