﻿using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

public class LoginSuccessPacket : IMinecraftClientboundPacket<LoginSuccessPacket>
{
    public required GameProfile GameProfile { get; set; }
    public required bool? StrictErrorHandling { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteUuid(GameProfile.Id);
        buffer.WriteString(GameProfile.Username);
        buffer.WritePropertyArray(GameProfile.Properties);

        if (protocolVersion < ProtocolVersion.MINECRAFT_1_20_5 || protocolVersion > ProtocolVersion.MINECRAFT_1_21)
            return;

        if (!StrictErrorHandling.HasValue)
            throw new InvalidDataException(nameof(StrictErrorHandling));

        buffer.WriteBoolean(StrictErrorHandling.Value);
    }

    public static LoginSuccessPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var uuid = buffer.ReadUuid();
        var username = buffer.ReadString();
        var properties = buffer.ReadPropertyArray();
        bool? strictErrorHandling = null;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_5 && protocolVersion <= ProtocolVersion.MINECRAFT_1_21)
            strictErrorHandling = buffer.ReadBoolean();

        return new LoginSuccessPacket
        {
            GameProfile = new GameProfile(username, uuid, properties),
            StrictErrorHandling = strictErrorHandling
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
