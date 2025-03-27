using System.Collections;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Commands;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

public class SignedChatCommandPacket : IMinecraftServerboundPacket<SignedChatCommandPacket>, IChatCommand
{
    public const int WindowSize = 20;
    public const int DivFloor = (WindowSize + 8 - 1) / 8;

    public bool IsSigned => true;
    public required string Command { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
    public required long Salt { get; set; }
    public required Dictionary<string, byte[]> ArgumentSignatures { get; set; }
    public required int MessageCount { get; set; }
    public required BitArray Acknowledged { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Command);
        buffer.WriteLong(Timestamp.ToUnixTimeMilliseconds());
        buffer.WriteLong(Salt);

        buffer.WriteVarInt(ArgumentSignatures.Count);
        foreach (var (name, value) in ArgumentSignatures)
        {
            buffer.WriteString(name);
            buffer.Write(value);
        }

        buffer.WriteVarInt(MessageCount);

        var array = new byte[DivFloor];
        Acknowledged.CopyTo(array, 0);
        buffer.Write(array);
    }

    public static SignedChatCommandPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new SignedChatCommandPacket
        {
            Command = buffer.ReadString(),
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(buffer.ReadLong()),
            Salt = buffer.ReadLong(),
            ArgumentSignatures = DecodeArgumentSignatureArray(ref buffer),
            MessageCount = buffer.ReadVarInt(),
            Acknowledged = DecodeAcknowledged(ref buffer)
        };

        static Dictionary<string, byte[]> DecodeArgumentSignatureArray(ref MinecraftBuffer buffer)
        {
            var count = buffer.ReadVarInt();
            var array = new Dictionary<string, byte[]>(count);

            for (var i = 0; i < count; i++)
                array.Add(buffer.ReadString(), buffer.Read(256).ToArray());

            return array;
        }

        static BitArray DecodeAcknowledged(ref MinecraftBuffer buffer)
        {
            return new BitArray(buffer.Read(DivFloor).ToArray());
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}