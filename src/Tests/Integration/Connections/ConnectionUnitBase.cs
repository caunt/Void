using System;
using System.Linq;
using System.Threading.Tasks;
using Void.Tests.Exceptions;
using Void.Tests.Integration.Sides;

namespace Void.Tests.Integration.Connections;

public class ConnectionUnitBase
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
            throw new IntegrationTestException($"{string.Join("\n\n\n", sides.Select(side => $"{side} logs:\n{string.Join("\n", side.Logs)}"))}", exception);
        }
    }
}
