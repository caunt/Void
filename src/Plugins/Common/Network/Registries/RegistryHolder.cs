using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Minecraft.Network.Registries.Transformations;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Plugins.Common.Network.Registries.PacketId;
using Void.Proxy.Plugins.Common.Network.Registries.Transformations;

namespace Void.Proxy.Plugins.Common.Network.Registries;

public class RegistryHolder : IRegistryHolder
{
    public ProtocolVersion ProtocolVersion => PacketIdSystem.ProtocolVersion ?? ProtocolVersion.Oldest;

    public IMinecraftPacketIdSystemRegistry PacketIdSystem { get; } = new MinecraftPacketIdSystemRegistry();
    public IMinecraftPacketIdPluginsRegistry PacketIdPlugins { get; } = new MinecraftPacketIdPluginsRegistry();
    public IMinecraftPacketTransformationsSystemRegistry PacketTransformationsSystem { get; } = new MinecraftPacketTransformationsSystemRegistry();
    public IMinecraftPacketTransformationsPluginsRegistry PacketTransformationsPlugins { get; } = new MinecraftPacketTransformationsPluginsRegistry();

    public void Setup(IPlugin managedBy, ProtocolVersion protocolVersion)
    {
        if (PacketIdSystem.ManagedBy is not null)
            throw new InvalidOperationException($"System packet-id registry is already managed by {PacketIdSystem.ManagedBy.Name} ({PacketIdSystem.ManagedBy})");

        if (PacketIdPlugins.ManagedBy is not null)
            throw new InvalidOperationException($"Plugins packet-id registry is already managed by {PacketIdPlugins.ManagedBy.Name} ({PacketIdPlugins.ManagedBy})");

        if (PacketTransformationsSystem.ManagedBy is not null)
            throw new InvalidOperationException($"System packet-transformations registry is already managed by {PacketTransformationsSystem.ManagedBy.Name} ({PacketTransformationsSystem.ManagedBy})");

        if (PacketTransformationsPlugins.ManagedBy is not null)
            throw new InvalidOperationException($"Plugins packet-transformations registry is already managed by {PacketTransformationsPlugins.ManagedBy.Name} ({PacketTransformationsPlugins.ManagedBy})");

        PacketIdSystem.ManagedBy = managedBy;
        PacketIdSystem.ProtocolVersion = protocolVersion;

        PacketIdPlugins.ManagedBy = managedBy;
        PacketIdPlugins.ProtocolVersion = protocolVersion;

        PacketTransformationsSystem.ManagedBy = managedBy;
        PacketTransformationsSystem.ProtocolVersion = protocolVersion;

        PacketTransformationsPlugins.ManagedBy = managedBy;
        PacketTransformationsPlugins.ProtocolVersion = protocolVersion;
    }

    public void ClearPlugin(IPlugin plugin)
    {
        PacketIdPlugins.Remove(plugin);
        PacketTransformationsPlugins.Remove(plugin);
    }

    public void ClearPlugins()
    {
        PacketIdPlugins.Clear();
        PacketTransformationsPlugins.Clear();
    }

    public void ClearPlugins(Direction direction, Operation operation)
    {
        PacketIdPlugins.Clear(direction, operation);
        PacketTransformationsPlugins.Clear(direction, operation);
    }

    public string PrintPackets()
    {
        var packetIdSystemRead = $"{nameof(PacketIdSystem)} READ [{string.Join(", ", PacketIdSystem.Read.PacketTypes.Select(type => type.Name))}]";
        var packetIdSystemWrite = $"{nameof(PacketIdSystem)} WRITE [{string.Join(", ", PacketIdSystem.Write.PacketTypes.Select(type => type.Name))}]";
        var packetIdPluginsRead = $"{nameof(PacketIdPlugins)} READ [{string.Join(", ", PacketIdPlugins.Read.SelectMany(registry => registry.PacketTypes).Select(type => type.Name))}]";
        var packetIdPluginsWrite = $"{nameof(PacketIdPlugins)} WRITE [{string.Join(", ", PacketIdPlugins.Write.SelectMany(registry => registry.PacketTypes).Select(type => type.Name))}]";
        var packetTransformationsSystem = $"{nameof(PacketTransformationsSystem)} [{string.Join(", ", PacketTransformationsSystem.All.PacketTypes.Select(type => type.Name))}]";
        var packetTransformationsPlugins = $"{nameof(PacketTransformationsPlugins)} [{string.Join(", ", PacketTransformationsPlugins.All.SelectMany(registry => registry.PacketTypes).Select(type => type.Name))}]";

        return $"{packetIdSystemRead}\n{packetIdSystemWrite}\n{packetIdPluginsRead}\n{packetIdPluginsWrite}\n{packetTransformationsSystem}\n{packetTransformationsPlugins}";
    }

    public void Dispose()
    {
        PacketIdSystem.Reset();
        PacketIdPlugins.Reset();
        PacketTransformationsSystem.Reset();
        PacketTransformationsPlugins.Reset();

        GC.SuppressFinalize(this);
    }

    public void DisposeBy(IPlugin managedBy)
    {
        if (PacketIdSystem.ManagedBy != managedBy)
            return;

        if (PacketIdPlugins.ManagedBy != managedBy)
            return;

        if (PacketTransformationsSystem.ManagedBy != managedBy)
            return;

        if (PacketTransformationsPlugins.ManagedBy != managedBy)
            return;

        Dispose();
    }
}
