using Void.Minecraft.Commands.Brigadier.Tree.Nodes;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events.Chat;

public record AvailableCommandsEvent(ILink Link, IPlayer Player, RootCommandNode Node) : IScopedEvent;
