namespace Void.Proxy.Api.Commands;

/// <summary>
/// Describes a completion that can replace part of a command input.
/// </summary>
public interface ICommandSuggestion
{
    /// <summary>
    /// Gets the zero-based input position at which replacement begins.
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// Gets the replacement text presented to the command source.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets optional descriptive text associated with the suggestion.
    /// </summary>
    /// <value>The tooltip text, or <see langword="null" /> when the suggestion has no tooltip.</value>
    public string? Tooltip { get; }
}
