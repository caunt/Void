using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;
using System.Collections;

namespace MinecraftProxy.Network.Protocol.Packets.Shared;

public enum ChatMessageType
{
    Chat,
    System,
    GameInfo
}

public struct ChatMessage : IMinecraftPacket<PlayState>
{
    public string? Message { get; set; }
    public ChatMessageType? Type { get; set; }
    public Guid? Sender { get; set; }
    public bool? SignedPreview { get; set; }
    public bool? Unsigned { get; set; }
    public long? ExpiresAt { get; set; }
    public byte[]? Signature { get; set; }
    public byte[]? Salt { get; set; }
    public KeyValuePair<Guid, byte[]>[]? PreviousMessages { get; set; }
    public KeyValuePair<Guid, byte[]>? LastMessage { get; set; }
    public long? Timestamp { get; set; }
    public LastSeenMessages? LastSeenMessages { get; set; }
    public Direction Direction { get; }


    public ChatMessage() : this(0) { throw new NotSupportedException("Specify direction to chat message"); }

    public ChatMessage(Direction direction) => Direction = direction;

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        ArgumentNullException.ThrowIfNull(Message);
        buffer.WriteString(Message);

        if (Direction == Direction.Clientbound && protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            ArgumentNullException.ThrowIfNull(Type);
            buffer.WriteUnsignedByte((byte)Type);

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
                buffer.WriteGuid(Sender ?? Guid.Empty);
        }


        if (Direction == Direction.Serverbound && protocolVersion > ProtocolVersion.MINECRAFT_1_19 && protocolVersion <= ProtocolVersion.MINECRAFT_1_19_1)
        {
            buffer.WriteLong(Unsigned.HasValue && Unsigned.Value ? DateTimeOffset.Now.ToUnixTimeMilliseconds() : ExpiresAt ?? throw new Exception("Signed message doesn't have expires at value"));
            buffer.WriteLong(Unsigned.HasValue && Unsigned.Value ? 0L : BitConverter.ToInt64(Salt ?? throw new Exception("Signed message doesn't have salt value")));

            var signature = Unsigned.HasValue && Unsigned.Value ? [] : Signature ?? throw new Exception("Signed message doesn't have signature value");
            buffer.WriteVarInt(signature.Length);
            buffer.Write(signature);

            buffer.WriteBoolean(SignedPreview.HasValue && SignedPreview.Value);

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
            {
                var previousMessages = PreviousMessages ?? throw new Exception("Signed message doesn't have previous messages");

                buffer.WriteVarInt(previousMessages.Length);
                foreach (var previousMessage in previousMessages)
                {
                    buffer.WriteGuid(previousMessage.Key);
                    buffer.WriteVarInt(previousMessage.Value.Length);
                    buffer.Write(previousMessage.Value);
                }

                if (LastMessage.HasValue)
                {
                    buffer.WriteBoolean(true);
                    buffer.WriteGuid(LastMessage.Value.Key);
                    buffer.WriteVarInt(LastMessage.Value.Value.Length);
                    buffer.Write(LastMessage.Value.Value);
                }
                else
                {
                    buffer.WriteBoolean(false);
                }
            }
        }

        if (Direction == Direction.Serverbound && protocolVersion > ProtocolVersion.MINECRAFT_1_19_3)
        {
            buffer.WriteLong(Timestamp ?? throw new Exception("Message doesn't have timestamp value"));
            buffer.WriteLong(BitConverter.ToInt64(Salt ?? throw new Exception("Message doesn't have salt value")));
            buffer.WriteBoolean(Unsigned.HasValue && !Unsigned.Value);

            if (Unsigned.HasValue && !Unsigned.Value)
                buffer.Write(Signature ?? throw new Exception("Signed message doesn't have signature value"));

            ArgumentNullException.ThrowIfNull(LastSeenMessages);
            LastSeenMessages.Encode(ref buffer);
        }
    }

    public async Task<bool> HandleAsync(PlayState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        Message = buffer.ReadString(256);

        if (Direction == Direction.Clientbound && protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            Type = (ChatMessageType)buffer.ReadUnsignedByte();

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
                Sender = buffer.ReadGuid();
        }

        if (Direction == Direction.Serverbound && protocolVersion > ProtocolVersion.MINECRAFT_1_19 && protocolVersion <= ProtocolVersion.MINECRAFT_1_19_1)
        {
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

            if (SignedPreview.HasValue && SignedPreview.Value && Unsigned.HasValue && Unsigned.Value)
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

        if (Direction == Direction.Serverbound && protocolVersion > ProtocolVersion.MINECRAFT_1_19_3)
        {
            Timestamp = buffer.ReadLong();
            Salt = BitConverter.GetBytes(buffer.ReadLong());
            Unsigned = !buffer.ReadBoolean();

            if (Unsigned.HasValue && !Unsigned.Value)
                Signature = buffer.Read(256).ToArray();
            else
                Signature = [];

            LastSeenMessages = new LastSeenMessages(ref buffer);
        }
    }
}

public class LastSeenMessages
{
    public static int DivFloor(int dividend, int divisor) => dividend >= 0 ? dividend / divisor : dividend / divisor - (dividend % divisor == 0 ? 0 : 1);
    public static readonly int DIV_FLOOR = -DivFloor(-20, 8);

    public int Offset { get; set; }
    public BitArray Acknowledged { get; set; }
    public bool IsEmpty => !Acknowledged.HasAnySet();

    public LastSeenMessages()
    {
        Offset = 0;
        Acknowledged = new(Array.Empty<byte>());
    }

    public LastSeenMessages(ref MinecraftBuffer buffer)
    {
        Offset = buffer.ReadVarInt();

        var bytes = buffer.Read(DIV_FLOOR);
        Acknowledged = new(bytes.ToArray());
    }

    public void Encode(ref MinecraftBuffer buffer)
    {
        buffer.WriteVarInt(Offset);

        byte[] acknowledged = new byte[DIV_FLOOR];
        Acknowledged.CopyTo(acknowledged, 0);

        buffer.Write(acknowledged);
    }
}