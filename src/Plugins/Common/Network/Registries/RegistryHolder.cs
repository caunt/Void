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
    public ProtocolVersion ProtocolVersion => SystemRegistryHolder.ProtocolVersion ?? ProtocolVersion.Oldest;

    public IMinecraftPacketIdSystemRegistry SystemRegistryHolder { get; } = new MinecraftPacketIdSystemRegistry();
    public IMinecraftPacketIdPluginsRegistry PluginsRegistryHolder { get; } = new MinecraftPacketIdPluginsRegistry();
    public IMinecraftPacketPluginsTransformations TransformationsHolder { get; } = new MinecraftPacketPluginsTransformations();

    public void Setup(IPlugin managedBy, ProtocolVersion protocolVersion)
    {
        if (SystemRegistryHolder.ManagedBy is not null)
            throw new InvalidOperationException($"System registry is already managed by {SystemRegistryHolder.ManagedBy.Name}");

        if (PluginsRegistryHolder.ManagedBy is not null)
            throw new InvalidOperationException($"Plugins registry is already managed by {PluginsRegistryHolder.ManagedBy.Name}");

        if (TransformationsHolder.ManagedBy is not null)
            throw new InvalidOperationException($"Transformations holder is already managed by {TransformationsHolder.ManagedBy.Name}");

        SystemRegistryHolder.ManagedBy = managedBy;
        SystemRegistryHolder.ProtocolVersion = protocolVersion;

        PluginsRegistryHolder.ManagedBy = managedBy;
        PluginsRegistryHolder.ProtocolVersion = protocolVersion;

        TransformationsHolder.ManagedBy = managedBy;
        TransformationsHolder.ProtocolVersion = protocolVersion;
    }

    public void ClearPlugins()
    {
        PluginsRegistryHolder.Clear();
        TransformationsHolder.Clear();
    }

    public string PrintPackets()
    {
        var system = $"{nameof(SystemRegistryHolder)} [{string.Join(", ", SystemRegistryHolder?.Write?.PacketTypes.Select(type => type.Name) ?? [])}]";
        var plugins = $"{nameof(PluginsRegistryHolder)} [{string.Join(", ", PluginsRegistryHolder?.All.SelectMany(registry => registry.PacketTypes).Select(type => type.Name) ?? [])}]";

        return $"{system}\n{plugins}";
    }

    public void Dispose()
    {
        SystemRegistryHolder.Reset();
        PluginsRegistryHolder.Reset();
        TransformationsHolder.Reset();

        GC.SuppressFinalize(this);
    }

    public void DisposeBy(IPlugin managedBy)
    {
        if (SystemRegistryHolder.ManagedBy != managedBy)
            return;

        if (PluginsRegistryHolder.ManagedBy != managedBy)
            return;

        if (TransformationsHolder.ManagedBy != managedBy)
            return;

        Dispose();
    }
}
