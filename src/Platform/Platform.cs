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
        LoggingLevelSwitch.MinimumLevel = LogEventLevel.Verbose;

        logger.LogInformation("Starting Void proxy");
        var startTime = Stopwatch.GetTimestamp();

        if (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) is { } currentDirectory)
        {
            logger.LogTrace("Changing working directory to {Path}", currentDirectory);
            Directory.SetCurrentDirectory(currentDirectory);
        }

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
        _listener.Start();

        logger.LogInformation("Connection listener started on port {Port}", settings.Port);

        _backgroundTask = ExecuteAsync(hostApplicationLifetime.ApplicationStopping);

        var totalTime = Stopwatch.GetElapsedTime(startTime);
        logger.LogInformation("Proxy started in {StartTimeSeconds} seconds!", totalTime.TotalSeconds.ToString("F"));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping proxy");
        await events.ThrowAsync<ProxyStoppingEvent>(cancellationToken);

        // TODO disconnect everyone here

        logger.LogInformation("Awaiting completion of connection listener");
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