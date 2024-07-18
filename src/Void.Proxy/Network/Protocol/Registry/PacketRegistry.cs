using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.Network.Protocol.Registry;

public class PacketRegistry
{
    private readonly Direction _direction;
    private readonly bool _fallback = true;
    private readonly Dictionary<ProtocolVersion, ProtocolRegistry> _versions = [];

    public PacketRegistry(Direction direction)
    {
        _direction = direction;

        foreach (var version in ProtocolVersion.Range())
            _versions.Add(version, new ProtocolRegistry(direction, version));
    }

    public ProtocolRegistry GetProtocolRegistry(ProtocolVersion version)
    {
        if (!_versions.TryGetValue(version, out var registry))
        {
            if (_fallback && _versions.TryGetValue(ProtocolVersion.Oldest, out registry))
                return registry;

            throw new KeyNotFoundException($"Could not find Registry for {version} protocol version ");
        }

        return registry;
    }

    public void Register<T>(Func<T> factory, params PacketMapping[] mappings) where T : IMinecraftPacket
    {
        if (mappings.Length == 0)
            throw new ArgumentException("At least one mapping must be provided.");

        // TODO uncomment Proxy.Logger.Verbose($"Registering {_direction} {typeof(T).Name} with {mappings.Length} mappings");

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

            var to = current == next ? lastValid ?? ProtocolVersion.Latest : next.ProtocolVersion;
            var lastInList = lastValid ?? ProtocolVersion.Latest;

            if (from.CompareTo(to) >= 0 && from != lastInList)
                throw new ArgumentException($"Next mapping version ({to}) should be lower than the current ({from})");

            var type = typeof(T);

            foreach (var protocol in ProtocolVersion.Range(from, to))
            {
                if (protocol == to && !next.Equals(current))
                    break;

                if (!_versions.TryGetValue(protocol, out var registry))
                    throw new ArgumentException($"Unknown protocol version {current.ProtocolVersion}");

                if (registry.PacketIdToFactory.ContainsKey(current.Id))
                    throw new ArgumentException($"Cannot register class {type.Name} with id {current.Id} for {registry.Version} because another packet is already registered");

                if (registry.PacketTypeToId.ContainsKey(type))
                    throw new ArgumentException($"{type.Name} is already registered for version {registry.Version}");

                if (!current.EncodeOnly)
                    registry.PacketIdToFactory[current.Id] = () => factory();

                registry.PacketTypeToId[type] = current.Id;
            }
        }
    }
}