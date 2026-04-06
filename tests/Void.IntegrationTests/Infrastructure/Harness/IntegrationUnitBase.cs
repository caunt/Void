using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Exceptions;

namespace Void.IntegrationTests.Infrastructure.Harness;

public class IntegrationUnitBase
{
    public TimeSpan StepTimeout { get; } = TimeSpan.FromSeconds(90);
    public CancellationToken StepTimeoutToken => new CancellationTokenSource(StepTimeout).Token;

    public static async Task LoggedExecutorAsync(Func<Task> function, params IIntegrationSide[] sides)
    {
        try
        {
            await function();
        }
        catch (Exception exception)
        {
            Console.WriteLine(CollectLogs());
            throw new IntegrationTestException("Test execution failed: ", exception);
        }
        finally
        {
            foreach (var side in sides)
                side.ClearLogs();
        }

        return;

        string CollectLogs() => $"Logs:\n\n\n{string.Join("\n\n\n", sides.Select(side => $"{side} logs:\n{string.Join("\n", side.Logs)}"))}";
    }

    public static async Task<T> WithTimeoutRetriesAsync<T>(Func<Task<T>> function, int maxRetries)
    {
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await function();
            }
            catch (OperationCanceledException) when (attempt < maxRetries)
            {
                // Ignored
            }
        }

        throw new TimeoutException($"Operation did not complete within the allotted time after {maxRetries} attempts.");
    }
}
