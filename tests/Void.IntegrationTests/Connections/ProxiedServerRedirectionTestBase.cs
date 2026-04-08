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

public abstract class ProxiedServerRedirectionTestBase(PaperFixture paperFixture, VoidFixture voidFixture, PortableMinecraftClientFixture portableMinecraftClientFixture) : IntegrationUnitBase, IClassFixture<VoidFixture>, IClassFixture<PortableMinecraftClientFixture>
{
    private readonly EndPoint _proxyEndPoint = new IPEndPoint(IPAddress.Loopback, voidFixture.VoidProxy.Port);

    protected async Task RunAsync(ProtocolVersion protocolVersion)
    {
        if (!portableMinecraftClientFixture.PortableMinecraftClient.SupportedVersions.Contains(protocolVersion))
            Assert.Skip($"Protocol version {protocolVersion} is not supported by the client, skipping test.");

        var server1First = $"server1-{Guid.NewGuid()}";
        var server2Text = $"server2-{Guid.NewGuid()}";

        await LoggedExecutorAsync(async () =>
        {
            using (var gameCancellationTokenSource = new CancellationTokenSource(Timeouts.StepTimeout * 5)) // Game should run enough time for all steps below
            {
                await using var game = await WithTimeoutRetriesAsync(async () => await portableMinecraftClientFixture.PortableMinecraftClient.RunGameAsync(_proxyEndPoint, protocolVersion, gameCancellationTokenSource.Token), maxRetries: 5);

                await portableMinecraftClientFixture.PortableMinecraftClient.SendTextMessagesAsync(
                [
                    server1First,
                    "/server args-server-2"
                ], Timeouts.StepTimeoutToken);

                await portableMinecraftClientFixture.PortableMinecraftClient.EnsureStableAsync(Timeouts.StepTimeoutToken);

                await portableMinecraftClientFixture.PortableMinecraftClient.SendTextMessagesAsync(
                [
                    server2Text,
                    "/server args-server-1"
                ], Timeouts.StepTimeoutToken);

                await portableMinecraftClientFixture.PortableMinecraftClient.EnsureStableAsync(Timeouts.StepTimeoutToken);
            }

            Assert.Contains(voidFixture.VoidProxy.Logs, line => line.Contains("connected to args-server-2"));
            Assert.True(voidFixture.VoidProxy.Logs.Count(line => line.Contains("connected to args-server-1")) is >= 2); // TODO: sometimes, proxy prints multiple times "connected to" message
        }, portableMinecraftClientFixture.PortableMinecraftClient, voidFixture.VoidProxy, paperFixture.PaperServer1, paperFixture.PaperServer2);
    }
}
