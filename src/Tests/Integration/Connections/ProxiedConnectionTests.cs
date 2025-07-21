using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Proxies;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections;

public class ProxiedConnectionTests : ConnectionTestBase
{
    private const string ExpectedText = "hello void!";

    [Fact]
    public async Task MccConnectsToPaperServerThroughProxy()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(3));

        await using var paper = new PaperServer(ExpectedText);
        await using var proxy = new ProxyPlatform("localhost:25565", 25566, true);
        await using var mcc = new MinecraftConsoleClient(ExpectedText, "localhost:25566");

        var proxyTask = proxy.RunAsync(cancellationTokenSource.Token);
        await ExecuteAsync(paper, mcc, cancellationTokenSource.Token);
        cancellationTokenSource.Cancel();
        await proxyTask;

        Assert.Contains(paper.Logs, line => line.Contains(ExpectedText));
    }
}
