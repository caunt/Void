using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Network;
using Void.Proxy.Common.Services;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Services;

public class ChannelCoordinatorService : IPluginService
{
    [Subscribe(PostOrder.Last)]
    public void OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
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
}