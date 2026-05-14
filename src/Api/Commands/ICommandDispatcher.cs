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
    /// <summary>
    /// Registers <paramref name="node" /> as a direct child of the root command node,
    /// making it available for dispatch and tab-completion.
    /// </summary>
    /// <param name="node">
    /// The command node to add. Must be a concrete <c>CommandNode</c> instance;
    /// any other implementation throws <see cref="System.ArgumentException" />.
    /// </param>
    public void Add(ICommandNode node);

    /// <summary>
    /// Registers multiple command nodes as direct children of the root command node,
    /// improving performance by reducing dispatch overhead.
    /// </summary>
    /// <param name="nodes">The command nodes to add.</param>
    public void AddRange(IEnumerable<ICommandNode> nodes);

    /// <summary>
    /// Gets the command node with the specified name.
    /// </summary>
    /// <param name="name">The name of the command node.</param>
    /// <returns>The command node, or null if not found.</returns>
    public ICommandNode? Get(string name);

    /// <summary>
    /// Tries to get the command node with the specified name.
    /// </summary>
    /// <param name="name">The name of the command node.</param>
    /// <param name="node">The command node, or null if not found.</param>
    /// <returns>True if the node was found; otherwise, false.</returns>
    public bool TryGet(string name, out ICommandNode? node);
}
