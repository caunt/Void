using Void.Minecraft.Network;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Players;

public interface IMinecraftPlayer : IPlayer, ICommandSource
{
    public string? Brand { get; set; }
    public ProtocolVersion ProtocolVersion { get; set; }
    public GameProfile? Profile { get; set; }
    public IdentifiedKey? IdentifiedKey { get; set; }
    public Phase Phase { get; set; }
}
