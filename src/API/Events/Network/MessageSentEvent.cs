using Void.Proxy.API.Links;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Messages;

namespace Void.Proxy.API.Events.Network;

public record MessageSentEvent(Side Origin, Side From, Side To, Direction Direction, IMinecraftMessage Message, ILink Link) : IEvent;