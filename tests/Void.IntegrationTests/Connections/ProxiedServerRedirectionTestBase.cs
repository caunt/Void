using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Extensions;
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
        if (!portableMinecraftClientFixture.Api.SupportedVersions.Contains(protocolVersion))
            Assert.Skip($"Protocol version {protocolVersion} is not supported by the client, skipping test.");

        var firstMessage = $"server1-{Guid.NewGuid()}";
        var secondMessage = $"server2-{Guid.NewGuid()}";
        var thirdMessage = $"server1-{Guid.NewGuid()}";

        await LoggedExecutorAsync(async () =>
        {
            DateTime voidLogWindowStartedAt;
            Task playerDisconnectionTask;

            await using (var game = await portableMinecraftClientFixture.Api.RunGameAsync(nameof(ProxiedServerRedirectionTestBase), protocolVersion, [voidFixture.VoidProxy, paperFixture.Server1, paperFixture.Server2], Timeouts.SetupTimeoutToken))
            {
                voidLogWindowStartedAt = DateTime.UtcNow;
                playerDisconnectionTask = voidFixture.VoidProxy.WaitForPlayerDisconnectionAsync(game.Username, Timeouts.StepTimeoutToken);

                await game.JoinServerAsync(_proxyEndPoint, Timeouts.SetupTimeoutToken);
                await paperFixture.Server1.Container.ExpectTextAsync($"{game.Username} joined the game", game.StartedAt, Timeouts.SetupTimeoutToken);
                await game.EnsureStableAsync(Timeouts.SetupTimeoutToken);

                await game.SendTextMessageAsync(firstMessage, Timeouts.StepTimeoutToken);
                await paperFixture.Server1.ExpectTextAsync(firstMessage, lookupHistory: true, Timeouts.StepTimeoutToken);
                
                var secondServerReadyTask = voidFixture.VoidProxy.WaitForServerConnectionAndKeepAliveAsync(game.Username, "args-server-2", Timeouts.StepTimeoutToken);
                await game.SendTextMessageAsync("/server args-server-2", Timeouts.StepTimeoutToken);
                await secondServerReadyTask;
                
                await game.SendTextMessageAsync(secondMessage, Timeouts.StepTimeoutToken);
                await paperFixture.Server2.ExpectTextAsync(secondMessage, lookupHistory: true, Timeouts.StepTimeoutToken);
                
                var firstServerReadyTask = voidFixture.VoidProxy.WaitForServerConnectionAndKeepAliveAsync(game.Username, "args-server-1", Timeouts.StepTimeoutToken);
                await game.SendTextMessageAsync("/server args-server-1", Timeouts.StepTimeoutToken);
                await firstServerReadyTask;
                
                await game.SendTextMessageAsync(thirdMessage, Timeouts.StepTimeoutToken);
                await paperFixture.Server1.ExpectTextAsync(thirdMessage, lookupHistory: true, Timeouts.StepTimeoutToken);
            }

            await playerDisconnectionTask;

            Assert.Contains(paperFixture.Server1.Logs, line => line.Contains(firstMessage));
            Assert.Contains(paperFixture.Server2.Logs, line => line.Contains(secondMessage));
            Assert.Contains(paperFixture.Server1.Logs, line => line.Contains(thirdMessage));
            
            Assert.Contains(voidFixture.VoidProxy.Logs, line => line.Contains("connected to args-server-2"));
            Assert.True(voidFixture.VoidProxy.Logs.Count(line => line.Contains("connected to args-server-1")) is >= 2); // TODO: sometimes, proxy prints multiple times "connected to" message
            voidFixture.VoidProxy.AssertNoWarningOrHigherLogsSince(voidLogWindowStartedAt);
        }, portableMinecraftClientFixture.Api, voidFixture.VoidProxy, paperFixture.Server1, paperFixture.Server2);
    }
}
