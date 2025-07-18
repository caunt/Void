using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Void.Tests.Integration;

public class DirectConnectionTests : ConnectionTestBase
{
    public DirectConnectionTests() : base(new PaperServer(), new MinecraftConsoleClient("hello void!"))
    {
    }

    [Fact]
    public async Task MccConnectsToPaperServer()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        await RunAsync(cancellationTokenSource.Token);
    }
}
