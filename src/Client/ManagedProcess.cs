using System.Diagnostics;

namespace Void.Client;

/// <summary>Adapts <see cref="Process"/> into the lifecycle surface owned by the coordinator.</summary>
internal sealed class ManagedProcess(Process process, int? memoryMb, long? initialOutOfMemoryKillCount) : IManagedProcess
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

    public Task WaitForExitAsync(CancellationToken cancellationToken)
    {
        return process.WaitForExitAsync(cancellationToken);
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
