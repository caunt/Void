using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
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
                var legacyMessage = chatMessagePacket.Message.SerializeLegacy();

                if (string.IsNullOrEmpty(legacyMessage) || legacyMessage[0] != '/')
                    return;

                await HandleCommandAsync(@event.Link, legacyMessage[1..], false, cancellationToken);
                break;
        }
    }
}
