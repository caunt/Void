using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;
using Void.Proxy.Api;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Settings;

namespace Void.Proxy;

public class Platform(
    ILogger<Platform> logger,
    IPluginService plugins,
    IEventService events,
    IPlayerService players,
    ISettings settings,
    IHostApplicationLifetime hostApplicationLifetime) : IProxy, IHostedService
{
    public static readonly LoggingLevelSwitch LoggingLevelSwitch = new();

    private Task? _backgroundTask;
    private TcpListener? _listener;

    public ProxyStatus Status
    {
        get;
        private set
        {
            logger.LogInformation("Proxy is {Status}", value);
            field = value;
        }
    }

    public void StartAcceptingConnections()
    {
        if (_listener is null)
            throw new InvalidOperationException("Listener is not created yet.");

        if (Status is ProxyStatus.Stopping)
            throw new InvalidOperationException("Proxy is stopping.");

        Status = ProxyStatus.Alive;
        _listener.Start();
    }

    public void PauseAcceptingConnections()
    {
        if (_listener is null)
            throw new InvalidOperationException("Listener is not created yet.");

        if (Status is ProxyStatus.Stopping)
            throw new InvalidOperationException("Proxy is stopping.");

        Status = ProxyStatus.Paused;
        _listener.Stop();
    }

    public void Stop(bool waitOnlinePlayers = false)
    {
        if (waitOnlinePlayers)
        {
            _ = Task.Run(async () =>
            {
                while (players.All.Any())
                {
                    logger.LogInformation("Waiting for {Count} players to disconnect ...", players.All.Count());
                    await Task.Delay(5_000);
                }

                hostApplicationLifetime.StopApplication();
            });
        }
        else
        {
            hostApplicationLifetime.StopApplication();
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
#if RELEASE
        LoggingLevelSwitch.MinimumLevel = (LogEventLevel)settings.LogLevel;
#else
        LoggingLevelSwitch.MinimumLevel = LogEventLevel.Debug;
#endif

        logger.LogInformation("Starting {Name} {Version} proxy", nameof(Void), "v" + GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);
        var startTime = Stopwatch.GetTimestamp();

        logger.LogTrace("Working directory is {Path}", Directory.GetCurrentDirectory());

        logger.LogInformation("Loading plugins");
        await plugins.LoadPluginsAsync(cancellationToken: cancellationToken);

        await events.ThrowAsync<ProxyStartingEvent>(cancellationToken);

        logger.LogInformation("Starting connection listener");
        _listener = new TcpListener(settings.Address, settings.Port);
        _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        StartAcceptingConnections();

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
        Status = ProxyStatus.Stopping;

        logger.LogInformation("Stopping proxy");
        await events.ThrowAsync<ProxyStoppingEvent>(cancellationToken);

        await players.ForEachAsync(async (player, cancellationToken) => await player.KickAsync("Proxy is shutting down", cancellationToken), cancellationToken);

        logger.LogInformation("Awaiting completion of connection listener");
        if (_backgroundTask is not null)
            await _backgroundTask;

        _listener?.Stop();

        await events.ThrowAsync<ProxyStoppedEvent>(cancellationToken);

        logger.LogInformation("Unloading plugins");
        await plugins.UnloadContainersAsync(cancellationToken);

        logger.LogInformation("Proxy stopped!");
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await events.ThrowAsync<ProxyStartedEvent>(cancellationToken);
        ArgumentNullException.ThrowIfNull(_listener);
        while (!cancellationToken.IsCancellationRequested)
        {
            if (Status is ProxyStatus.Stopping)
            {
                break;
            }
            if (Status is ProxyStatus.Paused)
            {
                await Task.Delay(1_000, cancellationToken);
                continue;
            }
            else
            {
                try
                {
                    await players.AcceptPlayerAsync(await _listener.AcceptTcpClientAsync(cancellationToken), cancellationToken);
                }
                catch (SocketException exception) when (exception.SocketErrorCode is SocketError.OperationAborted)
                {
                    continue;
                }
            }
        }
    }
}
