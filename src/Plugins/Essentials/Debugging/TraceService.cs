using Microsoft.Extensions.Logging;
using Void.Minecraft.Network.Messages.Binary;
using Void.Minecraft.Network.Messages.Packets;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Messages;

namespace Void.Proxy.Plugins.Essentials.Debugging;

public partial class TraceService(ILogger<TraceService> logger) : IEventListener
{
    [Subscribe(PostOrder.First)]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        switch (@event.Message)
        {
            case IBufferedBinaryMessage bufferedBinaryMessage:
                LogReceivedBufferFromSide(bufferedBinaryMessage.Stream.Length, @event.From, @event.From.FromLink(@event.Link));
                return;
            case IMinecraftBinaryMessage binaryMessage:
                LogReceivedBinaryPacketFromSide(binaryMessage.Id, binaryMessage.Stream.Length, @event.From, @event.From.FromLink(@event.Link));
                return;
            case IMinecraftPacket minecraftPacket:
                LogReceivedPacketFromSide(minecraftPacket, @event.From, @event.From.FromLink(@event.Link));
                return;
            default:
                LogReceivedPacket(@event.Message);
                break;
        }
    }

    [Subscribe(PostOrder.Last)]
    public void OnMessageSent(MessageSentEvent @event)
    {
        switch (@event.Message)
        {
            case IBufferedBinaryMessage bufferedBinaryMessage:
                LogSentBufferToDirection(bufferedBinaryMessage.Stream.Length, @event.To, @event.To.FromLink(@event.Link));
                return;
            case IMinecraftBinaryMessage binaryMessage:
                LogSentBinaryPacketToDirection(binaryMessage.Id, binaryMessage.Stream.Length, @event.To, @event.To.FromLink(@event.Link));
                return;
            case IMinecraftPacket minecraftPacket:
                LogSentPacketToDirection(minecraftPacket, @event.To, @event.To.FromLink(@event.Link));
                return;
            default:
                LogSentPacket(@event.Message);
                break;
        }
    }

    [LoggerMessage(LogLevel.Trace, "Received buffer length {Length} from {Side} {PlayerOrServer}")]
    partial void LogReceivedBufferFromSide(long length, Side side, object? playerOrServer);

    [LoggerMessage(LogLevel.Trace, "Received packet id 0x{PacketId:X2}, length {Length} from {Side} {PlayerOrServer}")]
    partial void LogReceivedBinaryPacketFromSide(int packetId, long length, Side side, object? playerOrServer);

    [LoggerMessage(LogLevel.Trace, "Received packet {Packet} from {Side} {PlayerOrServer}")]
    partial void LogReceivedPacketFromSide(IMinecraftPacket packet, Side side, object? playerOrServer);

    [LoggerMessage(LogLevel.Trace, "Received packet {Packet}")]
    partial void LogReceivedPacket(INetworkMessage packet);

    [LoggerMessage(LogLevel.Trace, "Sent buffer length {Length} to {Direction} {PlayerOrServer}")]
    partial void LogSentBufferToDirection(long length, Side direction, object? playerOrServer);

    [LoggerMessage(LogLevel.Trace, "Sent packet id 0x{PacketId:X2}, length {Length} to {Direction} {PlayerOrServer}")]
    partial void LogSentBinaryPacketToDirection(int packetId, long length, Side direction, object? playerOrServer);

    [LoggerMessage(LogLevel.Trace, "Sent packet {Packet} to {Direction} {PlayerOrServer}")]
    partial void LogSentPacketToDirection(IMinecraftPacket packet, Side direction, object? playerOrServer);

    [LoggerMessage(LogLevel.Trace, "Sent packet {Packet}")]
    partial void LogSentPacket(INetworkMessage packet);
}
