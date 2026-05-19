using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Proxy.Plugins.Common.Network.Packets.Clientbound;

public class CommandSuggestionsResponsePacket : IMinecraftServerboundPacket<CommandSuggestionsResponsePacket>
{
    public int? TransactionId { get; set; }
    public int? Start { get; set; }
    public int? Length { get; set; }
    public Dictionary<string, Component?> Offers { get; set; } = [];

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_13)
        {
            if (!TransactionId.HasValue)
                throw new InvalidOperationException("TransactionId must be set for protocol versions 1.13 and above.");
            
            if (!Start.HasValue)
                throw new InvalidOperationException("Start must be set for protocol versions 1.13 and above.");
            
            if (!Length.HasValue)
                throw new InvalidOperationException("Length must be set for protocol versions 1.13 and above.");
            
            buffer.WriteVarInt(TransactionId.Value);
            buffer.WriteVarInt(Start.Value);
            buffer.WriteVarInt(Length.Value);
            buffer.WriteVarInt(Offers.Count);
            
            foreach (var offer in Offers)
            {
                buffer.WriteString(offer.Key);
                buffer.WriteBoolean(offer.Value is not null);
                
                if (offer.Value is not null)
                    buffer.WriteComponent(offer.Value, asNbt: protocolVersion >= ProtocolVersion.MINECRAFT_1_20_3);
            }
        }
        else
        {
            buffer.WriteVarInt(Offers.Count);
            
            foreach (var offer in Offers)
                buffer.WriteString(offer.Key);
        }
    }

    public static CommandSuggestionsResponsePacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        var offers = new Dictionary<string, Component?>();
        int? transactionId = null;
        int? start = null;
        int? length = null;
        
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_13)
        {
            transactionId = buffer.ReadVarInt();
            start = buffer.ReadVarInt();
            length = buffer.ReadVarInt();
            
            var offersAvailable = buffer.ReadVarInt();
            for (int i = 0; i < offersAvailable; i++) 
            {
                var offer = buffer.ReadString();
                var tooltip = buffer.ReadBoolean() ? buffer.ReadComponent(asNbt: protocolVersion >= ProtocolVersion.MINECRAFT_1_20_3) : null;
                
                offers[offer] = tooltip;
            }
        } 
        else 
        {
            var offersAvailable = buffer.ReadVarInt();
            
            for (int i = 0; i < offersAvailable; i++)
                offers[buffer.ReadString()] = null;
        }
        
        return new CommandSuggestionsResponsePacket
        {
            TransactionId = transactionId,
            Start = start,
            Length = length,
            Offers = offers
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
