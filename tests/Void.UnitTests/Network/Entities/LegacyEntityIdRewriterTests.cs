using System.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network;
using Void.Proxy.Plugins.Common.Network.Entities;
using Void.Proxy.Plugins.Common.Network.Messages.Binary;
using Xunit;

namespace Void.UnitTests.Network.Entities;

public class LegacyEntityIdRewriterTests
{
    [Fact]
    public void Rewrite_ClientboundVarInt_RewritesServerIdAndPreservesPayload()
    {
        using var packet = CreatePacket(0x3F, (ref MinecraftBuffer buffer) =>
        {
            buffer.WriteVarInt(300);
            buffer.WriteUnsignedByte(0xFF);
        });

        LegacyEntityIdRewriter.Rewrite(packet, ProtocolVersion.MINECRAFT_1_13, Direction.Clientbound, serverEntityId: 300, clientEntityId: 5);

        var buffer = new MinecraftBuffer(packet.Stream);
        Assert.Equal(5, buffer.ReadVarInt());
        Assert.Equal(0xFF, buffer.ReadUnsignedByte());
        Assert.False(buffer.HasData);
    }

    [Fact]
    public void Rewrite_ClientboundVarInt_SwapsCollidingClientId()
    {
        using var packet = CreatePacket(0x3F, (ref MinecraftBuffer buffer) =>
        {
            buffer.WriteVarInt(5);
            buffer.WriteUnsignedByte(0xFF);
        });

        LegacyEntityIdRewriter.Rewrite(packet, ProtocolVersion.MINECRAFT_1_13, Direction.Clientbound, serverEntityId: 300, clientEntityId: 5);

        var buffer = new MinecraftBuffer(packet.Stream);
        Assert.Equal(300, buffer.ReadVarInt());
        Assert.Equal(0xFF, buffer.ReadUnsignedByte());
    }

    [Fact]
    public void Rewrite_DestroyEntities_SwapsEveryEntityId()
    {
        using var packet = CreatePacket(0x35, (ref MinecraftBuffer buffer) =>
        {
            buffer.WriteVarInt(3);
            buffer.WriteVarInt(300);
            buffer.WriteVarInt(5);
            buffer.WriteVarInt(42);
        });

        LegacyEntityIdRewriter.Rewrite(packet, ProtocolVersion.MINECRAFT_1_13, Direction.Clientbound, serverEntityId: 300, clientEntityId: 5);

        var buffer = new MinecraftBuffer(packet.Stream);
        Assert.Equal(3, buffer.ReadVarInt());
        Assert.Equal(5, buffer.ReadVarInt());
        Assert.Equal(300, buffer.ReadVarInt());
        Assert.Equal(42, buffer.ReadVarInt());
    }

    [Fact]
    public void Rewrite_CollectItem_SwapsCollectorAndCollectedEntityIds()
    {
        using var packet = CreatePacket(0x4F, (ref MinecraftBuffer buffer) =>
        {
            buffer.WriteVarInt(300);
            buffer.WriteVarInt(5);
            buffer.WriteVarInt(1);
        });

        LegacyEntityIdRewriter.Rewrite(packet, ProtocolVersion.MINECRAFT_1_13, Direction.Clientbound, serverEntityId: 300, clientEntityId: 5);

        var buffer = new MinecraftBuffer(packet.Stream);
        Assert.Equal(5, buffer.ReadVarInt());
        Assert.Equal(300, buffer.ReadVarInt());
        Assert.Equal(1, buffer.ReadVarInt());
    }

    [Fact]
    public void Rewrite_ClientboundInt_SwapsEntityId()
    {
        using var packet = CreatePacket(0x1C, (ref MinecraftBuffer buffer) =>
        {
            buffer.WriteInt(300);
            buffer.WriteUnsignedByte(2);
        });

        LegacyEntityIdRewriter.Rewrite(packet, ProtocolVersion.MINECRAFT_1_13, Direction.Clientbound, serverEntityId: 300, clientEntityId: 5);

        var buffer = new MinecraftBuffer(packet.Stream);
        Assert.Equal(5, buffer.ReadInt());
        Assert.Equal(2, buffer.ReadUnsignedByte());
    }

    [Fact]
    public void Rewrite_ServerboundVarInt_MapsClientIdToServerId()
    {
        using var packet = CreatePacket(0x0D, (ref MinecraftBuffer buffer) =>
        {
            buffer.WriteVarInt(5);
            buffer.WriteVarInt(1);
        });

        LegacyEntityIdRewriter.Rewrite(packet, ProtocolVersion.MINECRAFT_1_13, Direction.Serverbound, serverEntityId: 300, clientEntityId: 5);

        var buffer = new MinecraftBuffer(packet.Stream);
        Assert.Equal(300, buffer.ReadVarInt());
        Assert.Equal(1, buffer.ReadVarInt());
    }

    [Fact]
    public void Rewrite_UnrelatedPacket_RemainsUnchanged()
    {
        using var packet = CreatePacket(0x21, (ref MinecraftBuffer buffer) => buffer.WriteVarInt(300));
        var expected = packet.Stream.ToArray();

        LegacyEntityIdRewriter.Rewrite(packet, ProtocolVersion.MINECRAFT_1_13, Direction.Clientbound, serverEntityId: 300, clientEntityId: 5);

        Assert.Equal(expected, packet.Stream.ToArray());
    }

    private static MinecraftBinaryPacket CreatePacket(int packetId, WritePayload writePayload)
    {
        var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteVarInt(packetId);
        var payloadPosition = stream.Position;
        writePayload(ref buffer);
        stream.Position = payloadPosition;
        return new MinecraftBinaryPacket(packetId, stream);
    }

    private delegate void WritePayload(ref MinecraftBuffer buffer);
}
