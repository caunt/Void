using System.Diagnostics;

namespace Void.Client;

/// <summary>Adapts <see cref="Process"/> into the lifecycle surface owned by the coordinator.</summary>
internal sealed class ManagedProcess(Process process, int? memoryMb, long? initialOutOfMemoryKillCount, Task? outputCompletion = null) : IManagedProcess
{
    private bool? _wasOutOfMemoryKilled;

    public int Id => process.Id;

    public bool HasExited => process.HasExited;

    public int? ExitCode => process.HasExited ? process.ExitCode : null;

    public int? MemoryMb { get; } = memoryMb;

    public bool WasOutOfMemoryKilled
    {
        get
        {
            if (!process.HasExited || process.ExitCode is not 137)
                return false;

            return _wasOutOfMemoryKilled ??= initialOutOfMemoryKillCount is { } initialCount
                                               && CgroupMemoryEvents.ReadOutOfMemoryKillCount() is { } currentCount
                                               && currentCount > initialCount;
        }
    }

    public async Task WaitForExitAsync(CancellationToken cancellationToken)
    {
        await process.WaitForExitAsync(cancellationToken);
        if (outputCompletion is not null)
            await outputCompletion.WaitAsync(cancellationToken);
    }

    public void KillTree()
    {
        if (!process.HasExited)
            process.Kill(entireProcessTree: true);
    }

    public void Dispose()
    {
        process.Dispose();
    }
}
