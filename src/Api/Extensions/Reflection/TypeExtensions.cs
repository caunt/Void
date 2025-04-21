using System.Reflection;

namespace Void.Proxy.Api.Extensions.Reflection;

public static class TypeExtensions
{
    private const BindingFlags DefaultFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

    internal static void InvokeMethod(this Type type, string methodName, params object[] args)
    {
        type.InvokeMethod<object>(methodName, args);
    }

    internal static T? InvokeMethod<T>(this Type type, string methodName, params object[] args)
    {
        var method = type.GetMethod(methodName, DefaultFlags) ??
            throw new MissingMethodException($"Method '{methodName}' not found in '{type}'.");

        return (T?)method.Invoke(null, args);
    }
}
