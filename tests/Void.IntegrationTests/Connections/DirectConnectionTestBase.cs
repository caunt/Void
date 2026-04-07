using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Fixtures;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Connections;

public abstract class DirectConnectionTestBase(ServerClientFixture fixture) : IntegrationUnitBase, IClassFixture<ServerClientFixture>
{
    private const string ExpectedText = "hello void!";

    private readonly EndPoint _serverEndPoint = new IPEndPoint(IPAddress.Loopback, fixture.PaperServer.Port);

    protected async Task RunAsync(ProtocolVersion protocolVersion)
    {
        if (!fixture.PortableMinecraftClient.SupportedVersions.Contains(protocolVersion))
            Assert.Skip($"Protocol version {protocolVersion} is not supported by the client, skipping test.");

        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";

        await LoggedExecutorAsync(async () =>
        {
            using (var gameCancellationTokenSource = new CancellationTokenSource(StepTimeout * 3)) // Game should run enough time for all steps below
            {
                await using var game = await WithTimeoutRetriesAsync(async () => await fixture.PortableMinecraftClient.RunGameAsync(_serverEndPoint, protocolVersion, gameCancellationTokenSource.Token), maxRetries: 5);

                await fixture.PortableMinecraftClient.SendTextMessageAsync(expectedText, StepTimeoutToken);
                await fixture.PaperServer.ExpectTextAsync(expectedText, lookupHistory: true, StepTimeoutToken);
            }

            Assert.Contains(fixture.PaperServer.Logs, line => line.Contains(expectedText));
        }, fixture.PortableMinecraftClient, fixture.PaperServer);
    }
}
