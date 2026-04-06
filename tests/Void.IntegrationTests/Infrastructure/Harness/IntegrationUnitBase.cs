using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Exceptions;

namespace Void.IntegrationTests.Infrastructure.Harness;

public class IntegrationUnitBase
{
    public TimeSpan StepTimeout { get; } = TimeSpan.FromSeconds(60);
    public CancellationToken StepTimeoutToken => new CancellationTokenSource(StepTimeout).Token;

    public static async Task LoggedExecutorAsync(Func<Task> function, params IIntegrationSide[] sides)
    {
        try
        {
            await function();
        }
        catch (Exception exception)
        {
            throw new IntegrationTestException($"{exception}\n{CollectLogs()}");
        }
        finally
        {
            foreach (var side in sides)
                side.ClearLogs();
        }

        return;

        string CollectLogs() => $"Logs:\n\n\n{string.Join("\n\n\n", sides.Select(side => $"{side} logs:\n{string.Join("\n", side.Logs)}"))}";
    }
}
