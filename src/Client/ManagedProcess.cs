using System.Diagnostics;

namespace Void.Client;

/// <summary>Adapts <see cref="Process"/> into the lifecycle surface owned by the coordinator.</summary>
internal sealed class ManagedProcess(Process process) : IManagedProcess
{
    public int Id => process.Id;

    public bool HasExited => process.HasExited;

    public int? ExitCode => process.HasExited ? process.ExitCode : null;

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
