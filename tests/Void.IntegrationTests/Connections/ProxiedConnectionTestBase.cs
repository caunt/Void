using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Fixtures;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Connections;

public abstract class ProxiedConnectionTestBase(ServerProxyClientFixture fixture) : IntegrationUnitBase, IClassFixture<ServerProxyClientFixture>
{
    private const string ExpectedText = "hello proxied void!";

    private readonly EndPoint _proxyEndPoint = new IPEndPoint(IPAddress.Loopback, fixture.VoidProxy.Port);

    protected async Task RunAsync(ProtocolVersion protocolVersion)
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";

        await LoggedExecutorAsync(async () =>
        {
            using (var gameCancellationTokenSource = new CancellationTokenSource(StepTimeout * 3)) // Game should run enough time for all steps below
            {
                await using var game = await WithTimeoutRetriesAsync(async () => await fixture.PortableMinecraftClient.RunGameAsync(_proxyEndPoint, protocolVersion, gameCancellationTokenSource.Token), maxRetries: 5);

                await fixture.PortableMinecraftClient.SendTextMessageAsync(expectedText, StepTimeoutToken);
                await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, StepTimeoutToken);
            }

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.PortableMinecraftClient, fixture.VoidProxy, fixture.PaperServer);
    }
}
