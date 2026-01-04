using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Network;

namespace Void.Proxy.Plugins.Common.Network.Registries.Transformations;

public class MinecraftPacketTransformationsRegistry : IMinecraftPacketTransformationsRegistry
{
    private readonly Dictionary<Type, MinecraftPacketTransformation[]> _upgradeMappings = [];
    private readonly Dictionary<Type, MinecraftPacketTransformation[]> _downgradeMappings = [];

    public IEnumerable<Type> PacketTypes => _upgradeMappings.Keys;
    public bool IsEmpty => this is { _upgradeMappings.Count: 0, _downgradeMappings.Count: 0 };

    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket
    {
        return Contains(typeof(T), type);
    }

    public bool Contains(IMinecraftMessage message, TransformationType type)
    {
        return Contains(message.GetType(), type);
    }

    public bool Contains(Type packetType, TransformationType transformationType)
    {
        return GetMappings(transformationType).Keys.Any(packetType.IsAssignableFrom);
    }

    public bool TryGetTransformations(Type packetType, TransformationType type, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformation)
    {
        return GetMappings(type).TryGetValue(packetType, out transformation);
    }

    public IMinecraftPacketTransformationsRegistry ReplaceTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> transformations, ProtocolVersion protocolVersion)
    {
        Clear();
        AddTransformations(transformations, protocolVersion);

        return this;
    }

    public IMinecraftPacketTransformationsRegistry AddTransformations(IReadOnlyDictionary<MinecraftPacketTransformationMapping[], Type> transformationMappings, ProtocolVersion protocolVersion)
    {
        foreach (var (mappings, type) in transformationMappings)
        {
            var mappingsToUpgrade = new List<MinecraftPacketTransformationMapping>();
            var mappingsToDowngrade = new List<MinecraftPacketTransformationMapping>();

            foreach (var mapping in mappings)
            {
                if (mapping.From <= protocolVersion && mapping.To <= protocolVersion)
                    continue;

                if (mapping.From > mapping.To)
                    mappingsToDowngrade.Add(mapping);
                else
                    mappingsToUpgrade.Add(mapping);
            }

            mappingsToUpgrade.Sort((a, b) => a.From.CompareTo(b.From));
            mappingsToDowngrade.Sort((a, b) => b.From.CompareTo(a.From));

            var upgradeTransformers = mappingsToUpgrade.Select(i => i.Transformation);
            var downgradeTransformers = mappingsToDowngrade.Select(i => i.Transformation);

            if (!upgradeTransformers.Any() && !downgradeTransformers.Any())
                continue;

            if (!_upgradeMappings.TryAdd(type, [.. upgradeTransformers]))
                throw new ArgumentException($"Upgrade transformations for {type} are already registered");

            if (!_downgradeMappings.TryAdd(type, [.. downgradeTransformers]))
                throw new ArgumentException($"Downgrade transformations for {type} are already registered");
        }

        return this;
    }

    public void Clear()
    {
        _upgradeMappings.Clear();
        _downgradeMappings.Clear();
    }

    public void Clear(Direction direction)
    {
        Clear(direction, _upgradeMappings);
        Clear(direction, _downgradeMappings);
    }

    private static void Clear(Direction direction, Dictionary<Type, MinecraftPacketTransformation[]> mappings)
    {
        var directionType =
            direction is Direction.Clientbound ? typeof(IMinecraftClientboundPacket) :
            direction is Direction.Serverbound ? typeof(IMinecraftServerboundPacket) :
            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);

        var filtered = mappings.Where(mapping => mapping.Key.IsAssignableTo(directionType)).ToArray();

        foreach (var (type, _) in filtered)
            mappings.Remove(type);
    }

    private Dictionary<Type, MinecraftPacketTransformation[]> GetMappings(TransformationType type) => type switch
    {
        TransformationType.Upgrade => _upgradeMappings,
        TransformationType.Downgrade => _downgradeMappings,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, string.Empty)
    };
}
