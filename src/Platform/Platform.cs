using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;
using Void.Proxy.API;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Forwarding;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.API.Servers;
using Void.Proxy.API.Settings;

namespace Void.Proxy;

public class Platform(ILogger<Platform> logger, ISettings settings, IPluginService plugins, IEventService events, IPlayerService players, IServerService servers, IForwardingService forwardings, IHostApplicationLifetime hostApplicationLifetime) : IProxy
{
    public static readonly LoggingLevelSwitch LoggingLevelSwitch = new();

    private Task? _backgroundTask;
    private TcpListener? _listener;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        LoggingLevelSwitch.MinimumLevel = LogEventLevel.Debug;

        var startTime = Stopwatch.GetTimestamp();

        if (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) is { } currentDirectory)
            Directory.SetCurrentDirectory(currentDirectory);

        await plugins.LoadEmbeddedPluginsAsync(cancellationToken);
        await plugins.LoadPluginsAsync(cancellationToken: cancellationToken);
        await events.ThrowAsync<ProxyStartingEvent>(cancellationToken);

        forwardings.RegisterDefault();

        await settings.LoadAsync(cancellationToken: cancellationToken);

#if RELEASE
        LoggingLevelSwitch.MinimumLevel = (LogEventLevel)settings.LogLevel;
#endif

        foreach (var server in settings.Servers)
            servers.RegisterServer(server);

        _listener = new TcpListener(settings.Address, settings.Port);
        _listener.Start();

        logger.LogInformation("Listening for connections on port {Port}...", settings.Port);

        _backgroundTask = ExecuteAsync(hostApplicationLifetime.ApplicationStopping);

        var totalTime = Stopwatch.GetElapsedTime(startTime);
        logger.LogInformation("Proxy started in {StartTimeSeconds} seconds!", totalTime.TotalSeconds.ToString("F"));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await events.ThrowAsync<ProxyStoppingEvent>(cancellationToken);
        logger.LogInformation("Stopping proxy");

        // TODO disconnect everyone here

        if (_backgroundTask is not null)
            await _backgroundTask.ContinueWith(backgroundTask =>
            {
                if (backgroundTask.IsCanceled)
                    return;

                if (backgroundTask.IsCompletedSuccessfully)
                    return;

                throw backgroundTask.Exception?.Flatten().InnerException ?? new Exception("Proxy stopped with unknown exception");
            }, cancellationToken);

        _listener?.Stop();

        await settings.SaveAsync(cancellationToken: cancellationToken);

        await events.ThrowAsync<ProxyStoppedEvent>(cancellationToken);
        await plugins.UnloadPluginsAsync(cancellationToken);
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await events.ThrowAsync<ProxyStartedEvent>(cancellationToken);
        ArgumentNullException.ThrowIfNull(_listener);
        while (!cancellationToken.IsCancellationRequested)
            await players.AcceptPlayerAsync(await _listener.AcceptTcpClientAsync(cancellationToken), cancellationToken);
    }
}