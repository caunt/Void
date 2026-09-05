using System.Threading.Channels;

namespace Void.Client;

/// <summary>
/// Serializes all lifecycle and X11 mutations through one channel. Status reads are lock-free because only the
/// channel reader publishes immutable snapshots.
/// </summary>
internal sealed class GameCoordinator(IGameRuntime runtime, ILogger<GameCoordinator> logger, SessionDiagnostics? diagnostics = null) : BackgroundService
{
    private readonly Channel<Message> _messages = Channel.CreateUnbounded<Message>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false,
        AllowSynchronousContinuations = false
    });
    private readonly List<Task> _ownedTasks = [];
    private readonly List<ConnectWaiter> _connectWaiters = [];
    private GameStatus _status = new(GameState.Idle, 0, null, OperationState.None, null, null, null, null, null, null, [], DateTimeOffset.UtcNow);
    private RunningGame? _game;
    private Guid? _sessionId;
    private CancellationTokenSource? _activeCancellation;
    private ServerAddress? _connectingServer;
    private ConnectGameResponse? _connectedResponse;
    private GameProcessExitException? _processExitFailure;
    private long? _connectOperationId;
    private long _nextOperationId;
    private CancellationToken _stoppingToken;
    private int _started;

    public GameStatus Status => Volatile.Read(ref _status);

    public bool IsHealthy => Volatile.Read(ref _started) is 1;

    public async Task<GameStatus> StartVanillaAsync(StartGameRequest request, CancellationToken cancellationToken)
    {
        return await EnqueueAsync<GameStatus>(completion => new StartMessage("start-vanilla", request, null, null, completion), cancellationToken);
    }

    public async Task<GameStatus> StartNeoForgeAsync(StartNeoForgeGameRequest request, CancellationToken cancellationToken)
    {
        return await EnqueueAsync<GameStatus>(completion => new StartMessage("start-neoforge", null, request, null, completion), cancellationToken);
    }

    public async Task<GameStatus> StartCurseForgeAsync(StartCurseForgeGameRequest request, CancellationToken cancellationToken)
    {
        return await EnqueueAsync<GameStatus>(completion => new StartMessage("start-curseforge", null, null, request, completion), cancellationToken);
    }

    public async Task<StopGameResponse> StopGameAsync(CancellationToken cancellationToken)
    {
        return await EnqueueAsync<StopGameResponse>(completion => new StopMessage(completion), cancellationToken);
    }

    public async Task<ConnectGameResponse> ConnectAsync(ConnectGameRequest request, CancellationToken cancellationToken)
    {
        return await EnqueueAsync<ConnectGameResponse>(completion => new ConnectMessage(request, completion, cancellationToken), cancellationToken);
    }

    public async Task SendChatAsync(SendChatRequest request, CancellationToken cancellationToken)
    {
        await EnqueueAsync<bool>(completion => new SendChatMessage(request, completion, cancellationToken), cancellationToken);
    }

    public async Task<byte[]> CaptureScreenshotAsync(CancellationToken cancellationToken)
    {
        return await EnqueueAsync<byte[]>(completion => new ScreenshotMessage(completion, cancellationToken), cancellationToken);
    }

    public async Task<GamePlayers> GetPlayersAsync(CancellationToken cancellationToken)
    {
        return await EnqueueAsync<GamePlayers>(completion => new PlayersMessage(completion, cancellationToken), cancellationToken);
    }

    public async Task WriteOptionsAsync(string options, CancellationToken cancellationToken)
    {
        await EnqueueAsync<bool>(completion => new OptionsMessage(options, completion, cancellationToken), cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;
        Volatile.Write(ref _started, 1);

        try
        {
            await foreach (var message in _messages.Reader.ReadAllAsync(stoppingToken))
                await HandleAsync(message);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Host shutdown owns cancellation and cleanup below.
        }
        finally
        {
            Volatile.Write(ref _started, 0);
            _messages.Writer.TryComplete();
            _activeCancellation?.Cancel();

            if (_game is not null)
            {
                try
                {
                    await runtime.StopAsync(_game, CancellationToken.None);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Failed to stop Minecraft during API shutdown");
                }
            }

            await Task.WhenAll(_ownedTasks);
            if (_sessionId is { } sessionId)
                await (diagnostics?.CompleteAsync(sessionId, CancellationToken.None) ?? Task.CompletedTask);
        }
    }

    private async Task HandleAsync(Message message)
    {
        using var diagnosticContext = diagnostics?.Enter(_sessionId);
        switch (message)
        {
            case StartMessage start:
                await HandleStartAsync(start);
                break;
            case StopMessage stop:
                await HandleStopAsync(stop);
                break;
            case ConnectMessage connect:
                await HandleConnectAsync(connect);
                break;
            case ConnectWaiterCanceled canceled:
                HandleConnectWaiterCanceled(canceled);
                break;
            case SendChatMessage chat:
                await HandleSendChatAsync(chat);
                break;
            case ScreenshotMessage screenshot:
                await HandleScreenshotAsync(screenshot);
                break;
            case PlayersMessage players:
                HandlePlayers(players);
                break;
            case OptionsMessage options:
                await HandleOptionsAsync(options);
                break;
            case StartCompleted completed:
                await HandleStartCompletedAsync(completed);
                break;
            case StopCompleted completed:
                await HandleStopCompletedAsync(completed);
                break;
            case ConnectCompleted completed:
                await HandleConnectCompletedAsync(completed);
                break;
            case VoidOperationCompleted completed:
                await HandleVoidOperationCompletedAsync(completed);
                break;
            case ScreenshotCompleted completed:
                await HandleScreenshotCompletedAsync(completed);
                break;
            case ProcessExited exited:
                await HandleProcessExitedAsync(exited);
                break;
            default:
                throw new InvalidOperationException($"Unknown coordinator message type {message.GetType().Name}");
        }
    }

    private async Task HandleStartAsync(StartMessage message)
    {
        if (_activeCancellation is not null || _game is not null || Status.State is not (GameState.Idle or GameState.Failed))
        {
            message.Completion.SetException(Conflict("A game is already running or changing state"));
            return;
        }

        var version = message.Request?.Version?.Trim();
        var neoForgeVersion = message.NeoForgeRequest?.Version?.Trim();
        var slug = message.CurseForgeRequest?.Slug?.Trim();
        var arguments = message.Request?.Arguments ?? message.NeoForgeRequest?.Arguments ?? message.CurseForgeRequest?.Arguments ?? [];
        var memoryMb = message.Request?.MemoryMb ?? message.NeoForgeRequest?.MemoryMb ?? message.CurseForgeRequest?.MemoryMb;

        if (message.Kind is "start-vanilla" && string.IsNullOrWhiteSpace(version))
        {
            message.Completion.SetException(BadRequest("version is required"));
            return;
        }

        if (message.Kind is "start-curseforge" && (string.IsNullOrWhiteSpace(slug) || message.CurseForgeRequest?.FileId <= 0))
        {
            message.Completion.SetException(BadRequest("slug and a positive fileId are required"));
            return;
        }

        if (memoryMb is <= 0)
        {
            message.Completion.SetException(BadRequest("memoryMb must be a positive integer"));
            return;
        }

        if (memoryMb is not null && arguments.Any(IsMaximumHeapArgument))
        {
            message.Completion.SetException(BadRequest("memoryMb cannot be combined with an -Xmx JVM argument"));
            return;
        }

        if (_sessionId is { } previousSession)
            await (diagnostics?.CompleteAsync(previousSession, _stoppingToken) ?? Task.CompletedTask);
        _sessionId = diagnostics is null ? null : await diagnostics.BeginAsync($"{message.Kind}:{version ?? neoForgeVersion ?? slug}:{message.CurseForgeRequest?.FileId}", Environment.GetEnvironmentVariable("MINECRAFT_DIRECTORY") ?? "/root/.minecraft", _stoppingToken);
        using var diagnosticContext = diagnostics?.Enter(_sessionId);
        var operationId = ++_nextOperationId;
        var operationCancellation = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken);
        _activeCancellation = operationCancellation;
        _connectedResponse = null;
        _processExitFailure = null;
        await PublishAsync(new(GameState.Starting, operationId, message.Kind, OperationState.Running, null, null, null, "Game launch accepted", null, null, [], DateTimeOffset.UtcNow));

        Task<RunningGame> operation = message.Kind switch
        {
            "start-vanilla" => runtime.LaunchVanillaAsync(version ?? "", arguments, memoryMb, operationCancellation.Token),
            "start-neoforge" => runtime.LaunchNeoForgeAsync(neoForgeVersion ?? "", arguments, memoryMb, operationCancellation.Token),
            "start-curseforge" => runtime.LaunchCurseForgeAsync(slug ?? "", message.CurseForgeRequest?.FileId ?? 0, arguments, memoryMb, operationCancellation.Token),
            _ => throw new InvalidOperationException($"Unknown launch kind {message.Kind}")
        };

        Own(ObserveStartAsync(operationId, message.Kind, operation, operationCancellation));
        message.Completion.SetResult(Status);
    }

    private async Task HandleStopAsync(StopMessage message)
    {
        if (Status.State is GameState.Stopping)
        {
            message.Completion.SetException(Conflict("The game is already stopping"));
            return;
        }

        _activeCancellation?.Cancel();
        CancelConnectWaiters();
        var operationId = ++_nextOperationId;
        var operationCancellation = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken);
        _activeCancellation = operationCancellation;
        await PublishAsync(Status with
        {
            State = GameState.Stopping,
            OperationId = operationId,
            Operation = "stop",
            OperationState = OperationState.Running,
            Message = "Stopping game",
            Error = null,
            Failure = null,
            UpdatedAt = DateTimeOffset.UtcNow
        });
        Own(ObserveStopAsync(operationId, runtime.StopAsync(_game, operationCancellation.Token), operationCancellation, message.Completion));
    }

    private async Task HandleConnectAsync(ConnectMessage message)
    {
        var host = message.Request.Host?.Trim();

        if (string.IsNullOrWhiteSpace(host) || message.Request.Port is < 1 or > 65535)
        {
            message.Completion.SetException(BadRequest("host and a port between 1 and 65535 are required"));
            return;
        }

        var server = new ServerAddress(host, message.Request.Port);

        if (_game is null)
        {
            message.Completion.SetException(Conflict("A running game is required before connecting"));
            return;
        }

        if (Status.State is GameState.Connected)
        {
            if (_connectedResponse?.Server == server)
                message.Completion.SetResult(_connectedResponse);
            else
                message.Completion.SetException(Conflict("The game is already connected to a different server"));

            return;
        }

        if (Status.State is not GameState.Ready)
        {
            message.Completion.SetException(Conflict("A ready game is required before connecting"));
            return;
        }

        if (_connectingServer is not null)
        {
            if (_connectingServer == server)
                AddConnectWaiter(message);
            else
                message.Completion.SetException(Conflict("A connection to a different server is already in progress"));

            return;
        }

        if (_activeCancellation is not null)
        {
            message.Completion.SetException(Conflict("Another game operation is running"));
            return;
        }

        var operationId = ++_nextOperationId;
        var cancellation = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken);
        _activeCancellation = cancellation;
        _connectingServer = server;
        _connectOperationId = operationId;
        AddConnectWaiter(message);
        await PublishAsync(Status with { OperationId = operationId, Operation = "connect", OperationState = OperationState.Running, Message = "connect running", Error = null, Failure = null, UpdatedAt = DateTimeOffset.UtcNow });
        Own(ObserveConnectAsync(operationId, server, runtime.ConnectAsync(_game, host, message.Request.Port, cancellation.Token), cancellation));
    }

    private void AddConnectWaiter(ConnectMessage message)
    {
        var registration = message.RequestCancellation.Register(() => _messages.Writer.TryWrite(new ConnectWaiterCanceled(message.Completion, message.RequestCancellation)));
        _connectWaiters.Add(new(message.Completion, registration));
    }

    private void HandleConnectWaiterCanceled(ConnectWaiterCanceled message)
    {
        var waiter = _connectWaiters.FirstOrDefault(waiter => waiter.Completion == message.Completion);

        if (waiter is null)
            return;

        waiter.CancellationRegistration.Dispose();
        _connectWaiters.Remove(waiter);
        waiter.Completion.TrySetCanceled(message.CancellationToken);

        // The accepted connection intent outlives individual HTTP waiters. Stop and process-exit paths still own
        // cancellation of the background operation.
    }

    private async Task HandleSendChatAsync(SendChatMessage message)
    {
        var text = message.Request.Message;

        if (_game is null || Status.State is not GameState.Connected)
        {
            message.Completion.SetException(Conflict("A connected game is required before sending chat"));
            return;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            message.Completion.SetException(BadRequest("message is required"));
            return;
        }

        if (_activeCancellation is not null)
        {
            message.Completion.SetException(Conflict("Another game operation is running"));
            return;
        }

        var (operationId, cancellation) = await BeginConfirmedOperationAsync("send-chat", message.RequestCancellation);
        Own(ObserveVoidOperationAsync(operationId, "send-chat", runtime.SendChatAsync(_game, text, cancellation.Token), cancellation, message.Completion));
    }

    private async Task HandleScreenshotAsync(ScreenshotMessage message)
    {
        if (_game is null || Status.State is not (GameState.Ready or GameState.Connected))
        {
            message.Completion.SetException(Conflict("A running game is required before taking a screenshot"));
            return;
        }

        if (_activeCancellation is not null)
        {
            message.Completion.SetException(Conflict("Another game operation is running"));
            return;
        }

        var (operationId, cancellation) = await BeginConfirmedOperationAsync("screenshot", message.RequestCancellation);
        Own(ObserveScreenshotAsync(operationId, runtime.CaptureScreenshotAsync(cancellation.Token), cancellation, message.Completion));
    }

    private void HandlePlayers(PlayersMessage message)
    {
        if (_game is null || Status.State is not (GameState.Ready or GameState.Connected))
        {
            message.Completion.SetException(Conflict("A running game is required before reading its players"));
            return;
        }

        Own(ObservePlayersAsync(runtime.ReadPlayersAsync(_game, message.RequestCancellation), message.Completion));
    }

    private async Task HandleOptionsAsync(OptionsMessage message)
    {
        if (_activeCancellation is not null)
        {
            message.Completion.SetException(Conflict("Options cannot change while another game operation is running"));
            return;
        }

        var (operationId, cancellation) = await BeginConfirmedOperationAsync("options", message.RequestCancellation);
        Own(ObserveVoidOperationAsync(operationId, "options", runtime.WriteOptionsAsync(message.Options, cancellation.Token), cancellation, message.Completion));
    }

    private async Task HandleStartCompletedAsync(StartCompleted completed)
    {
        completed.Cancellation.Dispose();

        if (completed.OperationId != Status.OperationId || Status.Operation is "stop")
        {
            if (completed.Game is not null)
                Own(CleanupSupersededGameAsync(completed.Game));

            return;
        }

        _activeCancellation = null;

        if (completed.Error is not null)
        {
            logger.LogError(completed.Error, "{Operation} failed", completed.Kind);
            await PublishAsync(Status with
            {
                State = completed.Canceled ? GameState.Idle : GameState.Failed,
                OperationState = completed.Canceled ? OperationState.Canceled : OperationState.Failed,
                Message = completed.Canceled ? "Game launch canceled" : "Game launch failed",
                Error = completed.Canceled ? null : completed.Error.Message,
                Failure = completed.Canceled ? null : FailureFor(completed.Error, completed.Kind),
                UpdatedAt = DateTimeOffset.UtcNow
            });
            if (_sessionId is { } sessionId)
                await (diagnostics?.CompleteAsync(sessionId, _stoppingToken) ?? Task.CompletedTask);
            return;
        }

        _game = completed.Game ?? throw new InvalidOperationException("A successful launch did not return a game process");
        await PublishAsync(Status with
        {
            State = GameState.Ready,
            OperationState = OperationState.Succeeded,
            ProcessId = _game.Process.Id,
            ExitCode = null,
            Message = "Game window is ready",
            Error = null,
            Failure = null,
            UpdatedAt = DateTimeOffset.UtcNow
        });
        Own(MonitorProcessAsync(_game));
    }

    private async Task HandleStopCompletedAsync(StopCompleted completed)
    {
        completed.Cancellation.Dispose();

        if (completed.OperationId != Status.OperationId)
            return;

        _activeCancellation = null;

        if (completed.Error is not null)
        {
            logger.LogError(completed.Error, "Game stop failed");
            await PublishAsync(Status with { State = GameState.Failed, OperationState = OperationState.Failed, Error = completed.Error.Message, Failure = FailureFor(completed.Error, "stop"), Message = "Game stop failed", UpdatedAt = DateTimeOffset.UtcNow });
            completed.Completion.SetException(completed.Error);
            return;
        }

        var exitCode = _game?.Process.ExitCode;
        _game?.Process.Dispose();
        _game = null;
        _connectedResponse = null;
        await PublishAsync(new(GameState.Idle, completed.OperationId, "stop", OperationState.Succeeded, null, exitCode, null, "Game stopped", null, null, [], DateTimeOffset.UtcNow));
        if (_sessionId is { } sessionId)
            await (diagnostics?.CompleteAsync(sessionId, _stoppingToken) ?? Task.CompletedTask);
        completed.Completion.SetResult(new(completed.Mode, Status));
    }

    private async Task HandleConnectCompletedAsync(ConnectCompleted completed)
    {
        completed.Cancellation.Dispose();

        if (_connectOperationId != completed.OperationId)
            return;

        var waiters = _connectWaiters.ToArray();
        _connectWaiters.Clear();

        foreach (var waiter in waiters)
            waiter.CancellationRegistration.Dispose();

        _connectingServer = null;
        _connectOperationId = null;

        if (completed.OperationId != Status.OperationId)
        {
            foreach (var waiter in waiters)
                waiter.Completion.TrySetException(Conflict($"connect was superseded by operation {Status.OperationId}"));

            return;
        }

        _activeCancellation = null;

        if (completed.Error is not null)
        {
            var operationState = completed.Canceled ? OperationState.Canceled : OperationState.Failed;
            await PublishAsync(Status with { OperationState = operationState, Message = $"connect {operationState.ToString().ToLowerInvariant()}", Error = completed.Canceled ? null : completed.Error.Message, Failure = completed.Canceled ? null : FailureFor(completed.Error, "connect"), UpdatedAt = DateTimeOffset.UtcNow });

            foreach (var waiter in waiters)
            {
                if (completed.Canceled)
                    waiter.Completion.TrySetCanceled();
                else
                    waiter.Completion.TrySetException(completed.Error);
            }

            return;
        }

        _connectedResponse = new(completed.Server, DateTimeOffset.UtcNow);
        await PublishAsync(Status with { State = GameState.Connected, Server = completed.Server, OperationState = OperationState.Succeeded, Message = "Interactive game connection confirmed", Error = null, Failure = null, UpdatedAt = DateTimeOffset.UtcNow });

        foreach (var waiter in waiters)
            waiter.Completion.TrySetResult(_connectedResponse);
    }

    private async Task HandleVoidOperationCompletedAsync(VoidOperationCompleted completed)
    {
        completed.Cancellation.Dispose();

        if (!await CompleteConfirmedOperationAsync(completed.OperationId, completed.Kind, completed.Error, completed.Canceled, completed.Completion))
            return;

        completed.Completion.SetResult(true);
    }

    private async Task HandleScreenshotCompletedAsync(ScreenshotCompleted completed)
    {
        completed.Cancellation.Dispose();

        if (!await CompleteConfirmedOperationAsync(completed.OperationId, "screenshot", completed.Error, completed.Canceled, completed.Completion))
            return;

        completed.Completion.SetResult(completed.Image ?? throw new InvalidOperationException("Screen capture returned no image"));
    }

    private async Task HandleProcessExitedAsync(ProcessExited exited)
    {
        if (_game?.Process.Id != exited.ProcessId)
            return;

        // Stop completion owns final state and disposal so an expected exit cannot race it into a false failure.
        if (Status.State is GameState.Stopping)
        {
            await PublishAsync(Status with { ProcessId = null, ExitCode = exited.ExitCode, UpdatedAt = DateTimeOffset.UtcNow });
            return;
        }

        var processExitedDuringOperation = _activeCancellation is not null || _connectWaiters.Count is not 0;
        var processFailure = exited.ExitCode is not 0 || processExitedDuringOperation
            ? new GameProcessExitException(exited.ExitCode, exited.WasOutOfMemoryKilled, exited.MemoryMb)
            : null;

        _game.Process.Dispose();
        _game = null;
        _connectedResponse = null;
        Volatile.Write(ref _processExitFailure, processFailure);
        _activeCancellation?.Cancel();

        if (processFailure is null)
            CancelConnectWaiters();
        else
            FailConnectWaiters(processFailure);

        _activeCancellation = null;
        await PublishAsync(Status with
        {
            State = processFailure is null ? GameState.Idle : GameState.Failed,
            OperationState = processFailure is null ? OperationState.Succeeded : OperationState.Failed,
            ProcessId = null,
            ExitCode = exited.ExitCode,
            Server = null,
            Message = processFailure is null ? "Game exited" : "Game exited unexpectedly",
            Error = processFailure?.Message,
            Failure = processFailure?.Failure,
            UpdatedAt = DateTimeOffset.UtcNow
        });
        if (_sessionId is { } sessionId)
            await (diagnostics?.CompleteAsync(sessionId, _stoppingToken) ?? Task.CompletedTask);
    }

    private async Task<(long OperationId, CancellationTokenSource Cancellation)> BeginConfirmedOperationAsync(string operation, CancellationToken requestCancellation)
    {
        var operationId = ++_nextOperationId;
        var cancellation = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken, requestCancellation);
        _activeCancellation = cancellation;
        await PublishAsync(Status with { OperationId = operationId, Operation = operation, OperationState = OperationState.Running, Message = $"{operation} running", Error = null, Failure = null, UpdatedAt = DateTimeOffset.UtcNow });
        return (operationId, cancellation);
    }

    private async Task<bool> CompleteConfirmedOperationAsync<T>(long operationId, string operation, Exception? error, bool canceled, TaskCompletionSource<T> completion)
    {
        if (operationId != Status.OperationId)
        {
            completion.TrySetException(Conflict($"{operation} was superseded by operation {Status.OperationId}"));
            return false;
        }

        _activeCancellation = null;

        if (error is null)
        {
            await PublishAsync(Status with { OperationState = OperationState.Succeeded, Message = $"{operation} succeeded", Error = null, Failure = null, UpdatedAt = DateTimeOffset.UtcNow });
            return true;
        }

        var operationState = canceled ? OperationState.Canceled : OperationState.Failed;
        await PublishAsync(Status with { OperationState = operationState, Message = $"{operation} {operationState.ToString().ToLowerInvariant()}", Error = canceled ? null : error.Message, Failure = canceled ? null : FailureFor(error, operation), UpdatedAt = DateTimeOffset.UtcNow });

        if (canceled)
            completion.SetCanceled();
        else
            completion.SetException(error);

        return false;
    }

    private async Task ObserveStartAsync(long operationId, string kind, Task<RunningGame> operation, CancellationTokenSource cancellation)
    {
        try
        {
            var game = await operation;
            await WriteCompletionAsync(new StartCompleted(operationId, kind, game, null, false, cancellation));
        }
        catch (OperationCanceledException exception) when (cancellation.IsCancellationRequested)
        {
            await WriteCompletionAsync(new StartCompleted(operationId, kind, null, exception, true, cancellation));
        }
        catch (Exception exception)
        {
            await CaptureFailureAsync(operationId);
            await WriteCompletionAsync(new StartCompleted(operationId, kind, null, exception, false, cancellation));
        }
    }

    private async Task ObserveStopAsync(long operationId, Task<StopMode> operation, CancellationTokenSource cancellation, TaskCompletionSource<StopGameResponse> completion)
    {
        try
        {
            var mode = await operation;
            await WriteCompletionAsync(new StopCompleted(operationId, mode, null, cancellation, completion));
        }
        catch (Exception exception)
        {
            await WriteCompletionAsync(new StopCompleted(operationId, default, exception, cancellation, completion));
        }
    }

    private async Task ObserveConnectAsync(long operationId, ServerAddress server, Task operation, CancellationTokenSource cancellation)
    {
        var (error, canceled) = await ObserveAsync(operation, cancellation, operationId);
        await WriteCompletionAsync(new ConnectCompleted(operationId, server, error, canceled, cancellation));
    }

    private async Task ObserveVoidOperationAsync(long operationId, string kind, Task operation, CancellationTokenSource cancellation, TaskCompletionSource<bool> completion)
    {
        var (error, canceled) = await ObserveAsync(operation, cancellation, operationId);
        await WriteCompletionAsync(new VoidOperationCompleted(operationId, kind, error, canceled, cancellation, completion));
    }

    private async Task ObserveScreenshotAsync(long operationId, Task<byte[]> operation, CancellationTokenSource cancellation, TaskCompletionSource<byte[]> completion)
    {
        try
        {
            var image = await operation;
            await WriteCompletionAsync(new ScreenshotCompleted(operationId, image, null, false, cancellation, completion));
        }
        catch (OperationCanceledException exception) when (cancellation.IsCancellationRequested)
        {
            if (Volatile.Read(ref _processExitFailure) is { } processExitFailure)
                await WriteCompletionAsync(new ScreenshotCompleted(operationId, null, processExitFailure, false, cancellation, completion));
            else
                await WriteCompletionAsync(new ScreenshotCompleted(operationId, null, exception, true, cancellation, completion));
        }
        catch (Exception exception)
        {
            await WriteCompletionAsync(new ScreenshotCompleted(operationId, null, exception, false, cancellation, completion));
        }
    }

    private static async Task ObservePlayersAsync(Task<GamePlayers> operation, TaskCompletionSource<GamePlayers> completion)
    {
        try
        {
            completion.TrySetResult(await operation);
        }
        catch (Exception exception)
        {
            completion.TrySetException(exception);
        }
    }

    private async Task<(Exception? Error, bool Canceled)> ObserveAsync(Task operation, CancellationTokenSource cancellation, long operationId)
    {
        try
        {
            await operation;
            return (null, false);
        }
        catch (OperationCanceledException exception) when (cancellation.IsCancellationRequested)
        {
            return Volatile.Read(ref _processExitFailure) is { } processExitFailure
                ? (processExitFailure, false)
                : (exception, true);
        }
        catch (Exception exception)
        {
            await CaptureFailureAsync(operationId);
            return (exception, false);
        }
    }

    private async Task CaptureFailureAsync(long operationId)
    {
        if (diagnostics?.CurrentSessionId is not { } sessionId)
            return;
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        try
        {
            var screenshot = await runtime.CaptureScreenshotAsync(timeout.Token);
            await diagnostics.SaveScreenshotAsync(sessionId, operationId, screenshot, timeout.Token);
        }
        catch (Exception exception)
        {
            await diagnostics.WarnAsync(sessionId, $"Failure screenshot unavailable: {exception.Message}", _stoppingToken);
        }
        await diagnostics.CollectAsync(sessionId, _stoppingToken);
    }

    private async Task MonitorProcessAsync(RunningGame game)
    {
        await game.Process.WaitForExitAsync(CancellationToken.None);
        await WriteCompletionAsync(new ProcessExited(game.Process.Id, game.Process.ExitCode ?? -1, game.Process.WasOutOfMemoryKilled, game.Process.MemoryMb));
    }

    private async Task CleanupSupersededGameAsync(RunningGame game)
    {
        try
        {
            await runtime.StopAsync(game, CancellationToken.None);
        }
        finally
        {
            game.Process.Dispose();
        }
    }

    private async Task WriteCompletionAsync(Message message)
    {
        if (!await _messages.Writer.WaitToWriteAsync(CancellationToken.None) || !_messages.Writer.TryWrite(message))
            logger.LogError("Coordinator stopped before it could record {MessageType}", message.GetType().Name);
    }

    private async Task<T> EnqueueAsync<T>(Func<TaskCompletionSource<T>, Message> createMessage, CancellationToken cancellationToken)
    {
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        await _messages.Writer.WriteAsync(createMessage(completion), cancellationToken);
        return await completion.Task.WaitAsync(cancellationToken);
    }

    private void Own(Task task)
    {
        _ownedTasks.Add(task);
    }

    private async Task PublishAsync(GameStatus status)
    {
        status = status with { SessionId = _sessionId };
        await (diagnostics?.RecordAsync(status, _stoppingToken) ?? Task.CompletedTask);
        Volatile.Write(ref _status, status);
    }

    private static ClientFailure FailureFor(Exception exception, string operation, string stage = "coordinator")
    {
        return ClientFailure.FromException("client.operation.failed", operation, stage, exception);
    }

    private void CancelConnectWaiters()
    {
        foreach (var waiter in _connectWaiters)
        {
            waiter.CancellationRegistration.Dispose();
            waiter.Completion.TrySetCanceled();
        }

        _connectWaiters.Clear();
        _connectingServer = null;
        _connectOperationId = null;
    }

    private void FailConnectWaiters(Exception exception)
    {
        foreach (var waiter in _connectWaiters)
        {
            waiter.CancellationRegistration.Dispose();
            waiter.Completion.TrySetException(exception);
        }

        _connectWaiters.Clear();
        _connectingServer = null;
        _connectOperationId = null;
    }

    private static bool IsMaximumHeapArgument(string argument)
    {
        return argument.StartsWith("-Xmx", StringComparison.Ordinal)
               || argument.StartsWith("--jvm-arg=-Xmx", StringComparison.Ordinal);
    }

    private static GameCommandException BadRequest(string message) => new(StatusCodes.Status400BadRequest, message);

    private static GameCommandException Conflict(string message) => new(StatusCodes.Status409Conflict, message);

    private abstract record Message;
    private sealed record StartMessage(string Kind, StartGameRequest? Request, StartNeoForgeGameRequest? NeoForgeRequest, StartCurseForgeGameRequest? CurseForgeRequest, TaskCompletionSource<GameStatus> Completion) : Message;
    private sealed record StopMessage(TaskCompletionSource<StopGameResponse> Completion) : Message;
    private sealed record ConnectMessage(ConnectGameRequest Request, TaskCompletionSource<ConnectGameResponse> Completion, CancellationToken RequestCancellation) : Message;
    private sealed record ConnectWaiterCanceled(TaskCompletionSource<ConnectGameResponse> Completion, CancellationToken CancellationToken) : Message;
    private sealed record SendChatMessage(SendChatRequest Request, TaskCompletionSource<bool> Completion, CancellationToken RequestCancellation) : Message;
    private sealed record ScreenshotMessage(TaskCompletionSource<byte[]> Completion, CancellationToken RequestCancellation) : Message;
    private sealed record PlayersMessage(TaskCompletionSource<GamePlayers> Completion, CancellationToken RequestCancellation) : Message;
    private sealed record OptionsMessage(string Options, TaskCompletionSource<bool> Completion, CancellationToken RequestCancellation) : Message;
    private sealed record StartCompleted(long OperationId, string Kind, RunningGame? Game, Exception? Error, bool Canceled, CancellationTokenSource Cancellation) : Message;
    private sealed record StopCompleted(long OperationId, StopMode Mode, Exception? Error, CancellationTokenSource Cancellation, TaskCompletionSource<StopGameResponse> Completion) : Message;
    private sealed record ConnectCompleted(long OperationId, ServerAddress Server, Exception? Error, bool Canceled, CancellationTokenSource Cancellation) : Message;
    private sealed record VoidOperationCompleted(long OperationId, string Kind, Exception? Error, bool Canceled, CancellationTokenSource Cancellation, TaskCompletionSource<bool> Completion) : Message;
    private sealed record ScreenshotCompleted(long OperationId, byte[]? Image, Exception? Error, bool Canceled, CancellationTokenSource Cancellation, TaskCompletionSource<byte[]> Completion) : Message;
    private sealed record ProcessExited(int ProcessId, int ExitCode, bool WasOutOfMemoryKilled, int? MemoryMb) : Message;
    private sealed record ConnectWaiter(TaskCompletionSource<ConnectGameResponse> Completion, CancellationTokenRegistration CancellationRegistration);
}
