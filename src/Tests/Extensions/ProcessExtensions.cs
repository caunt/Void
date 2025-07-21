using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Void.Tests.Extensions;

public static class ProcessExtensions
{
    public static async Task ExitAsync(this Process process, bool entireProcessTree = true)
    {
        ArgumentNullException.ThrowIfNull(process, nameof(process));

        if (process.HasExited)
            return;

        var taskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        process.EnableRaisingEvents = true;
        process.Exited += (sender, eventArgs) => taskCompletionSource.TrySetResult();

        process.Kill(entireProcessTree);

        await taskCompletionSource.Task;
    }
}
