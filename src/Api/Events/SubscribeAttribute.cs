using System.Reflection;

namespace Void.Proxy.Api.Events;

/// <summary>
/// Attribute to mark a method as an event listener.
/// </summary>
/// <param name="order">Specifies the order in which the event should be processed.</param>
/// <param name="bypassScopedFilter">Indicates if this event should be triggered by all players, not only scoped one.</param>
[AttributeUsage(AttributeTargets.Method)]
public class SubscribeAttribute(PostOrder order = PostOrder.Normal, bool bypassScopedFilter = false) : Attribute
{
    /// <summary>
    /// Gets the priority used to order the annotated handler relative to other handlers.
    /// </summary>
    public PostOrder Order => order;

    /// <summary>
    /// Gets whether player-scope filtering is disabled for the annotated handler.
    /// </summary>
    public bool BypassScopedFilter => bypassScopedFilter;

    /// <summary>
    /// Validates that a subscribed method has a supported event-handler signature.
    /// </summary>
    /// <param name="methodInfo">The subscribed method to validate.</param>
    /// <exception cref="ArgumentException">
    /// The method does not have one or two parameters, its first parameter does not implement <see cref="IEvent" />, or its optional second parameter is not a <see cref="CancellationToken" />.
    /// </exception>
    public static void SanityChecks(MethodInfo methodInfo)
    {
        var parameters = methodInfo.GetParameters();

        if (parameters.Length is < 1 or > 2)
            throw new ArgumentException($"The method '{methodInfo.Name}' must have 1 or 2 parameters, but has {parameters.Length}.");

        if (!parameters[0].ParameterType.IsAssignableTo(typeof(IEvent)))
            throw new ArgumentException($"The method '{methodInfo.Name}' first parameter must be of type IEvent, but is {parameters[0].ParameterType.Name}.");

        if (parameters.Length == 2 && !parameters[1].ParameterType.IsAssignableTo(typeof(CancellationToken)))
            throw new ArgumentException($"The method '{methodInfo.Name}' second parameter must be of type CancellationToken, but is {parameters[1].ParameterType.Name}.");
    }
}
