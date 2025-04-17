﻿using Nito.Disposables.Internals;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Extensions;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Essentials.Moderation;

public class ModerationService(IPlayerService players, ICommandService commands) : IPluginCommonService
{
    [Subscribe]
    public void OnProxyStarted(ProxyStartedEvent _)
    {
        commands.Register(builder => builder
            .Literal("kick")
            .Then(builder => builder
                .Argument("name", Arguments.String())
                .Suggests(SuggestPlayer)
                .Executes(KickAsync)
                .Then(builder => builder
                    .Argument("reason", Arguments.GreedyString())
                    .Executes(KickAsync))));
    }

    private async ValueTask<int> KickAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var name = context.GetArgument<string>("name");
        var reason = context.TryGetArgument<string>("reason", out var textReason) ? textReason : "You have been kicked by an operator.";

        if (string.IsNullOrWhiteSpace(name))
            return 1;

        if (!TryGetPlayerByName(name, out var target))
            return 1;

        await target.KickAsync(reason, cancellationToken);
        return 0;
    }

    private bool TryGetPlayerByName(string name, [MaybeNullWhen(false)] out IMinecraftPlayer player)
    {
        player = null;

        foreach (var candidate in players.All)
        {
            if (!candidate.TryGetMinecraftPlayer(out var candidateMinecraftPlayer))
                continue;

            if (candidateMinecraftPlayer.Profile is null)
                continue;

            if (!candidateMinecraftPlayer.Profile.Username.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                continue;

            player = candidateMinecraftPlayer;
            return true;
        }

        return false;
    }

    private Suggestions SuggestPlayer(CommandContext context, SuggestionsBuilder builder)
    {
        return Suggestions.Create(context.Input, players.All.Select(player =>
        {
            if (!player.TryGetMinecraftPlayer(out var minecraftPlayer))
                return null;

            if (minecraftPlayer.Profile is null)
                return null;

            var name = minecraftPlayer.Profile.Username;
            return new Suggestion(StringRange.Between(0, context.Input.Length), name);
        }).WhereNotNull());
    }
}
