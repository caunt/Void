using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Services;
using Void.Proxy.Plugins.Common.Network.Protocol.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Commands;

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
            case IChatCommand chatCommand:
                await HandleCommandAsync(@event.Link, chatCommand.Command, chatCommand.IsSigned, cancellationToken);
                break;
        }
    }
}

