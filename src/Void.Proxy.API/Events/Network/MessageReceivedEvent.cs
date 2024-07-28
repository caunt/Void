using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Events.Network;

public class MessageReceivedEvent : IEventWithResult<bool>
{
    public required Side From { get; init; }
    public required Direction To { get; init; }
    public required IMinecraftMessage Message { get; init; }
    public required IPlayer Player { get; init; }
    public required IServer Server { get; init; }
    public required IMinecraftChannel PlayerChannel { get; init; }
    public required IMinecraftChannel ServerChannel { get; init; }

    /// <summary>
    ///     <see langword="true" /> if packet should not be sent
    /// </summary>
    public bool Result { get; set; }
}