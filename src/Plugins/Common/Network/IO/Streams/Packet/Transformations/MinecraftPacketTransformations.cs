using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Transformations;

public class MinecraftPacketTransformations : IMinecraftPacketTransformations
{
    private readonly Dictionary<MinecraftPacketTransformation, Type> _mappings = [];
    private readonly Dictionary<Type, MinecraftPacketTransformation> _reverseMappings = [];

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

    public bool TryGetTransformation(IMinecraftPacket packet, [MaybeNullWhen(false)] out MinecraftPacketTransformation transformation)
    {
        return _reverseMappings.TryGetValue(packet.GetType(), out transformation);
    }

    public IMinecraftPacketTransformations ReplaceTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> transformations, ProtocolVersion protocolVersion)
    {
        Clear();
        AddTransformations(transformations, protocolVersion);

        return this;
    }

    public IMinecraftPacketTransformations AddTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> transformations, ProtocolVersion protocolVersion)
    {
        foreach (var (mappings, type) in transformations)
        {
            foreach (var mapping in mappings)
            {
                if (!ProtocolVersion.Range(mapping.From, mapping.To).Contains(protocolVersion))
                    continue;

                if (!_mappings.TryAdd(mapping.Transformation, type) || !_reverseMappings.TryAdd(type, mapping.Transformation))
                    throw new ArgumentException($"{type} cannot be registered with packet Transformation {mapping.Transformation}, because there is already {_mappings[mapping.Transformation].FullName}");
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