using Microsoft.Extensions.Logging;
using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Plugins.Common.Services.Commands;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Extensions;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Commands;

public class CommandService(ILogger<CommandService> logger, IEventService events, ICommandService commands) : AbstractCommandService(logger, events, commands)
{
    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }

    protected override async ValueTask<bool> SendChatCommandAsync(IMinecraftPlayer player, string command, CancellationToken cancellationToken)
    {
        if (!await player.IsPlayingAsync(cancellationToken))
            return false;

        await player.SendPacketAsync(new ChatCommandPacket { Command = command }, cancellationToken);
        return true;
    }

    [Subscribe]
    public async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case ChatCommandPacket chatCommand:
                @event.Result = await HandleCommandAsync(@event.Link, chatCommand.Command, false, cancellationToken);
                break;
            case SignedChatCommandPacket signedChatCommand:
                @event.Result = await HandleCommandAsync(@event.Link, signedChatCommand.Command, true, cancellationToken);
                break;
        }
    }
}
