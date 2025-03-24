using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.IO.Messages;

namespace Void.Proxy.Api.Events.Network;

public record MessageSentEvent(Side Origin, Side From, Side To, Direction Direction, IMinecraftMessage Message, ILink Link) : IEvent;