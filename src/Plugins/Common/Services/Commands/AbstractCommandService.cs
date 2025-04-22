using Microsoft.Extensions.Logging;
using Void.Minecraft.Events.Chat;
using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Commands;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;

namespace Void.Proxy.Plugins.Common.Services.Commands;

public abstract class AbstractCommandService(ILogger logger, IEventService events, ICommandService commands) : IPluginCommonService
{
    public async ValueTask<bool> HandleCommandAsync(ILink link, string command, bool isSigned, CancellationToken cancellationToken)
    {
        var cancelled = await events.ThrowWithResultAsync(new ChatCommandEvent(link, link.Player, command, isSigned), cancellationToken);

        if (isSigned && cancelled)
            logger.LogWarning("Signed command cannot be cancelled: {Command}", command);

        return cancelled;
    }

    [Subscribe]
    public async ValueTask OnChatCommand(ChatCommandEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Link.Player.TryGetMinecraftPlayer(out var player))
            return;

        if (!IsSupportedVersion(player.ProtocolVersion))
            return;

        @event.Result = await commands.ExecuteAsync(player, @event.Command, Side.Client, cancellationToken) is CommandExecutionResult.Executed;
    }

    [Subscribe]
    public async ValueTask OnChatMessageSend(ChatCommandSendEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Origin is not Side.Proxy)
            return;

        if (!@event.Player.TryGetMinecraftPlayer(out var player))
            return;

        if (!IsSupportedVersion(player.ProtocolVersion))
            return;

        await SendChatCommandAsync(@event.Player, @event.Command, cancellationToken);
    }

    protected abstract ValueTask<bool> SendChatCommandAsync(IMinecraftPlayer player, string command, CancellationToken cancellationToken);
    protected abstract bool IsSupportedVersion(ProtocolVersion version);
}
