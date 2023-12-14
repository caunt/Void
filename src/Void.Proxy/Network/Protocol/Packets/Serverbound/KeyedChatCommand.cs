using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.Packets.Shared;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Network.Protocol.Packets.Serverbound;

public struct KeyedChatCommand : IMinecraftPacket<PlayState>, IChatCommand
{
    public bool Signed { get; set; }
    public string Command { get; set; }
    public long Timestamp { get; set; }
    public long Salt { get; set; }
    public bool SignedPreview { get; set; }
    public KeyValuePair<Guid, byte[]>[] PreviousMessages { get; set; }
    public KeyValuePair<Guid, byte[]>? LastMessage { get; set; }
    public Dictionary<string, byte[]> Arguments { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Command);
        buffer.WriteLong(Timestamp);
        buffer.WriteLong(Signed ? Salt : 0);

        if (Arguments.Count > 8)
            throw new Exception($"Too many argument signatures, {Arguments.Count} is above limit 8");

        buffer.WriteVarInt(Arguments.Count);
        foreach (var (argument, key) in Arguments)
        {
            buffer.WriteString(argument);
            buffer.WriteVarInt(Signed ? key.Length : 0);

            if (Signed)
                buffer.Write(key);
        }

        buffer.WriteBoolean(SignedPreview);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
        {
            buffer.WriteVarInt(PreviousMessages.Length);

            foreach (var (signer, signature) in PreviousMessages)
            {
                buffer.WriteGuid(signer);
                buffer.WriteVarInt(signature.Length);
                buffer.Write(signature);
            }

            if (LastMessage.HasValue)
            {
                var (signer, signature) = LastMessage.Value;

                buffer.WriteBoolean(true);
                buffer.WriteGuid(signer);

                buffer.WriteVarInt(signature.Length);
                buffer.Write(signature);
            }
            else
            {
                buffer.WriteBoolean(false);
            }
        }
    }

    public async Task<bool> HandleAsync(PlayState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        Command = buffer.ReadString(256);
        Timestamp = buffer.ReadLong();
        Salt = buffer.ReadLong();

        int size = buffer.ReadVarInt();

        if (size > 8)
            throw new Exception($"Too many argument signatures, {size} is above limit 8");

        Arguments = new Dictionary<string, byte[]>(size);

        for (int i = 0; i < size; i++)
            Arguments.Add(buffer.ReadString(16), buffer.Read(Signed ? 65536 : 0).ToArray());

        SignedPreview = buffer.ReadBoolean();

        if (!Signed && SignedPreview)
            throw new Exception("Unsigned chat command requested signed preview");

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
        {
            size = buffer.ReadVarInt();

            if (size < 0 || size > 5)
                throw new Exception($"Bad argument signatures count, {size} should be between 0 and 5");

            PreviousMessages = new KeyValuePair<Guid, byte[]>[size];

            for (int i = 0; i < size; i++)
                PreviousMessages[i] = new(buffer.ReadGuid(), buffer.Read(buffer.ReadVarInt()).ToArray());

            if (buffer.ReadBoolean())
                LastMessage = new(buffer.ReadGuid(), buffer.Read(buffer.ReadVarInt()).ToArray());
        }

        if (Salt != 0L || PreviousMessages.Length != 0)
            Signed = true;
    }
}