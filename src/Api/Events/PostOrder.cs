namespace Void.Proxy.Api.Events;

/// <summary>
/// Specifies the relative invocation order of subscribed event handlers.
/// </summary>
public enum PostOrder
{
    /// <summary>
    /// Runs the event handler before normal-priority handlers.
    /// </summary>
    First = 0,

    /// <summary>
    /// Runs the event handler at the default priority.
    /// </summary>
    Normal = 500,
    /// <summary>
    /// Runs the event handler after the normal priority handlers have completed.
    /// </summary>
    Last = 1000
}
