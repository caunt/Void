using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Void.Common.Players;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Events;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Players.Extensions;

public static class PlayerServiceExtensions
{
    public static bool TryGetByName(this IPlayerService players, string username, [NotNullWhen(true)] out IPlayer? player)
    {
        player = players.All.FirstOrDefault(player =>
        {
            if (!player.TryGetMinecraftPlayer(out var minecraftPlayer))
                return false;

            var profileUsername = minecraftPlayer.Profile?.Username ?? string.Empty;
            return profileUsername.Equals(username, StringComparison.InvariantCultureIgnoreCase);
        });

        return player is not null;
    }

    public static async ValueTask KickPlayerAsync(this IPlayerService players, IMinecraftPlayer player, Component? reason = null, CancellationToken cancellationToken = default)
    {
        await players.KickPlayerAsync(player, new MinecraftPlayerKickEvent(player, reason), cancellationToken);
    }
}
