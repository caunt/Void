using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections;

public class DirectConnectionTests : ConnectionTestBase
{
    private const string ExpectedText = "hello void!";

    [Fact]
    public async Task MccConnectsToPaperServer()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(3));

        await using var paper = new PaperServer(ExpectedText);
        await using var mcc = new MinecraftConsoleClient(ExpectedText, "localhost:25565", ProtocolVersion.MINECRAFT_1_20_3);

        await ExecuteAsync(paper, mcc, cancellationTokenSource.Token);

        Assert.Contains(paper.Logs, line => line.Contains(ExpectedText));
    }
}
