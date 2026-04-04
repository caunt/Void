using System;
using System.Linq;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Exceptions;

namespace Void.IntegrationTests.Infrastructure.Harness;

public class IntegrationUnitBase
{
    public TimeSpan TestTimeout { get; } = TimeSpan.FromSeconds(120);

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
            Console.WriteLine(CollectLogs());

            foreach (var side in sides)
                side.ClearLogs();
        }

        return;

        string CollectLogs() => $"Logs:\n\n\n{string.Join("\n\n\n", sides.Select(side => $"{side} logs:\n{string.Join("\n", side.Logs)}"))}";
    }
}
