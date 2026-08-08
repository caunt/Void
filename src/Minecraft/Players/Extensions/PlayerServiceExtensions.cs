using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Players.Extensions;

/// <summary>
/// Provides Minecraft-profile lookup operations for the proxy player service.
/// </summary>
public static class PlayerServiceExtensions
{
    /// <summary>
    /// Attempts to find a Minecraft player by authenticated profile name.
    /// </summary>
    /// <param name="players">The player service to search.</param>
    /// <param name="username">The profile name to match using an invariant-culture, case-insensitive comparison.</param>
    /// <param name="player">When this method returns <see langword="true" />, the first matching player; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when a matching Minecraft profile is found; otherwise, <see langword="false" />.</returns>
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
}
