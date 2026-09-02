using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Void.IntegrationTests.Infrastructure.Exceptions;
using Void.IntegrationTests.Infrastructure.Harness.Sides;

namespace Void.IntegrationTests.Infrastructure.Harness;

public class IntegrationUnitBase
{
    public static Task LoggedExecutorAsync(Func<Task> function, params IIntegrationSide[] sides)
    {
        return LoggedExecutorAsync(function, LogLevel.Warning, sides);
    }

    public static async Task LoggedExecutorAsync(Func<Task> function, LogLevel minimumFailureLogLevel, params IIntegrationSide[] sides)
    {
        var voidLogWindowStartedAt = DateTime.UtcNow;
        var voidProxies = sides.OfType<VoidProxy>().Distinct().ToArray();

        try
        {
            await function();

            foreach (var voidProxy in voidProxies)
                voidProxy.AssertNoLogsAtOrAboveSince(voidLogWindowStartedAt, minimumFailureLogLevel);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Primary integration failure:\n{exception}");
            Console.WriteLine(CollectLogs());
            throw new IntegrationTestException($"Test execution failed: {exception.Message}", exception);
        }
        finally
        {
            foreach (var voidProxy in voidProxies)
                voidProxy.ClearLogs();
        }

        return;

        string CollectLogs() => $"Logs:\n\n\n{string.Join("\n\n\n", sides.Select(side => $"{side} logs:\n{string.Join("\n", side.Logs)}"))}";
    }
}
