using Microsoft.Extensions.Logging.Abstractions;
using Void.Client;
using Xunit;

namespace Void.Client.UnitTests;

public sealed class GameCoordinatorTests
{
    [Fact]
    public async Task CoordinatorSupportsMultipleConfirmedGameSessions()
    {
        var runtime = new FakeGameRuntime();
        using var coordinator = new GameCoordinator(runtime, NullLogger<GameCoordinator>.Instance);
        await coordinator.StartAsync(CancellationToken.None);

        var firstLaunch = await coordinator.StartVanillaAsync(new("1.21.1", []), CancellationToken.None);
        Assert.Equal(GameState.Starting, firstLaunch.State);
        Assert.True(firstLaunch.OperationId > 0);

        runtime.CompleteLaunch();
        await WaitForStateAsync(coordinator, GameState.Ready);
        var firstStop = await coordinator.StopGameAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        Assert.Equal(StopMode.Graceful, firstStop.Mode);
        Assert.Equal(GameState.Idle, firstStop.Status.State);

        var secondLaunch = await coordinator.StartVanillaAsync(new("1.20.1", []), CancellationToken.None);
        Assert.True(secondLaunch.OperationId > firstLaunch.OperationId);
        runtime.CompleteLaunch();
        await WaitForStateAsync(coordinator, GameState.Ready);

        var secondStop = await coordinator.StopGameAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        Assert.Equal(GameState.Idle, secondStop.Status.State);
        Assert.Equal(2, runtime.LaunchCount);
        Assert.Equal(2, runtime.StopCount);

        await coordinator.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ConcurrentLaunchIsRejectedWithoutStartingAnotherProcess()
    {
        var runtime = new FakeGameRuntime();
        using var coordinator = new GameCoordinator(runtime, NullLogger<GameCoordinator>.Instance);
        await coordinator.StartAsync(CancellationToken.None);

        await coordinator.StartNeoForgeAsync(new([]), CancellationToken.None);
        var exception = await Assert.ThrowsAsync<GameCommandException>(() => coordinator.StartNeoForgeAsync(new([]), CancellationToken.None));

        Assert.Equal(409, exception.StatusCode);
        Assert.Equal(1, runtime.LaunchCount);

        await coordinator.StopGameAsync(CancellationToken.None);
        await coordinator.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task InvalidLaunchIsRejectedBeforeRuntimeInvocation()
    {
        var runtime = new FakeGameRuntime();
        using var coordinator = new GameCoordinator(runtime, NullLogger<GameCoordinator>.Instance);
        await coordinator.StartAsync(CancellationToken.None);

        var exception = await Assert.ThrowsAsync<GameCommandException>(() => coordinator.StartVanillaAsync(new(" ", []), CancellationToken.None));

        Assert.Equal(400, exception.StatusCode);
        Assert.Equal(0, runtime.LaunchCount);

        await coordinator.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ConcurrentStopIsRejectedWithoutSupersedingTheAcceptedStop()
    {
        var runtime = new FakeGameRuntime { BlockStop = true };
        using var coordinator = new GameCoordinator(runtime, NullLogger<GameCoordinator>.Instance);
        await coordinator.StartAsync(CancellationToken.None);

        await coordinator.StartVanillaAsync(new("1.21.1", []), CancellationToken.None);
        runtime.CompleteLaunch();
        await WaitForStateAsync(coordinator, GameState.Ready);

        var acceptedStop = coordinator.StopGameAsync(CancellationToken.None);
        await WaitForStateAsync(coordinator, GameState.Stopping);
        var exception = await Assert.ThrowsAsync<GameCommandException>(() => coordinator.StopGameAsync(CancellationToken.None));

        Assert.Equal(409, exception.StatusCode);
        runtime.CompleteStop();
        var response = await acceptedStop.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        Assert.Equal(GameState.Idle, response.Status.State);

        await coordinator.StopAsync(CancellationToken.None);
    }

    private static async Task WaitForStateAsync(GameCoordinator coordinator, GameState expected)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        while (coordinator.Status.State != expected)
            await Task.Delay(10, timeout.Token);
    }

    private sealed class FakeGameRuntime : IGameRuntime
    {
        private TaskCompletionSource<RunningGame> _launch = CreateLaunchCompletion();
        private TaskCompletionSource<StopMode>? _stop;
        private FakeManagedProcess? _process;

        public bool BlockStop { get; init; }
        public int LaunchCount { get; private set; }
        public int StopCount { get; private set; }

        public void CompleteLaunch()
        {
            _process = new FakeManagedProcess(LaunchCount);
            _launch.SetResult(new(_process, $"test:{LaunchCount}", DateTimeOffset.UtcNow));
        }

        public void CompleteStop()
        {
            _process?.Exit(0);
            _process = null;
            _stop?.SetResult(StopMode.Graceful);
        }

        public Task WriteOptionsAsync(string options, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<RunningGame> LaunchVanillaAsync(string version, IReadOnlyList<string> arguments, CancellationToken cancellationToken) => BeginLaunch(cancellationToken);

        public Task<RunningGame> LaunchNeoForgeAsync(IReadOnlyList<string> arguments, CancellationToken cancellationToken) => BeginLaunch(cancellationToken);

        public Task<RunningGame> LaunchCurseForgeAsync(string slug, int fileId, IReadOnlyList<string> arguments, CancellationToken cancellationToken) => BeginLaunch(cancellationToken);

        public Task ConnectAsync(string host, int port, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task SendChatAsync(string message, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<byte[]> CaptureScreenshotAsync(CancellationToken cancellationToken) => Task.FromResult(Array.Empty<byte>());

        public Task<StopMode> StopAsync(RunningGame? game, CancellationToken cancellationToken)
        {
            StopCount++;

            if (BlockStop && game is not null)
            {
                _stop = new(TaskCreationOptions.RunContinuationsAsynchronously);
                return _stop.Task;
            }

            _process?.Exit(0);
            _process = null;
            return Task.FromResult(game is null ? StopMode.AlreadyStopped : StopMode.Graceful);
        }

        private Task<RunningGame> BeginLaunch(CancellationToken cancellationToken)
        {
            LaunchCount++;
            _launch = CreateLaunchCompletion();
            cancellationToken.Register(() => _launch.TrySetCanceled(cancellationToken));
            return _launch.Task;
        }

        private static TaskCompletionSource<RunningGame> CreateLaunchCompletion()
        {
            return new(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }

    private sealed class FakeManagedProcess(int id) : IManagedProcess
    {
        private readonly TaskCompletionSource _exit = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public int Id { get; } = id;
        public bool HasExited { get; private set; }
        public int? ExitCode { get; private set; }

        public Task WaitForExitAsync(CancellationToken cancellationToken) => _exit.Task.WaitAsync(cancellationToken);

        public void KillTree() => Exit(137);

        public void Exit(int exitCode)
        {
            HasExited = true;
            ExitCode = exitCode;
            _exit.TrySetResult();
        }

        public void Dispose()
        {
        }
    }
}
