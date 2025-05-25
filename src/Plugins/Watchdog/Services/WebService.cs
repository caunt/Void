using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Void.Proxy.Api;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;

namespace Void.Proxy.Plugins.Watchdog.Services;

public class WebService(ILogger logger, IProxy proxy, Settings settings, WatchdogPlugin plugin) : IEventListener
{
    private IHost? _host;

    [Subscribe]
    public async ValueTask OnPluginLoading(PluginLoadingEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != plugin)
            return;

        _host = new HostBuilder()
            .ConfigureWebHostDefaults(builder => builder
                .UseUrls($"http://{settings.Address}:{settings.Port}")
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
        builder.MapGet("/health", () => TypedResults.Text(GetMessage()));
        builder.MapGet("/bound", () => TypedResults.Text(GetMessage(), statusCode: GetStatusCode()));
        builder.MapGet("/pause", () => proxy.PauseAcceptingConnections());
        builder.MapGet("/continue", async (CancellationToken cancellationToken) => await proxy.StartAcceptingConnectionsAsync(cancellationToken));
        builder.MapGet("/slow-stop", () => Stop(waitOnlinePlayers: true));
        builder.MapGet("/stop", () => Stop(waitOnlinePlayers: false));

        IResult Stop(bool waitOnlinePlayers)
        {
            proxy.PauseAcceptingConnections();
            proxy.Stop(waitOnlinePlayers);
            return TypedResults.Ok();
        }

        string GetMessage() => proxy.Status switch
        {
            ProxyStatus.Alive => "OK",
            ProxyStatus.Paused => "PAUSED",
            ProxyStatus.Stopping => "STOPPING",
            _ => "UNKNOWN"
        };

        int GetStatusCode() => proxy.Status switch
        {
            ProxyStatus.Alive => 200,
            _ => 503
        };
    }
}
