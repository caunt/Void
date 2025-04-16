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
    public IMinecraftPacketTransformationsSystemRegistry PacketTransformationsSystem { get; } = new MinecraftPacketTransformationsSystemRegistry();
    public IMinecraftPacketTransformationsPluginsRegistry PacketTransformationsPlugins { get; } = new MinecraftPacketTransformationsPluginsRegistry();

    public void Setup(IPlugin managedBy, ProtocolVersion protocolVersion)
    {
        if (PacketIdSystem.ManagedBy is not null)
            throw new InvalidOperationException($"System packet-id registry is already managed by {PacketIdSystem.ManagedBy.Name} ({PacketIdSystem.ManagedBy})");

        if (PacketIdPlugins.ManagedBy is not null)
            throw new InvalidOperationException($"Plugins packet-id registry is already managed by {PacketIdPlugins.ManagedBy.Name} ({PacketIdSystem.ManagedBy})");

        if (PacketTransformationsSystem.ManagedBy is not null)
            throw new InvalidOperationException($"System packet-transformations registry is already managed by {PacketTransformationsSystem.ManagedBy.Name} ({PacketIdSystem.ManagedBy})");

        if (PacketTransformationsPlugins.ManagedBy is not null)
            throw new InvalidOperationException($"Plugins packet-transformations registry is already managed by {PacketTransformationsPlugins.ManagedBy.Name} ({PacketIdSystem.ManagedBy})");

        PacketIdSystem.ManagedBy = managedBy;
        PacketIdSystem.ProtocolVersion = protocolVersion;

        PacketIdPlugins.ManagedBy = managedBy;
        PacketIdPlugins.ProtocolVersion = protocolVersion;

        PacketTransformationsSystem.ManagedBy = managedBy;
        PacketTransformationsSystem.ProtocolVersion = protocolVersion;

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
        var packetIdSystem = $"{nameof(PacketIdSystem)} [{string.Join(", ", PacketIdSystem.Write.PacketTypes.Select(type => type.Name))}]";
        var packetIdPlugins = $"{nameof(PacketIdPlugins)} [{string.Join(", ", PacketIdPlugins.All.SelectMany(registry => registry.PacketTypes).Select(type => type.Name))}]";
        var packetTransformationsSystem = $"{nameof(PacketTransformationsSystem)} [{string.Join(", ", PacketTransformationsSystem.All.PacketTypes.Select(type => type.Name))}]";
        var packetTransformationsPlugins = $"{nameof(PacketTransformationsPlugins)} [{string.Join(", ", PacketTransformationsPlugins.All.SelectMany(registry => registry.PacketTypes).Select(type => type.Name))}]";

        return $"{packetIdSystem}\n{packetIdPlugins}\n{packetTransformationsSystem}\n{packetTransformationsPlugins}";
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
