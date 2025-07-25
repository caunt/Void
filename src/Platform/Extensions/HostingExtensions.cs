﻿using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Settings;
using Void.Proxy.Configurations;

namespace Void.Proxy.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection AddSettings(this IServiceCollection services)
    {
        return services
            .AddSingleton<ISettings>(services =>
            {
                return services.GetRequiredService<IConfigurationService>().GetAsync<Settings>().AsTask().GetAwaiter().GetResult();
            })
            .Configure<HostOptions>(options =>
            {
                options.ServicesStartConcurrently = true;
                options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
                options.ShutdownTimeout = Timeout.InfiniteTimeSpan;
            });
    }
}
