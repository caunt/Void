using System;
using Void.Minecraft.Commands.Brigadier.Tree;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events.Chat;

/// <summary>
/// Signals that the server command tree could not be decoded and allows listeners to provide a replacement.
/// </summary>
/// <param name="Link">The link on which command-tree decoding failed.</param>
/// <param name="Player">The player that would receive the command tree.</param>
/// <param name="Reason">The decoding failure, or <see langword="null" /> when no exception was recorded.</param>
public record UnavailableCommandsEvent(ILink Link, IPlayer Player, Exception? Reason) : IScopedEvent
{
    /// <summary>
    /// Gets or sets whether proxy commands should be copied into a listener-provided replacement root.
    /// </summary>
    public bool CopyProxyCommandNode { get; set; } = true;

    /// <summary>
    /// Gets or sets the replacement command root supplied by listeners.
    /// </summary>
    /// <value>The replacement root, or <see langword="null" /> to leave the command tree unavailable.</value>
    public RootCommandNode? CustomCommandNode { get; set; }

    /// <summary>
    /// Creates an empty replacement root command node.
    /// </summary>
    /// <param name="copyProxyCommandNode">Whether subsequent handling should copy proxy commands into the new root.</param>
    public void ReplaceCommandNode(bool copyProxyCommandNode = true)
    {
        ReplaceCommandNode(commandNode: new RootCommandNode(), copyProxyCommandNode);
    }
    
    /// <summary>
    /// Selects a replacement command root and configures whether proxy commands are copied into it.
    /// </summary>
    /// <param name="commandNode">The root command node to use as the replacement.</param>
    /// <param name="copyProxyCommandNode">Whether subsequent handling should copy proxy commands into <paramref name="commandNode" />.</param>
    public void ReplaceCommandNode(RootCommandNode commandNode, bool copyProxyCommandNode = true)
    {
        CopyProxyCommandNode = copyProxyCommandNode;
        CustomCommandNode = commandNode;
    }
}
