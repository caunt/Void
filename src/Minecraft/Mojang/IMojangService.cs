using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Mojang;

/// <summary>
/// Provides Mojang session authentication for online-mode players joining the proxy.
/// </summary>
public interface IMojangService
{
    /// <summary>
    /// Verifies that the player has authenticated with the Minecraft session server by computing a
    /// Java-style SHA-1 server ID from the encryption shared secret and the proxy's RSA public key,
    /// then querying <c>sessionserver.mojang.com/session/minecraft/hasJoined</c> with the player's
    /// username and the computed server ID.
    /// When offline mode is enabled, the verification step is skipped entirely and a
    /// <see cref="GameProfile"/> with an offline-mode UUID derived from the player's username is
    /// returned immediately.
    /// </summary>
    /// <param name="player">
    /// The player to authenticate. The player's profile must already contain at least a username
    /// before this method is called.
    /// </param>
    /// <param name="cancellationToken">A token to cancel the HTTP request to the session server.</param>
    /// <returns>
    /// The <see cref="GameProfile"/> returned by Mojang's session server if verification succeeds,
    /// or <see langword="null"/> if the session server responds with HTTP 204 No Content, which
    /// indicates that the player has not joined the session and authentication should be refused.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown when the player's profile is <see langword="null"/>.
    /// </exception>
    public ValueTask<GameProfile?> VerifyAsync(IPlayer player, CancellationToken cancellationToken = default);
}
