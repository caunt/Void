using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Mojang;

public interface IMojangService
{
    public ValueTask<GameProfile?> VerifyAsync(IPlayer player, ReadOnlyMemory<byte> secret, CancellationToken cancellationToken = default);
}
