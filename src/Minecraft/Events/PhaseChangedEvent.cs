using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events;

/// <summary>
/// Signals that one side of a player's Minecraft connection entered a new protocol phase.
/// </summary>
/// <param name="Link">The active link, or <see langword="null" /> when a phase changes before link creation.</param>
/// <param name="Player">The player whose connection phase changed.</param>
/// <param name="Side">The connection side that changed phase.</param>
/// <param name="Channel">The channel associated with that side.</param>
/// <param name="Phase">The newly entered protocol phase.</param>
public record PhaseChangedEvent(ILink? Link, IPlayer Player, Side Side, INetworkChannel Channel, Phase Phase) : IScopedEvent;
