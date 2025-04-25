using Void.Minecraft.Network;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Players;

public interface IMinecraftPlayer : IPlayer, ICommandSource
{
    public ProtocolVersion ProtocolVersion { get; }

    public GameProfile? Profile { get; set; }
    public IdentifiedKey? IdentifiedKey { get; set; }
    public Phase Phase { get; set; }
}
