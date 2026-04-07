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

public abstract class ProxiedServerRedirectionTestBase(TwoServersProxyClientFixture fixture) : IntegrationUnitBase, IClassFixture<TwoServersProxyClientFixture>
{
    private readonly EndPoint _proxyEndPoint = new IPEndPoint(IPAddress.Loopback, fixture.VoidProxy.Port);

    protected async Task RunAsync(ProtocolVersion protocolVersion)
    {
        var server1First = $"server1-{Guid.NewGuid()}";
        var server2Text = $"server2-{Guid.NewGuid()}";

        await LoggedExecutorAsync(async () =>
        {
            using (var gameCancellationTokenSource = new CancellationTokenSource(StepTimeout * 5)) // Game should run enough time for all steps below
            {
                await using var game = await WithTimeoutRetriesAsync(async () => await fixture.PortableMinecraftClient.RunGameAsync(_proxyEndPoint, protocolVersion, gameCancellationTokenSource.Token), maxRetries: 5);

                await fixture.PortableMinecraftClient.SendTextMessagesAsync(
                [
                    server1First,
                    "/server args-server-2"
                ], StepTimeoutToken);

                await fixture.PortableMinecraftClient.EnsureStableAsync(StepTimeoutToken);

                await fixture.PortableMinecraftClient.SendTextMessagesAsync(
                [
                    server2Text,
                    "/server args-server-1"
                ], StepTimeoutToken);

                await fixture.PortableMinecraftClient.EnsureStableAsync(StepTimeoutToken);
            }

            Assert.Contains(fixture.VoidProxy.Logs, line => line.Contains("connected to args-server-2"));
            Assert.True(fixture.VoidProxy.Logs.Count(line => line.Contains("connected to args-server-1")) is >= 2); // TODO: sometimes, proxy prints multiple times "connected to" message
        }, fixture.PortableMinecraftClient, fixture.VoidProxy, fixture.PaperServer1, fixture.PaperServer2);
    }
}
