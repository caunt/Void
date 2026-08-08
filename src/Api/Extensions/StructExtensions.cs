namespace Void.Proxy.Api.Extensions;

/// <summary>
/// Provides value-type comparison helpers.
/// </summary>
public static class StructExtensions
{
    /// <summary>
    /// Determines whether a value equals the default value of its type.
    /// </summary>
    /// <typeparam name="T">The value type being compared.</typeparam>
    /// <param name="value">The value to compare.</param>
    /// <returns><see langword="true" /> when <paramref name="value" /> equals <see langword="default" />(<typeparamref name="T" />); otherwise, <see langword="false" />.</returns>
    public static bool IsDefault<T>(this T value) where T : struct
    {
        return value.Equals(default(T));
    }
}
