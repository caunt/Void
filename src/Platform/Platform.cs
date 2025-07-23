using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;
using Void.Proxy.Api;
using Void.Proxy.Api.Console;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Settings;

namespace Void.Proxy;

public class Platform(
    ILogger<Platform> logger,
    IConsoleService console,
    IPluginService plugins,
    IEventService events,
    IPlayerService players,
    ISettings settings,
    IHostApplicationLifetime hostApplicationLifetime,
    InvocationContext context) : IProxy, IHostedService
{
    public static readonly LoggingLevelSwitch LoggingLevelSwitch = new();

    private static readonly Option<int> _portOption = new("--port", description: "Sets the listening port");
    private static readonly Option<string> _interfaceOption = new("--interface", description: "Sets the listening network interface");
    private static readonly Option<bool> _offlineOption = new("--offline", description: "Allows players to connect without Mojang authorization");

    private readonly IPAddress _interface = context.ParseResult.GetValueForOption(_interfaceOption) is { } value ? IPAddress.Parse(value) : settings.Address;
    private readonly int _port = context.ParseResult.HasOption(_portOption) ? context.ParseResult.GetValueForOption(_portOption) : settings.Port;

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

                logger.LogError("Address {Address}:{Port} already in use by another process. Retrying in 5 seconds ...", _interface, _port);
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
        console.Setup();

#if RELEASE
        LoggingLevelSwitch.MinimumLevel = (LogEventLevel)settings.LogLevel;
#else
        LoggingLevelSwitch.MinimumLevel = LogEventLevel.Debug;
#endif

        logger.LogInformation("Starting {Name} {Version} proxy", nameof(Void), "v" + GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);
        var startTime = Stopwatch.GetTimestamp();

        logger.LogTrace("Working directory is {Path}", Directory.GetCurrentDirectory());

        if (bool.TryParse(Environment.GetEnvironmentVariable("VOID_OFFLINE"), out var offlineVariable))
            settings.Offline = offlineVariable;

        if (context.ParseResult.GetValueForOption(_offlineOption) is { } option && option)
            settings.Offline = offlineVariable;

        if (settings.Offline)
            logger.LogWarning("Offline mode is enabled. Players will be able to connect without Mojang authorization.");

        logger.LogInformation("Loading plugins");
        await plugins.LoadPluginsAsync(cancellationToken: cancellationToken);

        await events.ThrowAsync<ProxyStartingEvent>(cancellationToken);

        logger.LogInformation("Starting connection listener");
        _listener = new TcpListener(_interface, _port);
        _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        await StartAcceptingConnectionsAsync(cancellationToken);

        logger.LogInformation("Connection listener started on address {Address}:{Port}", _interface, _port);

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

    public static void RegisterOptions(Command command)
    {
        command.AddOption(_interfaceOption);
        command.AddOption(_portOption);
        command.AddOption(_offlineOption);
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
