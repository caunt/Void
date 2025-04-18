﻿using Microsoft.Extensions.Logging;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Commands;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;

namespace Void.Proxy.Plugins.Common.Services.Commands;

public abstract class AbstractCommandService(ILogger logger, IEventService events, ICommandService commands) : IPluginCommonService
{
    public async ValueTask<bool> HandleCommandAsync(ILink link, string command, bool isSigned, CancellationToken cancellationToken)
    {
        var cancelled = await events.ThrowWithResultAsync(new ChatCommandEvent(link, command, isSigned), cancellationToken);

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

        player.GetLogger().LogInformation("Entered command: {Command}", @event.Command);

        try
        {
            await commands.ExecuteAsync(player, @event.Command, cancellationToken);
            @event.Result = true;
        }
        catch (CommandSyntaxException exception) when (exception.Message.Contains("Unknown command"))
        {
            // Ignore unknown commands
        }
        catch (CommandSyntaxException exception)
        {
            await player.SendChatMessageAsync(exception.Message, cancellationToken);
        }
    }

    protected abstract bool IsSupportedVersion(ProtocolVersion version);
}
