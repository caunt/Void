using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Void.Proxy.API.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        return services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.WriteIndented = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
    }

    public static bool HasService<TInterface>(this IServiceCollection services)
    {
        return services.Any(descriptor => descriptor.ServiceType == typeof(TInterface));
    }
}