using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Components.Text;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Players.Extensions;

public static class PlayerExtensions
{
    // extension(IPlayer player)
    // {
    //     public IMinecraftPlayer AsMinecraft => TryGetMinecraftPlayer(player, out var minecraftPlayer) ? minecraftPlayer : throw new InvalidOperationException($"Player is not a {nameof(IMinecraftPlayer)}.");
    // 
    //     public IMinecraftPlayer AsMinecraftPlayer()
    //     {
    //         return player.AsMinecraft;
    //     }
    // 
    //     public async ValueTask KickAsync(Component? reason = null, CancellationToken cancellationToken = default)
    //     {
    //         var players = player.Context.Services.GetRequiredService<IPlayerService>();
    // 
    //         if (player.TryGetMinecraftPlayer(out var minecraftPlayer))
    //             await players.KickPlayerAsync(minecraftPlayer, reason, cancellationToken);
    //         else
    //             await players.KickPlayerAsync(player, reason?.SerializeLegacy(), cancellationToken);
    //     }
    // 
    //     public bool TryGetMinecraftPlayer([MaybeNullWhen(false)] out IMinecraftPlayer minecraftPlayer)
    //     {
    //         minecraftPlayer = player as IMinecraftPlayer;
    //         return minecraftPlayer is not null;
    //     }
    // }

    public static IMinecraftPlayer AsMinecraftPlayer(this IPlayer player)
    {
        return TryGetMinecraftPlayer(player, out var minecraftPlayer) ? minecraftPlayer : throw new InvalidOperationException($"Player is not a {nameof(IMinecraftPlayer)}.");
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
