using Void.Common.Commands;
using Void.Common.Players;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Players;

public interface IMinecraftPlayer : IPlayer, ICommandSource
{
    public string? Brand { get; set; }
    public ProtocolVersion ProtocolVersion { get; set; }
    public GameProfile? Profile { get; set; }
    public IdentifiedKey? IdentifiedKey { get; set; }
    public Phase Phase { get; set; }
}
