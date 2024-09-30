using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Network;
using Void.Proxy.Common.Network.IO.Streams.Packet;
using Void.Proxy.Common.Services;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Services;

public class ChannelCoordinatorService : IPluginService
{
    [Subscribe]
    public void OnMessageSent(MessageSentEvent @event)
    {
        switch (@event.Message)
        {
            case LoginStartPacket:
                @event.Link.PlayerChannel.Pause();
                break;
            case EncryptionRequestPacket:
                @event.Link.PlayerChannel.Resume();
                @event.Link.ServerChannel.Pause();
                break;
            case EncryptionResponsePacket:
                @event.Link.PlayerChannel.Pause();
                @event.Link.ServerChannel.Resume();
                break;
            case SetCompressionPacket:
                if (@event.Link.PlayerChannel.IsPaused)
                    @event.Link.PlayerChannel.Resume();
                break;
            case LoginSuccessPacket:
                if (@event.Link.PlayerChannel.IsPaused)
                    @event.Link.PlayerChannel.Resume();
                break;
        }
    }

    [Subscribe(PostOrder.Last)]
    public void OnMessageSentLast(MessageSentEvent @event)
    {
        if (@event.Message is not EncryptionResponsePacket)
            return;

        // if encryption forced channel to remove packet stream, resume reading with what has left
        if (!@event.Link.PlayerChannel.Search<MinecraftPacketMessageStream>(out _))
            @event.Link.PlayerChannel.Resume();
    }
}