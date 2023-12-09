﻿using Minecraft.Component.Component;
using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Custom;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public struct DisconnectPacket : IMinecraftPacket<ILoginConfigurePlayState>
{
    public bool EncodeNbt { get; set; }
    public ChatComponent Reason { get; set; }
    public string ReasonString { get; set; } // temporary before Minecraft.Component fixes
    public byte[] ReasonBuffer { get; set; } // temporary before Minecraft.Component fixes

    public DisconnectPacket() : this(true) { }

    public DisconnectPacket(bool nbt) => EncodeNbt = nbt;

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (EncodeNbt)
        {
            // buffer.WriteComponent(Message, protocolVersion);
            buffer.Write(ReasonBuffer);
        }
        else
        {
            buffer.WriteString(ReasonString);
        }
    }

    public async Task<bool> HandleAsync(ILoginConfigurePlayState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (EncodeNbt)
        {
            // Message = buffer.ReadComponent(262144, protocolVersion);
            ReasonBuffer = buffer.ReadToEnd().ToArray();
        }
        else
        {
            ReasonString = buffer.ReadString(262144);
        }
    }
}