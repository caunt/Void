using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Transformations;

public class MinecraftPacketTransformations : IMinecraftPacketTransformations
{
    private readonly Dictionary<Type, MinecraftPacketTransformation[]> _upgradeMappings = [];
    private readonly Dictionary<Type, MinecraftPacketTransformation[]> _downgradeMappings = [];

    public IEnumerable<Type> PacketTypes => _upgradeMappings.Keys;
    public bool IsEmpty => this is { _upgradeMappings.Count: 0, _downgradeMappings.Count: 0 };

    public bool UpgradeContains<T>() where T : IMinecraftPacket
    {
        return UpgradeContains(typeof(T));
    }

    public bool UpgradeContains(IMinecraftMessage message)
    {
        return UpgradeContains(message.GetType());
    }

    public bool UpgradeContains(Type type)
    {
        return _upgradeMappings.Keys.Any(type.IsAssignableFrom);
    }

    public bool DowngradeContains<T>() where T : IMinecraftPacket
    {
        return DowngradeContains(typeof(T));
    }

    public bool DowngradeContains(IMinecraftMessage message)
    {
        return DowngradeContains(message.GetType());
    }

    public bool DowngradeContains(Type type)
    {
        return _downgradeMappings.Keys.Any(type.IsAssignableFrom);
    }

    public bool TryGetUpgradeTransformation(IMinecraftPacket packet, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformation)
    {
        return _upgradeMappings.TryGetValue(packet.GetType(), out transformation);
    }

    public bool TryGetDowngradeTransformation(IMinecraftPacket packet, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformation)
    {
        return _downgradeMappings.TryGetValue(packet.GetType(), out transformation);
    }

    public IMinecraftPacketTransformations ReplaceTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> transformations, ProtocolVersion protocolVersion)
    {
        Clear();
        AddTransformations(transformations, protocolVersion);

        return this;
    }

    public IMinecraftPacketTransformations AddTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> transformationMappings, ProtocolVersion protocolVersion)
    {
        // Dictionary<Type, MinecraftPacketTransformation[]> _reverseMappings
        
        foreach (var (mappings, type) in transformationMappings)
        {
            var mappingsToUpgrade = new List<MinecraftPacketTransformationMapping>();
            var mappingsToDowngrate = new List<MinecraftPacketTransformationMapping>();
            
            foreach (var mapping in mappings)
            {
                if (mapping.From < protocolVersion || mapping.To < protocolVersion)
                    continue;
                
                if (mapping.From > mapping.To)
                    mappingsToUpgrade.Add(mapping);
                else 
                    mappingsToDowngrate.Add(mapping);
            }
            
            mappingsToUpgrade.Sort((a, b) => a.From > b.From ? 1 : -1);
            mappingsToDowngrate.Sort((a, b) => a.From > b.From ? -1 : 1);
        
            var upgrateTransformers = mappingsToUpgrade.Select(i => i.Transformation);
            var downgradeTransformers = mappingsToDowngrate.Select(i => i.Transformation);

            if (!_upgradeMappings.TryAdd(type, upgrateTransformers.ToArray()))
                throw new ArgumentException($"{type} cannot be registered with packet upgrade transformations, because it is already registered");

            if (!_downgradeMappings.TryAdd(type, downgradeTransformers.ToArray()))
                throw new ArgumentException($"{type} cannot be registered with packet downgrade transformations, because it is already registered");
        }
        
        
        return this;
    }

    public void Clear()
    {
        _upgradeMappings.Clear();
        _downgradeMappings.Clear();
    }
}