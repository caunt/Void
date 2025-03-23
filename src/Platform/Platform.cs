using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;
using System.Net.Sockets;
using Void.Proxy.API;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Forwarding;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins.Services;
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

        logger.LogInformation("Starting Void proxy");
        var startTime = Stopwatch.GetTimestamp();

        logger.LogTrace("Changing working directory to {Path}", AppContext.BaseDirectory);
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        logger.LogInformation("Loading embedded plugins");
        await plugins.LoadEmbeddedPluginsAsync(cancellationToken);
        logger.LogInformation("Loading directory plugins");
        await plugins.LoadPluginsAsync(cancellationToken: cancellationToken);
        await events.ThrowAsync<ProxyStartingEvent>(cancellationToken);

        forwardings.RegisterDefault();

        logger.LogInformation("Loading settings file");
        await settings.LoadAsync(cancellationToken: cancellationToken);

#if RELEASE
        LoggingLevelSwitch.MinimumLevel = (LogEventLevel)settings.LogLevel;
#endif

        logger.LogInformation("Registering servers from settings file");
        foreach (var server in settings.Servers)
            servers.RegisterServer(server);

        logger.LogInformation("Starting connection listener");
        _listener = new TcpListener(settings.Address, settings.Port);
        _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _listener.Start();

        logger.LogInformation("Connection listener started on port {Port}", settings.Port);

        _backgroundTask = ExecuteAsync(hostApplicationLifetime.ApplicationStopping).ContinueWith(backgroundTask =>
        {
            if (backgroundTask.IsCanceled)
                return;

            if (backgroundTask.IsCompletedSuccessfully)
                return;

            logger.LogCritical(backgroundTask.Exception?.Flatten().InnerException, "Platform background task completed with exception");
            throw backgroundTask.Exception?.Flatten().InnerException ?? new Exception("Platform background task completed with unknown exception");
        }, cancellationToken);

        var totalTime = Stopwatch.GetElapsedTime(startTime);
        logger.LogInformation("Proxy started in {StartTimeSeconds} seconds!", totalTime.TotalSeconds.ToString("F"));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping proxy");
        await events.ThrowAsync<ProxyStoppingEvent>(cancellationToken);

        for (var i = players.All.Count - 1; i >= 0; i--)
            await players.KickPlayerAsync(players.All[i], "Proxy is shutting down", cancellationToken);

        logger.LogInformation("Awaiting completion of connection listener");
        if (_backgroundTask is not null)
            await _backgroundTask;

        _listener?.Stop();

        logger.LogInformation("Saving settings file");
        await settings.SaveAsync(cancellationToken: cancellationToken);

        await events.ThrowAsync<ProxyStoppedEvent>(cancellationToken);

        logger.LogInformation("Unloading plugins");
        await plugins.UnloadPluginsAsync(cancellationToken);

        logger.LogInformation("Proxy stopped!");
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await events.ThrowAsync<ProxyStartedEvent>(cancellationToken);
        ArgumentNullException.ThrowIfNull(_listener);
        while (!cancellationToken.IsCancellationRequested)
            await players.AcceptPlayerAsync(await _listener.AcceptTcpClientAsync(cancellationToken), cancellationToken);
    }
}