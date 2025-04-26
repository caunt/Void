﻿using System.Reflection;

namespace Void.Proxy.Api.Events;

/// <summary>
/// Attribute to mark a method as an event listener.
/// </summary>
/// <param name="order">Specifies the order in which the event should be processed.</param>
/// <param name="bypassScopedFilter">Indicates if this event should be triggered by all players, not only scoped one.</param>
[AttributeUsage(AttributeTargets.Method)]
public class SubscribeAttribute(PostOrder order = PostOrder.Normal, bool bypassScopedFilter = false) : Attribute
{
    public PostOrder Order => order;
    public bool BypassScopedFilter => bypassScopedFilter;

    public static void SanityChecks(MethodInfo methodInfo)
    {
        var parameters = methodInfo.GetParameters();

        if (parameters.Length is < 1 or > 2)
            throw new ArgumentException("The method must have 1 or 2 parameters.");

        if (!parameters[0].ParameterType.IsAssignableTo(typeof(IEvent)))
            throw new ArgumentException("The first parameter must be of type IEvent.");

        if (parameters.Length == 2 && !parameters[1].ParameterType.IsAssignableTo(typeof(CancellationToken)))
            throw new ArgumentException("The second parameter must be of type CancellationToken.");
    }
}
