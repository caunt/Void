using System;
using Void.Minecraft.Commands.Brigadier.Tree;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events.Chat;

public record UnavailableCommandsEvent(ILink Link, IPlayer Player, Exception? Reason) : IScopedEvent
{
    public bool CopyProxyCommandNode { get; set; } = true;
    public RootCommandNode? CustomCommandNode { get; set; }

    public void ReplaceCommandNode(bool copyProxyCommandNode = true)
    {
        ReplaceCommandNode(commandNode: new RootCommandNode(), copyProxyCommandNode);
    }
    
    public void ReplaceCommandNode(RootCommandNode commandNode, bool copyProxyCommandNode = true)
    {
        CopyProxyCommandNode = copyProxyCommandNode;
        CustomCommandNode = commandNode;
    }
}
