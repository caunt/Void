using Microsoft.Extensions.Logging;
using Void.Common.Events;
using Void.Minecraft.Network.Messages.Binary;
using Void.Minecraft.Network.Messages.Packets;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Network.Messages;

namespace Void.Proxy.Plugins.Essentials.Debugging;

public class TraceService(ILogger<TraceService> logger) : IEventListener
{
    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        switch (@event.Message)
        {
            case IBufferedBinaryMessage bufferedBinaryMessage:
                logger.LogTrace("Received buffer length {Length} from {Side} {PlayerOrServer}", bufferedBinaryMessage.Stream.Length, @event.From, @event.From.FromLink(@event.Link));
                return;
            case IMinecraftBinaryMessage binaryMessage:
                logger.LogTrace("Received packet id 0x{PacketId:X2}, length {Length} from {Side} {PlayerOrServer}", binaryMessage.Id, binaryMessage.Stream.Length, @event.From, @event.From.FromLink(@event.Link));
                return;
            case IMinecraftPacket minecraftPacket:
                logger.LogTrace("Received packet {Packet} from {Side} {PlayerOrServer}", minecraftPacket, @event.From, @event.From.FromLink(@event.Link));
                return;
        }

        logger.LogTrace("Received packet {Packet}", @event.Message);
    }

    [Subscribe]
    public void OnMessageSent(MessageSentEvent @event)
    {
        switch (@event.Message)
        {
            case IBufferedBinaryMessage bufferedBinaryMessage:
                logger.LogTrace("Sent buffer length {Length} to {Direction} {PlayerOrServer}", bufferedBinaryMessage.Stream.Length, @event.To, @event.To.FromLink(@event.Link));
                return;
            case IMinecraftBinaryMessage binaryMessage:
                logger.LogTrace("Sent packet id 0x{PacketId:X2}, length {Length} to {Direction} {PlayerOrServer}", binaryMessage.Id, binaryMessage.Stream.Length, @event.To, @event.To.FromLink(@event.Link));
                return;
            case IMinecraftPacket minecraftPacket:
                logger.LogTrace("Sent packet {Packet} to {Direction} {PlayerOrServer}", minecraftPacket, @event.To, @event.To.FromLink(@event.Link));
                return;
        }

        logger.LogTrace("Sent packet {Packet}", @event.Message);
    }
}
