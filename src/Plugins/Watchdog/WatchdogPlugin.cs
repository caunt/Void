using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Watchdog.Services;

namespace Void.Proxy.Plugins.Watchdog;

public class WatchdogPlugin(IDependencyService dependencies, IConfigurationService configs) : IPlugin
{
    public string Name => nameof(Watchdog);

    [Subscribe]
    public async ValueTask OnPluginLoading(PluginLoadingEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != this)
            return;

        var settings = await configs.GetAsync<Settings>(cancellationToken);

        if (!settings.Enabled && Environment.GetEnvironmentVariable("VOID_WATCHDOG_ENABLE") is not { } variable || !ParseBoolean(variable))
            return;

        dependencies.Register(services =>
        {
            services.AddSingleton(settings);
            services.AddSingleton<WebService>();
        });

        static bool ParseBoolean(string text) => double.TryParse(text, out var number) ? number > 0 : bool.Parse(text);
    }
}
