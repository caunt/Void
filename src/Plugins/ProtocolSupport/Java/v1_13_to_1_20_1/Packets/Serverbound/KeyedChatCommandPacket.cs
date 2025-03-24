using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Mojang;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Commands;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

public class KeyedChatCommandPacket : IMinecraftServerboundPacket<KeyedChatCommandPacket>, IChatCommand
{
    public required bool IsSigned { get; set; }
    public required string Command { get; set; }
    public required long Timestamp { get; set; }
    public required long Salt { get; set; }
    public required bool SignedPreview { get; set; }
    public required KeyValuePair<Uuid, byte[]>[] PreviousMessages { get; set; }
    public required KeyValuePair<Uuid, byte[]> LastMessage { get; set; }
    public required Dictionary<string, byte[]> Arguments { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Command);
        buffer.WriteLong(Timestamp);
        buffer.WriteLong(IsSigned ? Salt : 0);

        if (Arguments.Count > 8)
            throw new Exception($"Too many argument signatures, {Arguments.Count} is above limit 8");

        buffer.WriteVarInt(Arguments.Count);
        foreach (var (argument, key) in Arguments)
        {
            buffer.WriteString(argument);
            buffer.WriteVarInt(IsSigned ? key.Length : 0);

            if (IsSigned)
                buffer.Write(key);
        }

        buffer.WriteBoolean(SignedPreview);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
        {
            buffer.WriteVarInt(PreviousMessages.Length);

            foreach (var (signer, signature) in PreviousMessages)
            {
                buffer.WriteUuid(signer);
                buffer.WriteVarInt(signature.Length);
                buffer.Write(signature);
            }

            if (!LastMessage.IsDefault())
            {
                var (signer, signature) = LastMessage;

                buffer.WriteBoolean(true);
                buffer.WriteUuid(signer);

                buffer.WriteVarInt(signature.Length);
                buffer.Write(signature);
            }
            else
            {
                buffer.WriteBoolean(false);
            }
        }
    }

    public static KeyedChatCommandPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var command = buffer.ReadString(256);
        var timestamp = buffer.ReadLong();
        var salt = buffer.ReadLong();

        var size = buffer.ReadVarInt();

        if (size > 8)
            throw new Exception($"Too many argument signatures, {size} is above limit 8");

        var arguments = new Dictionary<string, byte[]>(size);

        for (var i = 0; i < size; i++)
            arguments.Add(buffer.ReadString(16), buffer.Read(buffer.ReadVarInt()).ToArray());

        var signedPreview = buffer.ReadBoolean();

        var previousMessages = new KeyValuePair<Uuid, byte[]>[size];
        var lastMessage = default(KeyValuePair<Uuid, byte[]>);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
        {
            size = buffer.ReadVarInt();

            if (size < 0 || size > 5)
                throw new Exception($"Bad argument signatures count, {size} should be between 0 and 5");


            for (var i = 0; i < size; i++)
                previousMessages[i] = new KeyValuePair<Uuid, byte[]>(buffer.ReadUuid(), buffer.Read(buffer.ReadVarInt()).ToArray());

            if (buffer.ReadBoolean())
                lastMessage = new KeyValuePair<Uuid, byte[]>(buffer.ReadUuid(), buffer.Read(buffer.ReadVarInt()).ToArray());
        }

        var isSigned = salt != 0L || previousMessages.Length != 0;

        return new KeyedChatCommandPacket
        {
            IsSigned = isSigned,
            Command = command,
            Timestamp = timestamp,
            Salt = salt,
            Arguments = arguments,
            SignedPreview = signedPreview,
            PreviousMessages = previousMessages,
            LastMessage = lastMessage
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}