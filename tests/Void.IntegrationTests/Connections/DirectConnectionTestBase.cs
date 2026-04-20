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

public abstract class DirectConnectionTestBase(PaperFixture paperFixture, PortableMinecraftClientFixture portableMinecraftClientFixture) : IntegrationUnitBase, IClassFixture<PortableMinecraftClientFixture>, IAsyncLifetime
{
    private const string ExpectedText = "hello void!";

    private EndPoint ServerEndPoint { get => field ?? throw new InvalidOperationException($"{nameof(ServerEndPoint)} is not initialized."); set; }

    public async ValueTask InitializeAsync()
    {
        await paperFixture.EnsureInitializedAsync();
        ServerEndPoint = new IPEndPoint(IPAddress.Loopback, paperFixture.PaperServer1.Port);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    protected async Task RunAsync(ProtocolVersion protocolVersion)
    {
        if (!portableMinecraftClientFixture.PortableMinecraftClient.SupportedVersions.Contains(protocolVersion))
            Assert.Skip($"Protocol version {protocolVersion} is not supported by the client, skipping test.");

        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";

        await LoggedExecutorAsync(async () =>
        {
            using (var gameCancellationTokenSource = new CancellationTokenSource(Timeouts.StepTimeout * 3)) // Game should run enough time for all steps below
            {
                await using var game = await WithTimeoutRetriesAsync(async () => await portableMinecraftClientFixture.PortableMinecraftClient.RunGameAsync(ServerEndPoint, protocolVersion, gameCancellationTokenSource.Token), maxRetries: 5);

                await portableMinecraftClientFixture.PortableMinecraftClient.SendTextMessageAsync(expectedText, Timeouts.StepTimeoutToken);
                await paperFixture.PaperServer1.ExpectTextAsync(expectedText, lookupHistory: true, Timeouts.StepTimeoutToken);
            }

            Assert.Contains(paperFixture.PaperServer1.Logs, line => line.Contains(expectedText));
        }, portableMinecraftClientFixture.PortableMinecraftClient, paperFixture.PaperServer1);
    }
}
