using Void.Minecraft.Network.Messages.Packets;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Plugins.Common.Events;

public record HandshakeBuildEvent(IPlayer Player, ILink Link) : IScopedEventWithResult<HandshakeBuildEventResult>
{
    public HandshakeBuildEventResult? Result { get; set; }
}

public record HandshakeBuildEventResult(IMinecraftServerboundPacket? Packet, string? ServerAddress, int NextState)
{
    public HandshakeBuildEventResult(int NextState) : this(Packet: null, ServerAddress: null, NextState)
    {
        // Intentionally left blank
    }
}
