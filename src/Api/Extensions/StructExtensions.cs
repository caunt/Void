namespace Void.Proxy.Api.Extensions;

public static class StructExtensions
{
    public static bool IsDefault<T>(this T value) where T : struct
    {
        return value.Equals(default(T));
    }
}
