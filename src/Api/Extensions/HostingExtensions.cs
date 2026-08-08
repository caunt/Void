using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Void.Proxy.Api.Extensions;

/// <summary>
/// Provides dependency-injection registration helpers used by proxy hosts.
/// </summary>
public static class HostingExtensions
{
    /// <summary>
    /// Configures JSON serialization to use camel-case names, indented output, and omission of null-valued properties.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        return services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.WriteIndented = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
    }
}
