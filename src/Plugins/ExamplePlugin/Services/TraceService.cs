using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Network.IO.Messages.Binary;
using Void.Proxy.API.Network.IO.Messages.Packets;

namespace Void.Proxy.Plugins.ExamplePlugin.Services;

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
            case IBinaryMessage binaryPacket:
                logger.LogTrace("Received packet id {PacketId:X2}, length {Length} from {Side} {PlayerOrServer}", binaryPacket.Id, binaryPacket.Stream.Length, @event.From, @event.From.FromLink(@event.Link));
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
            case IBinaryMessage binaryPacket:
                logger.LogTrace("Sent packet id {PacketId:X2}, length {Length} to {Direction} {PlayerOrServer}", binaryPacket.Id, binaryPacket.Stream.Length, @event.To, @event.To.FromLink(@event.Link));
                return;
            case IMinecraftPacket minecraftPacket:
                logger.LogTrace("Sent packet {Packet} to {Direction} {PlayerOrServer}", minecraftPacket, @event.To, @event.To.FromLink(@event.Link));
                return;
        }

        logger.LogTrace("Sent packet {Packet}", @event.Message);
    }
}