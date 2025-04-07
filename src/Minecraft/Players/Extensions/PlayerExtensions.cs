using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Void.Common.Players;
using Void.Minecraft.Components.Text;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Players.Extensions;

public static class PlayerExtensions
{
    public static IMinecraftPlayer AsMinecraftPlayer(this IPlayer player)
    {
        if (!TryGetMinecraftPlayer(player, out var minecraftPlayer))
            throw new InvalidOperationException($"Player is not a {nameof(IMinecraftPlayer)}.");

        return minecraftPlayer;
    }

    public static async ValueTask KickAsync(this IPlayer player, Component? reason = null, CancellationToken cancellationToken = default)
    {
        var players = player.Context.Services.GetRequiredService<IPlayerService>();

        if (player.TryGetMinecraftPlayer(out var minecraftPlayer))
            await players.KickPlayerAsync(minecraftPlayer, reason, cancellationToken);
        else
            await players.KickPlayerAsync(player, reason?.SerializeLegacy(), cancellationToken);
    }

    public static bool TryGetMinecraftPlayer(this IPlayer player, [MaybeNullWhen(false)] out IMinecraftPlayer minecraftPlayer)
    {
        minecraftPlayer = player as IMinecraftPlayer;
        return minecraftPlayer is not null;
    }
}
