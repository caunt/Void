using System;
using System.Linq;
using System.Threading.Tasks;
using Void.Tests.Exceptions;
using Void.Tests.Integration.Sides;

namespace Void.Tests.Integration.Base;

public class IntegrationUnitBase
{
    public TimeSpan Timeout { get; } = TimeSpan.FromSeconds(60);

    public static async Task LoggedExecutorAsync(Func<Task> function, params IIntegrationSide[] sides)
    {
        try
        {
            await function();
        }
        catch (Exception exception)
        {
            throw new IntegrationTestException($"{exception}\nLogs:\n\n\n{string.Join("\n\n\n", sides.Select(side => $"{side} logs:\n{string.Join("\n", side.Logs)}"))}");
        }
        finally
        {
            foreach (var side in sides)
                side.ClearLogs();
        }
    }
}
