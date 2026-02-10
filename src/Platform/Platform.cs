using System.CommandLine;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Void.Proxy.Api;
using Void.Proxy.Api.Console;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Logging;
using Void.Proxy.Api.Network.Exceptions;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Settings;

namespace Void.Proxy;

public class Platform(
    ILogger<Platform> logger,
    ILogLevelSwitch logLevelSwitch,
    IRunOptions runOptions,
    IConsoleService console,
    IPluginService plugins,
    IEventService events,
    IPlayerService players,
    ISettings settings,
    IHostApplicationLifetime hostApplicationLifetime) : IProxy, IHostedService
{
    private readonly Option<int> _portOption = new("--port")
    {
        Description = "Sets the listening port"
    };

    private readonly Option<string> _interfaceOption = new("--interface")
    {
        Description = "Sets the listening network interface"
    };

    private readonly Option<bool> _offlineOption = new("--offline")
    {
        Description = "Allows players to connect without Mojang authorization"
    };

    private readonly Option<LogLevel> _loggingOption = new("--logging")
    {
        Description = "Sets the logging level"
    };

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

    private IPAddress Interface => console.TryGetOptionValue(_interfaceOption, out var value) && IPAddress.TryParse(value, out var address) ? address : settings.Address;
    private int Port => console.TryGetOptionValue(_portOption, out var value) ? value : settings.Port;

    public async ValueTask StartAcceptingConnectionsAsync(CancellationToken cancellationToken)
    {
        if (_listener is null)
            throw new InvalidOperationException("Listener is not created yet.");

        if (Status is ProxyStatus.Stopping)
            throw new InvalidOperationException("Proxy is stopping.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _listener.Start();
                break;
            }
            catch (SocketException exception)
            {
                if (exception.SocketErrorCode is not SocketError.AddressAlreadyInUse)
                    throw;

                logger.LogError("Address {Address}:{Port} already in use by another process. Retrying in 5 seconds ...", Interface, Port);
                await Task.Delay(5_000, cancellationToken);
            }
        }

        Status = ProxyStatus.Alive;
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
        logLevelSwitch.Level = settings.LogLevel;
#else
        logLevelSwitch.Level = LogLevel.Debug;
#endif

        if (console.TryGetOptionValue(_loggingOption, out var loggingOptionValue))
            logLevelSwitch.Level = loggingOptionValue;

        logger.LogInformation("Starting {Name} {Version} proxy", nameof(Void), $"v{GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
        var startTime = Stopwatch.GetTimestamp();

        logger.LogTrace("Working directory is {Path}", runOptions.WorkingDirectory);

        if (bool.TryParse(Environment.GetEnvironmentVariable("VOID_OFFLINE"), out var offlineVariable))
            settings.Offline = offlineVariable;

        if (console.TryGetOptionValue(_offlineOption, out var offlineOptionValue))
            settings.Offline = offlineOptionValue;

        if (settings.Offline)
            logger.LogWarning("Offline mode is enabled. Players will be able to connect without Mojang authorization.");

        logger.LogInformation("Loading plugins");
        await plugins.LoadPluginsAsync(cancellationToken: cancellationToken);

        await events.ThrowAsync<ProxyStartingEvent>(cancellationToken);

        logger.LogDebug("Starting connection listener");
        _listener = new TcpListener(Interface, Port);
        _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

        await StartAcceptingConnectionsAsync(cancellationToken);

        logger.LogInformation("Connection listener started on address {Address}:{Port}", Interface, Port);

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

        await players.ForEachAsync(async (player, cancellationToken) =>
        {
            try
            {
                await player.KickAsync("Proxy is shutting down", cancellationToken);
            }
            catch (StreamClosedException)
            {
                // Player disconnected before we could kick them, ignore
            }
        }, cancellationToken);

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
