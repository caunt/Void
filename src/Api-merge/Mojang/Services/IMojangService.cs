using Void.Proxy.Api.Mojang.Profiles;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Mojang.Services;

public interface IMojangService
{
    public ValueTask<GameProfile?> VerifyAsync(IPlayer player, ReadOnlyMemory<byte> secret, CancellationToken cancellationToken = default);
}