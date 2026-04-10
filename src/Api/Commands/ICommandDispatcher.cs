namespace Void.Proxy.Api.Commands;

/// <summary>
/// Manages the Brigadier command tree by registering top-level command nodes.
/// The concrete implementation is <c>CommandDispatcher</c>, which roots all nodes under a single <c>RootCommandNode</c>.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Registers <paramref name="node" /> as a direct child of the root command node,
    /// making it available for dispatch and tab-completion.
    /// </summary>
    /// <param name="node">
    /// The command node to add. Must be a concrete <c>CommandNode</c> instance;
    /// any other implementation throws <see cref="System.ArgumentException" />.
    /// </param>
    public void Add(ICommandNode node);
}
