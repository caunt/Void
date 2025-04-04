﻿using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Plugins.Common.Services.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Commands;

public class CommandService(IEventService events) : AbstractCommandService(events)
{
    [Subscribe]
    public async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case ChatCommandPacket chatCommand:
                await HandleCommandAsync(@event.Link, chatCommand.Command, false, cancellationToken);
                break;
            case SignedChatCommandPacket signedChatCommand:
                await HandleCommandAsync(@event.Link, signedChatCommand.Command, true, cancellationToken);
                break;
        }
    }
}
