using Minecraft.Component.Component;
using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Custom;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public struct DisconnectPacket : IMinecraftPacket<IPlayableState>
{
    public bool EncodeNbt { get; set; }
    public ChatComponent Reason { get; set; }
    public string ReasonString { get; set; } // temporary before Minecraft.Component fixes
    public byte[] ReasonBuffer { get; set; } // temporary before Minecraft.Component fixes

    public DisconnectPacket() : this(true) { }

    public DisconnectPacket(bool nbt) => EncodeNbt = nbt;

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion > ProtocolVersion.MINECRAFT_1_20_2 && EncodeNbt)
        {
            /*// issue SharpNBT to accept Span<byte> instead of only streams
            var stream = new MemoryStream();
            using var writer = new TagWriter(stream, FormatOptions.Java);

            var tag = Reason.ToNbt();
            stream.WriteByte((byte)tag.Type); // issue SharpNBT to write TAG type even if tag is unnamed
            writer.WriteTag(tag);

            buffer.Write(stream.ToArray());*/
            buffer.Write(ReasonBuffer);
            return;
        }

        buffer.WriteString(ReasonString);
    }

    public async Task<bool> HandleAsync(IPlayableState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion > ProtocolVersion.MINECRAFT_1_20_2 && EncodeNbt)
        {
            /*// issue SharpNBT to accept Span<byte> instead of only streams
            var span = buffer.ReadToEnd();
            var stream = new MemoryStream();

            stream.Write(span);
            stream.Position = 0;

            using var reader = new TagReader(stream, FormatOptions.Java);
            var tag = reader.ReadTag(false);

            Reason = ChatComponent.FromNbt(tag);*/
            ReasonBuffer = buffer.ReadToEnd().ToArray();
            return;
        }

        ReasonString = buffer.ReadString(262144);
    }
}