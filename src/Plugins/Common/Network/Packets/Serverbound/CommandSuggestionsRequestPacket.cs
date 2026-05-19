using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.Common.Network.Packets.Serverbound;

public class CommandSuggestionsRequestPacket : IMinecraftServerboundPacket<CommandSuggestionsRequestPacket>
{
    public required string Command { get; set; }
    public int? TransactionId { get; set; }
    public bool? AssumeCommand { get; set; }
    public long? Position { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_13)
        {
            if (!TransactionId.HasValue)
                throw new InvalidOperationException("TransactionId must be set for protocol versions 1.13 and above.");
            
            buffer.WriteVarInt(TransactionId.Value);
            buffer.WriteString(Command);
        }
        else
        {
            buffer.WriteString(Command);
            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_9)
            {
                if (!AssumeCommand.HasValue)
                    throw new InvalidOperationException("AssumeCommand must be set for protocol versions 1.9 and above.");
                
                buffer.WriteBoolean(AssumeCommand.Value);
            }
            
            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
            {
                buffer.WriteBoolean(Position.HasValue);
            
                if (Position.HasValue)
                    buffer.WriteLong(Position.Value);
            }
        }
    }

    public static CommandSuggestionsRequestPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        string command;
        int? transactionId = null;
        bool? assumeCommand = null;
        long? position = null;
        
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_13)
        {
            transactionId = buffer.ReadVarInt();
            command = buffer.ReadString();
        } 
        else 
        {
            command = buffer.ReadString();
            
            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_9)
                assumeCommand = buffer.ReadBoolean();
            
            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8 && buffer.ReadBoolean()) 
                position = buffer.ReadLong();
        }
        
        return new CommandSuggestionsRequestPacket
        {
            Command = command, 
            TransactionId = transactionId,
            AssumeCommand = assumeCommand, 
            Position = position
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
