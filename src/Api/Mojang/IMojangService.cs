using Void.Minecraft.Profiles;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Mojang;

public interface IMojangService
{
    public ValueTask<GameProfile?> VerifyAsync(IPlayer player, ReadOnlyMemory<byte> secret, CancellationToken cancellationToken = default);
}
