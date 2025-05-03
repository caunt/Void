using Microsoft.Extensions.Logging;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.Common.Services.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Extensions;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Commands;

public class CommandService(ILogger<CommandService> logger, IEventService events, ICommandService commands) : AbstractCommandService(logger, events, commands)
{
    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }

    protected override async ValueTask<bool> SendChatCommandAsync(IPlayer player, string command, CancellationToken cancellationToken)
    {
        if (!await player.IsPlayingAsync(cancellationToken))
            return false;

        if (!command.StartsWith('/'))
            command = "/" + command;

        await player.GetLink().SendPacketAsync(new ChatMessagePacket { Text = command }, cancellationToken);
        return true;
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

                @event.Result = await HandleCommandAsync(@event.Link, text[1..], false, cancellationToken);
                break;
        }
    }
}
