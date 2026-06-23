using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.Registry;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Definitions;
using Void.Minecraft.Profiles;
using Void.Minecraft.World;
using Xunit;
using LatestJoinGamePacket = Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound.JoinGamePacket;
using LatestLoginSuccessPacket = Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound.LoginSuccessPacket;

namespace Void.UnitTests.Protocol;

public class Minecraft26_2Tests
{
    [Fact]
    public void ProtocolVersion_Registers26_2AsLatest()
    {
        Assert.Equal(776, ProtocolVersion.MINECRAFT_26_2.Value);
        Assert.Equal("26.2", ProtocolVersion.MINECRAFT_26_2.ToString());
        Assert.Same(ProtocolVersion.MINECRAFT_26_2, ProtocolVersion.Get(776));
        Assert.Same(ProtocolVersion.MINECRAFT_26_2, ProtocolVersion.Latest);
    }

    [Fact]
    public void LoginSuccess_26_2_RoundTripsSessionId()
    {
        var sessionId = Uuid.Parse("12345678-1234-5678-9abc-123456789abc");
        var packet = new LatestLoginSuccessPacket
        {
            GameProfile = new GameProfile("Player", Uuid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), []),
            StrictErrorHandling = null,
            SessionId = sessionId
        };

        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        packet.Encode(ref buffer, ProtocolVersion.MINECRAFT_26_2);

        stream.Position = 0;
        var readBuffer = new MinecraftBuffer(stream);
        var result = LatestLoginSuccessPacket.Decode(ref readBuffer, ProtocolVersion.MINECRAFT_26_2);

        Assert.Equal(packet.GameProfile.Username, result.GameProfile.Username);
        Assert.Equal(packet.GameProfile.Id, result.GameProfile.Id);
        Assert.Empty(result.GameProfile.Properties ?? []);
        Assert.Null(result.StrictErrorHandling);
        Assert.Equal(sessionId, result.SessionId);
    }

    [Fact]
    public void LoginSuccess_26_1_DoesNotWriteSessionId()
    {
        var packet = new LatestLoginSuccessPacket
        {
            GameProfile = new GameProfile("Player", Uuid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), []),
            StrictErrorHandling = null,
            SessionId = Uuid.Parse("12345678-1234-5678-9abc-123456789abc")
        };

        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        packet.Encode(ref buffer, ProtocolVersion.MINECRAFT_26_1);

        stream.Position = 0;
        var readBuffer = new MinecraftBuffer(stream);
        var result = LatestLoginSuccessPacket.Decode(ref readBuffer, ProtocolVersion.MINECRAFT_26_1);

        Assert.Null(result.SessionId);
    }

    [Fact]
    public void ArgumentParserDefinitions_RenameColorToTeamColorIn26_2()
    {
        var color = ArgumentParserDefinitions.MinecraftArgumentParserDefinitions.Single(definition => definition.Mapping.Identifier == "minecraft:color").Mapping;
        var teamColor = ArgumentParserDefinitions.MinecraftArgumentParserDefinitions.Single(definition => definition.Mapping.Identifier == "minecraft:team_color").Mapping;

        Assert.True(color.TryGetParserId(ProtocolVersion.MINECRAFT_26_1, out var colorId));
        Assert.Equal(16, colorId);
        Assert.False(color.TryGetParserId(ProtocolVersion.MINECRAFT_26_2, out _));
        Assert.True(teamColor.TryGetParserId(ProtocolVersion.MINECRAFT_26_2, out var teamColorId));
        Assert.Equal(16, teamColorId);
    }

    [Fact]
    public void ArgumentSerializerRegistry_DecodesParserId16AsTeamColorIn26_2()
    {
        Span<byte> data = stackalloc byte[5];
        var buffer = new BufferSpan(data);
        buffer.WriteVarInt(16);
        buffer.Position = 0;

        var mapping = ArgumentSerializerRegistry.DecodeParserMapping(ref buffer, ProtocolVersion.MINECRAFT_26_2);

        Assert.Equal("minecraft:team_color", mapping.Identifier);
    }

    [Fact]
    public void JoinGame_26_2_RoundTripsOnlineModeBeforeSecureChat()
    {
        var packet = new LatestJoinGamePacket
        {
            EntityId = 42,
            IsHardcore = true,
            LevelNames = ["minecraft:overworld", "minecraft:the_nether"],
            MaxPlayers = 20,
            ViewDistance = 12,
            SimulationDistance = 8,
            ReducedDebugInfo = true,
            ShowRespawnScreen = false,
            DoLimitedCrafting = true,
            Dimension = 1,
            DimensionInfo = new DimensionInfo("minecraft:overworld", "minecraft:overworld", IsFlat: false, IsDebugType: true),
            PartialHashedSeed = 123456789L,
            Gamemode = 1,
            PreviousGamemode = -1,
            LastDeathPosition = new KeyValuePair<string, long>("minecraft:overworld", 12345L),
            PortalCooldown = 80,
            SeaLevel = 63,
            OnlineMode = true,
            EnforcesSecureChat = false
        };

        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        packet.Encode(ref buffer, ProtocolVersion.MINECRAFT_26_2);

        stream.Position = 0;
        var readBuffer = new MinecraftBuffer(stream);
        var result = LatestJoinGamePacket.Decode(ref readBuffer, ProtocolVersion.MINECRAFT_26_2);

        Assert.Equal(packet.EntityId, result.EntityId);
        Assert.Equal(packet.LevelNames, result.LevelNames);
        Assert.Equal(packet.Dimension, result.Dimension);
        Assert.Equal(packet.DimensionInfo.LevelName, result.DimensionInfo?.LevelName);
        Assert.Equal(packet.PreviousGamemode, result.PreviousGamemode);
        Assert.Equal(packet.LastDeathPosition, result.LastDeathPosition);
        Assert.Equal(packet.SeaLevel, result.SeaLevel);
        Assert.True(result.OnlineMode);
        Assert.False(result.EnforcesSecureChat);
        Assert.False(readBuffer.HasData);
    }

    [Fact]
    public void PacketIdDefinitions_MapLatestJoinGamePacketIds()
    {
        Assert.Contains(PacketIdDefinitions.ClientboundJoinGame, mapping => mapping.ProtocolVersion == ProtocolVersion.MINECRAFT_1_20_2 && mapping.Id == 0x29);
        Assert.Contains(PacketIdDefinitions.ClientboundJoinGame, mapping => mapping.ProtocolVersion == ProtocolVersion.MINECRAFT_1_20_5 && mapping.Id == 0x2B);
        Assert.Contains(PacketIdDefinitions.ClientboundJoinGame, mapping => mapping.ProtocolVersion == ProtocolVersion.MINECRAFT_1_21_2 && mapping.Id == 0x2C);
        Assert.Contains(PacketIdDefinitions.ClientboundJoinGame, mapping => mapping.ProtocolVersion == ProtocolVersion.MINECRAFT_1_21_5 && mapping.Id == 0x2B);
        Assert.Contains(PacketIdDefinitions.ClientboundJoinGame, mapping => mapping.ProtocolVersion == ProtocolVersion.MINECRAFT_1_21_9 && mapping.Id == 0x30);
        Assert.Contains(PacketIdDefinitions.ClientboundJoinGame, mapping => mapping.ProtocolVersion == ProtocolVersion.MINECRAFT_26_1 && mapping.Id == 0x31);
    }
}
