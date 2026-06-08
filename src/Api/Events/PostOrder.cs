namespace Void.Proxy.Api.Events;

public enum PostOrder
{
    First = 0,
    Normal = 500,
    /// <summary>
    /// Runs the event handler after the normal priority handlers have completed.
    /// </summary>
    Last = 1000
}
