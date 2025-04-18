using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.Common.Network.Registries.Transformations;

public class MinecraftPacketTransformationsSystemRegistry : IMinecraftPacketTransformationsSystemRegistry
{
    public bool IsEmpty => All.IsEmpty;
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IMinecraftPacketTransformationsRegistry All { get; } = new MinecraftPacketTransformationsRegistry();

    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket
    {
        return All.Contains<T>(type);
    }

    public bool Contains(IMinecraftMessage message, TransformationType type)
    {
        return All.Contains(message, type);
    }

    public bool Contains(Type packetType, TransformationType transformationType)
    {
        return All.Contains(packetType, transformationType);
    }

    public void Clear()
    {
        All.Clear();
    }

    public void Reset()
    {
        Clear();
        ProtocolVersion = null;
        ManagedBy = null;
    }
}
