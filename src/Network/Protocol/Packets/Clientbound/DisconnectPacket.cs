using Minecraft.Component.Component;
using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Custom;
using SharpNBT;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public struct DisconnectPacket : IMinecraftPacket<IPlayableState>
{
    public ChatComponent Reason { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion > ProtocolVersion.MINECRAFT_1_20_2)
        {
            // issue SharpNBT to accept Span<byte> instead of only streams
            var stream = new MemoryStream();
            using var writer = new TagWriter(stream, FormatOptions.Java);


            var tag = Reason.ToNbt();
            stream.WriteByte((byte)tag.Type); // SharpNBT not writing TAG type if tag is unnamed
            writer.WriteTag(tag);

            Console.WriteLine("AFTER: " + Convert.ToHexString(stream.ToArray()));

            buffer.Write(stream.ToArray());
            return;
        }
        
        buffer.WriteString(Reason.ToString());
    }

    public async Task<bool> HandleAsync(IPlayableState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion > ProtocolVersion.MINECRAFT_1_20_2)
        {
            // TODO if state is Login, then Reason is still string
            
            // issue SharpNBT to accept Span<byte> instead of only streams
            var span = buffer.ReadToEnd();
            var stream = new MemoryStream();

            stream.Write(span);
            stream.Position = 0;

            Console.WriteLine("BEFORE: " + Convert.ToHexString(stream.ToArray()));

            using var reader = new TagReader(stream, FormatOptions.Java);
            var tag = reader.ReadTag(false);

            Reason = ChatComponent.FromNbt(tag);
            return;
        }

        Reason = new StringComponent(buffer.ReadString(262144));
    }
}