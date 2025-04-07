using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Players;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Mojang;

public interface IMojangService
{
    public ValueTask<GameProfile?> VerifyAsync(IMinecraftPlayer player, ReadOnlyMemory<byte> secret, CancellationToken cancellationToken = default);
}
