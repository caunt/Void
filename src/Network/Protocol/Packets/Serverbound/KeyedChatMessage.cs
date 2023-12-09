using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.Packets.Shared;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public struct KeyedChatMessage : IMinecraftPacket<PlayState>, IChatMessage
{
    public string Message { get; set; }
    public bool SignedPreview { get; set; }
    public bool Unsigned { get; set; }
    public long ExpiresAt { get; set; }
    public byte[] Signature { get; set; }
    public byte[] Salt { get; set; }
    public KeyValuePair<Guid, byte[]>[] PreviousMessages { get; set; }
    public KeyValuePair<Guid, byte[]>? LastMessage { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Message);

        buffer.WriteLong(Unsigned ? DateTimeOffset.Now.ToUnixTimeMilliseconds() : ExpiresAt);
        buffer.WriteLong(Unsigned ? 0L : BitConverter.ToInt64(Salt ?? throw new Exception("Signed message doesn't have salt value")));

        var signature = Unsigned ? [] : Signature;
        buffer.WriteVarInt(signature.Length);
        buffer.Write(signature);

        buffer.WriteBoolean(SignedPreview);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
        {
            buffer.WriteVarInt(PreviousMessages.Length);
            foreach (var previousMessage in PreviousMessages)
            {
                buffer.WriteGuid(previousMessage.Key);
                buffer.WriteVarInt(previousMessage.Value.Length);
                buffer.Write(previousMessage.Value);
            }

            if (LastMessage.HasValue)
            {
                var (guid, key) = LastMessage.Value;

                buffer.WriteBoolean(true);
                buffer.WriteGuid(guid);
                buffer.WriteVarInt(key.Length);
                buffer.Write(key);
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
        Message = buffer.ReadString(256);

        var expiresAt = buffer.ReadLong();
        var saltLong = buffer.ReadLong();
        var signatureBytes = buffer.Read(buffer.ReadVarInt());

        if (saltLong != 0L && signatureBytes.Length > 0)
        {
            Salt = BitConverter.GetBytes(saltLong);
            Signature = signatureBytes.ToArray();
            ExpiresAt = expiresAt;
        }
        else if ((protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1 || saltLong == 0L) && signatureBytes.Length == 0)
        {
            Unsigned = true;
        }
        else
        {
            throw new Exception($"Invalid chat message signature");
        }

        SignedPreview = buffer.ReadBoolean();

        if (SignedPreview && Unsigned)
            throw new Exception($"Unsigned chat message requested signed preview");

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
        {
            var size = buffer.ReadVarInt();

            if (size < 0 || size > 5)
                throw new Exception("Invalid previous messages");

            var lastSignatures = new KeyValuePair<Guid, byte[]>[size];

            for (var i = 0; i < size; i++)
                lastSignatures[i] = new(buffer.ReadGuid(), buffer.Read(buffer.ReadVarInt()).ToArray());

            PreviousMessages = lastSignatures;

            if (buffer.ReadBoolean())
                LastMessage = new(buffer.ReadGuid(), buffer.Read(buffer.ReadVarInt()).ToArray());
        }
    }
}