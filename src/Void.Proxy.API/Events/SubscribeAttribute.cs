using System.Reflection;

namespace Void.Proxy.API.Events;

[AttributeUsage(AttributeTargets.Method)]
public class SubscribeAttribute : Attribute
{
    public static void SanityChecks(MethodInfo methodInfo)
    {
        var parameters = methodInfo.GetParameters();

        if (parameters.Length is < 1 or > 2)
            throw new ArgumentException("The method must have 1 or 2 parameters.");

        if (!parameters[0]
                .ParameterType.IsAssignableTo(typeof(IEvent)))
            throw new ArgumentException("The first parameter must be of type IEvent.");

        if (parameters.Length == 2 && !parameters[1]
                .ParameterType.IsAssignableTo(typeof(CancellationToken)))
            throw new ArgumentException("The second parameter must be of type CancellationToken.");
    }
}