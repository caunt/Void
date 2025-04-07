using Void.Common;
using Void.Proxy.Api.Links;

namespace Void.Proxy.Api.Events.Network;

public record MessageSentEvent(Side Origin, Side From, Side To, Direction Direction, INetworkMessage Message, ILink Link) : IEvent;
