using System;
using System.Collections.Generic;
using System.Linq;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Definitions;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Streams.Recyclable;
using Void.Proxy.Plugins.Common.Network.Channels;
using Void.Proxy.Plugins.Common.Network.Packets.Serverbound;
using Void.Proxy.Plugins.Common.Network.Packets.Transformations;
using Void.Proxy.Plugins.Common.Network.Streams.Packet;
using Xunit;

namespace Void.UnitTests.Network.Packets.Transformations;

public class NetworkTransformationsTests
{
    [Fact]
    public void RegisterMappings_AfterServerRedirect_PreservesLegacyKeepAliveDecoding()
    {
        const int keepAliveId = 1687097462;

        var protocolVersion = ProtocolVersion.MINECRAFT_1_8;
        var packetStream = new MinecraftPacketMessageStream();
        using var channel = new SimpleMinecraftChannel(packetStream);
        var mappings = NetworkTransformations.KeepAlive.SelectMany(transformation => transformation.Mappings);

        packetStream.Registries.PacketIdSystem.ProtocolVersion = protocolVersion;
        packetStream.Registries.PacketIdSystem.ReplacePackets(Operation.Read, new Dictionary<MinecraftPacketIdMapping[], Type>
        {
            { PacketIdDefinitions.ServerboundPlayKeepAliveResponse, typeof(KeepAliveResponsePacket) }
        });

        NetworkTransformations.RegisterMappings<KeepAliveResponsePacket>(channel, protocolVersion, mappings);
        Assert.True(packetStream.Registries.PacketTransformationsSystem.All.TryGetFor(typeof(KeepAliveResponsePacket), TransformationType.Upgrade, out var originalTransformations));

        NetworkTransformations.RegisterMappings<KeepAliveResponsePacket>(channel, protocolVersion, mappings);
        Assert.True(packetStream.Registries.PacketTransformationsSystem.All.TryGetFor(typeof(KeepAliveResponsePacket), TransformationType.Upgrade, out var retainedTransformations));
        Assert.Same(originalTransformations, retainedTransformations);

        var stream = RecyclableStream.RecyclableMemoryStreamManager.GetStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteVarInt(PacketIdDefinitions.ServerboundPlayKeepAliveResponse[0].Id);
        buffer.WriteVarInt(keepAliveId);

        var packet = Assert.IsType<KeepAliveResponsePacket>(packetStream.DecodePacket(stream));

        Assert.Equal(keepAliveId, packet.Id);
    }
}
