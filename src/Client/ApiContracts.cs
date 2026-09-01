namespace Void.Client;

/// <summary>Stable lifecycle states exposed by the client API.</summary>
internal enum GameState
{
    Idle,
    Starting,
    Ready,
    Connected,
    Stopping,
    Failed
}

/// <summary>Outcome of the most recently accepted operation.</summary>
internal enum OperationState
{
    None,
    Running,
    Succeeded,
    Failed,
    Canceled
}

internal enum StopMode
{
    AlreadyStopped,
    Graceful,
    Forced
}

internal sealed record StartGameRequest(string? Version, string[]? Arguments);

internal sealed record StartNeoForgeGameRequest(string? Version, string[]? Arguments);

internal sealed record StartCurseForgeGameRequest(string? Slug, int FileId, string[]? Arguments);

internal sealed record ConnectGameRequest(string? Host, int Port);

internal sealed record SendChatRequest(string? Message);

internal sealed record ServerAddress(string Host, int Port);

/// <summary>
/// Immutable coordinator snapshot. The operation identifier lets callers distinguish completion of their accepted
/// command from a later command issued by another caller.
/// </summary>
internal sealed record GameStatus(
    GameState State,
    long OperationId,
    string? Operation,
    OperationState OperationState,
    int? ProcessId,
    int? ExitCode,
    ServerAddress? Server,
    string? Message,
    string? Error,
    IReadOnlyList<string> Warnings,
    DateTimeOffset UpdatedAt);

internal sealed record StopGameResponse(StopMode Mode, GameStatus Status);

internal sealed record ConnectGameResponse(ServerAddress Server, DateTimeOffset ConnectedAt);

internal sealed record Position(double X, double Y, double Z);

internal sealed record BodyRotation(double Yaw);

internal sealed record HeadRotation(double Yaw, double Pitch);

internal sealed record GamePlayer(string? Uuid, string? Name, Position Position, BodyRotation Body, HeadRotation Head);

internal sealed record RemoteGamePlayer(string? Uuid, string? Name, Position Position, BodyRotation Body, HeadRotation Head);

internal sealed record GamePlayers(GamePlayer Local, IReadOnlyList<RemoteGamePlayer> Remote);

internal sealed record GameTrackerConnection(string DescriptorPath, string Token, string? ExpectedName);

internal sealed record RunningGame(IManagedProcess Process, string Version, DateTimeOffset StartedAt, GameTrackerConnection Tracker);

internal interface IManagedProcess : IDisposable
{
    int Id { get; }
    bool HasExited { get; }
    int? ExitCode { get; }
    Task WaitForExitAsync(CancellationToken cancellationToken);
    void KillTree();
}

internal interface IGameRuntime
{
    Task WriteOptionsAsync(string options, CancellationToken cancellationToken);
    Task<RunningGame> LaunchVanillaAsync(string version, IReadOnlyList<string> arguments, CancellationToken cancellationToken);
    Task<RunningGame> LaunchNeoForgeAsync(string version, IReadOnlyList<string> arguments, CancellationToken cancellationToken);
    Task<RunningGame> LaunchCurseForgeAsync(string slug, int fileId, IReadOnlyList<string> arguments, CancellationToken cancellationToken);
    Task ConnectAsync(string host, int port, CancellationToken cancellationToken);
    Task SendChatAsync(string message, CancellationToken cancellationToken);
    Task<byte[]> CaptureScreenshotAsync(CancellationToken cancellationToken);
    Task<GamePlayers> ReadPlayersAsync(RunningGame game, CancellationToken cancellationToken);
    Task<StopMode> StopAsync(RunningGame? game, CancellationToken cancellationToken);
}

internal sealed class GameCommandException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

internal sealed class GamePlayersException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
