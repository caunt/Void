using Void.Minecraft.Network;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Plugins.Common.Services.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Commands;

public class CommandService(IEventService events, ICommandService commands) : AbstractCommandService(events, commands)
{
    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }

    [Subscribe]
    public async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case ChatMessagePacket chatMessagePacket:
                var text = chatMessagePacket.Text;

                if (string.IsNullOrEmpty(text) || text[0] != '/')
                    return;

                await HandleCommandAsync(@event.Link, text[1..], false, cancellationToken);
                break;
            case IChatCommand chatCommand:
                await HandleCommandAsync(@event.Link, chatCommand.Command, chatCommand.IsSigned, cancellationToken);
                break;
        }
    }
}
