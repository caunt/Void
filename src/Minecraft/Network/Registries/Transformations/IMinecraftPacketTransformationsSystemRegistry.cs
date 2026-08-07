using System;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Plugins;

namespace Void.Minecraft.Network.Registries.Transformations;

public interface IMinecraftPacketTransformationsSystemRegistry
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IMinecraftPacketTransformationsRegistry All { get; }

    public bool Contains<T>(TransformationType type) where T : IMinecraftPacket;
    /// <summary>
    /// Determines whether the system registry contains a transformation of the specified type for the runtime type of a message.
    /// </summary>
    /// <param name="message">The message whose runtime type is used to locate a registered transformation.</param>
    /// <param name="type">The direction of transformation to locate.</param>
    /// <returns><see langword="true" /> if a matching transformation is registered; otherwise, <see langword="false" />.</returns>
    public bool Contains(IMinecraftMessage message, TransformationType type);
    public bool Contains(Type packetType, TransformationType transformationType);
    public void Clear();
    public void Reset();
}
