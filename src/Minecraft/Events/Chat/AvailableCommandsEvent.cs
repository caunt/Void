using Void.Minecraft.Commands.Brigadier.Tree.Nodes;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events.Chat;

/// <summary>
/// Signals that a player's command tree was decoded and is available for inspection or augmentation.
/// </summary>
/// <param name="Link">The link on which the command tree was received.</param>
/// <param name="Player">The player receiving the commands.</param>
/// <param name="Node">The mutable root command node decoded from the server packet.</param>
public record AvailableCommandsEvent(ILink Link, IPlayer Player, RootCommandNode Node) : IScopedEvent;
