using System.Collections;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

public class SignedChatCommandPacket : IMinecraftServerboundPacket<SignedChatCommandPacket>
{
    public const int WindowSize = 20;
    public const int DivFloor = (WindowSize + 8 - 1) / 8;

    public required string Command { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
    public required long Salt { get; set; }
    public required Dictionary<string, byte[]> ArgumentSignatures { get; set; }
    public required int MessageCount { get; set; }
    public required BitArray Acknowledged { get; set; }
    public byte Checksum { get; set; }

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

        Span<byte> array = stackalloc byte[DivFloor];
        array.Clear();

        for (var i = 0; i < Acknowledged.Length; i++)
        {
            if (Acknowledged[i])
                array[i / 8] |= (byte)(1 << (i % 8));
        }

        for (var i = 0; i < array.Length; i++)
            buffer.WriteUnsignedByte(array[i]);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_21_5)
            buffer.WriteUnsignedByte(0); // always 0, no Checksum
    }

    public static SignedChatCommandPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var command = buffer.ReadString();
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(buffer.ReadLong());
        var salt = buffer.ReadLong();
        var argumentSignatures = DecodeArgumentSignatureArray(ref buffer);
        var messageCount = buffer.ReadVarInt();
        var acknowledged = DecodeAcknowledged(ref buffer);
        var checksum = byte.MinValue;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_21_5)
            checksum = buffer.ReadUnsignedByte();


        return new SignedChatCommandPacket
        {
            Command = command,
            Timestamp = timestamp,
            Salt = salt,
            ArgumentSignatures = argumentSignatures,
            MessageCount = messageCount,
            Acknowledged = acknowledged,
            Checksum = checksum,
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
            Span<byte> data = stackalloc byte[DivFloor];
            buffer.Read(DivFloor).CopyTo(data);
            var result = new BitArray(WindowSize);

            for (var i = 0; i < WindowSize; i++)
                result[i] = (data[i / 8] & (1 << (i % 8))) != 0;

            return result;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
