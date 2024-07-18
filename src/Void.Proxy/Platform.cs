using System.Net.Sockets;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;
using Void.Proxy.API;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.API.Servers;
using Void.Proxy.API.Settings;

namespace Void.Proxy;

public class Platform(
    ILogger<Platform> logger,
    ISettings settings,
    IPluginService plugins,
    IEventService events,
    IPlayerService players,
    IServerService servers,
    IHostApplicationLifetime hostApplicationLifetime) : IProxy
{
    public static readonly LoggingLevelSwitch LoggingLevelSwitch = new();

    private Task? _backgroundTask;
    private TcpListener? _listener;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
        
        await settings.LoadAsync(cancellationToken: cancellationToken);
        LoggingLevelSwitch.MinimumLevel = (LogEventLevel)settings.LogLevel;

        foreach (var server in settings.Servers)
            servers.RegisterServer(server);

        await plugins.LoadAsync(cancellationToken: cancellationToken);
        await events.ThrowAsync<ProxyStart>(cancellationToken);

        _listener = new TcpListener(settings.Address, settings.Port);
        _listener.Start();

        logger.LogInformation("Listening for connections on port {Port}...", settings.Port);

        _backgroundTask = ExecuteAsync(hostApplicationLifetime.ApplicationStopping);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping proxy");

        // TODO disconnect everyone here

        if (_backgroundTask is not null)
            await _backgroundTask.ContinueWith(backgroundTask =>
            {
                if (backgroundTask.IsCanceled)
                    return;

                if (backgroundTask.IsCompletedSuccessfully)
                    return;

                throw backgroundTask.Exception?.Flatten()
                    .InnerException ?? new Exception("Proxy stopped with unknown exception");
            }, cancellationToken);

        _listener?.Stop();

        await settings.SaveAsync(cancellationToken: cancellationToken);
        await events.ThrowAsync<ProxyStop>(cancellationToken);
        await plugins.UnloadAsync(cancellationToken);
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_listener);
        while (!cancellationToken.IsCancellationRequested)
            await players.AcceptPlayerAsync(await _listener.AcceptTcpClientAsync(cancellationToken));
    }
}