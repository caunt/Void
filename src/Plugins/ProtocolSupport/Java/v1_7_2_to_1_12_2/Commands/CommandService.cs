using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Services;
using Void.Proxy.Plugins.Common.Services.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Commands;

public class CommandService(IEventService events) : AbstractCommandService(events)
{
    [Subscribe]
    public async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case ChatMessagePacket chatMessagePacket:
                if (chatMessagePacket.Message[0] != '/')
                    return;

                await HandleCommandAsync(@event.Link, chatMessagePacket.Message[1..], false, cancellationToken);
                break;
        }
    }
}

