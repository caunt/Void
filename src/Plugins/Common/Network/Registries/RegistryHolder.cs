using Void.Common.Plugins;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Minecraft.Network.Registries.Transformations;
using Void.Proxy.Plugins.Common.Network.Registries.PacketId;
using Void.Proxy.Plugins.Common.Network.Registries.Transformations;

namespace Void.Proxy.Plugins.Common.Network.Registries;

public class RegistryHolder : IRegistryHolder
{
    public ProtocolVersion ProtocolVersion => PacketIdSystem.ProtocolVersion ?? ProtocolVersion.Oldest;

    public IMinecraftPacketIdSystemRegistry PacketIdSystem { get; } = new MinecraftPacketIdSystemRegistry();
    public IMinecraftPacketIdPluginsRegistry PacketIdPlugins { get; } = new MinecraftPacketIdPluginsRegistry();
    public IMinecraftPacketTransformationsPluginsRegistry PacketTransformationsPlugins { get; } = new MinecraftPacketTransformationsPluginsRegistry();

    public void Setup(IPlugin managedBy, ProtocolVersion protocolVersion)
    {
        if (PacketIdSystem.ManagedBy is not null)
            throw new InvalidOperationException($"System packet-id registry is already managed by {PacketIdSystem.ManagedBy.Name}");

        if (PacketIdPlugins.ManagedBy is not null)
            throw new InvalidOperationException($"Plugins packet-id registry is already managed by {PacketIdPlugins.ManagedBy.Name}");

        if (PacketTransformationsPlugins.ManagedBy is not null)
            throw new InvalidOperationException($"Plugins packet-transformations registry is already managed by {PacketTransformationsPlugins.ManagedBy.Name}");

        PacketIdSystem.ManagedBy = managedBy;
        PacketIdSystem.ProtocolVersion = protocolVersion;

        PacketIdPlugins.ManagedBy = managedBy;
        PacketIdPlugins.ProtocolVersion = protocolVersion;

        PacketTransformationsPlugins.ManagedBy = managedBy;
        PacketTransformationsPlugins.ProtocolVersion = protocolVersion;
    }

    public void ClearPlugins()
    {
        PacketIdPlugins.Clear();
        PacketTransformationsPlugins.Clear();
    }

    public string PrintPackets()
    {
        var system = $"{nameof(PacketIdSystem)} [{string.Join(", ", PacketIdSystem?.Write?.PacketTypes.Select(type => type.Name) ?? [])}]";
        var plugins = $"{nameof(PacketIdPlugins)} [{string.Join(", ", PacketIdPlugins?.All.SelectMany(registry => registry.PacketTypes).Select(type => type.Name) ?? [])}]";

        return $"{system}\n{plugins}";
    }

    public void Dispose()
    {
        PacketIdSystem.Reset();
        PacketIdPlugins.Reset();
        PacketTransformationsPlugins.Reset();

        GC.SuppressFinalize(this);
    }

    public void DisposeBy(IPlugin managedBy)
    {
        if (PacketIdSystem.ManagedBy != managedBy)
            return;

        if (PacketIdPlugins.ManagedBy != managedBy)
            return;

        if (PacketTransformationsPlugins.ManagedBy != managedBy)
            return;

        Dispose();
    }
}
