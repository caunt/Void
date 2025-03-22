using Void.Proxy.API.Mojang.Profiles;
using Void.Proxy.API.Players;

namespace Void.Proxy.API.Mojang.Services;

public interface IMojangService
{
    public ValueTask<GameProfile?> VerifyAsync(IPlayer player, ReadOnlyMemory<byte> secret, CancellationToken cancellationToken = default);
}