using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;

namespace Void.Proxy.Plugins.Watchdog.Services;

public class WebService(ILogger logger, Settings settings, WatchdogPlugin plugin) : IEventListener
{
    private IHost? _host;

    [Subscribe]
    public async ValueTask OnPluginLoading(PluginLoadingEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != plugin)
            return;

        _host = new HostBuilder()
            .ConfigureWebHostDefaults(builder => builder
                .UseUrls("http://*:" + settings.Port)
                .Configure(builder => builder
                    .UseRouting()
                    .UseEndpoints(ConfigureEndpoints)))
            .Build();

        await _host.StartAsync(cancellationToken);
        logger.LogInformation("Started web service on port {Port}", settings.Port);
    }

    [Subscribe]
    public async ValueTask OnPluginUnloading(PluginUnloadingEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != plugin)
            return;

        if (_host is not null)
            await _host.StopAsync(cancellationToken);
    }

    private void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/health", () => TypedResults.Ok("OK"));
    }
}
