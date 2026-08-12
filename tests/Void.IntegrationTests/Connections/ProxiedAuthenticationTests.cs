using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Extensions;
using Void.IntegrationTests.Infrastructure.Fixtures;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Connections;

public class ProxiedAuthenticationTests(ProxiedAuthenticationFixture authenticationFixture, PortableMinecraftClientFixture portableMinecraftClientFixture) : IntegrationUnitBase, IClassFixture<ProxiedAuthenticationFixture>, IClassFixture<PortableMinecraftClientFixture>
{
    private const string ExpectedKickReason = "The server is full!";
    private const string Username = "Shonz1";
    private static readonly ProtocolVersion TestProtocolVersion = ProtocolVersion.MINECRAFT_1_21_4;
    private readonly EndPoint _proxyEndPoint = new IPEndPoint(IPAddress.Loopback, authenticationFixture.VoidProxy.Port);

    [Fact]
    public async Task PaperServerFullKickIsRelayedToPlayerAsync()
    {
        if (!portableMinecraftClientFixture.Api.SupportedVersions.Contains(TestProtocolVersion))
            Assert.Skip($"Protocol version {TestProtocolVersion} is not supported by the client, skipping test.");

        await LoggedExecutorAsync(async () =>
        {
            var voidLogWindowStartedAt = DateTime.UtcNow;

            try
            {
                await using var game = await portableMinecraftClientFixture.Api.RunGameAsync(nameof(ProxiedAuthenticationTests), TestProtocolVersion, Username, [authenticationFixture.VoidProxy, authenticationFixture.PaperServer], Timeouts.SetupTimeoutToken);
                var authenticationFailureTask = authenticationFixture.VoidProxy.LogWriter.WaitForLineAsync(
                    line => line.Contains($"Player {Username} cannot authenticate on args-server-1: {{text:\"{ExpectedKickReason}\"}}", StringComparison.Ordinal),
                    Timeouts.SetupTimeoutToken);
                var playerDisconnectionTask = authenticationFixture.VoidProxy.LogWriter.WaitForLineAsync(
                    line => line.Contains($"Player {Username} disconnected", StringComparison.Ordinal),
                    Timeouts.SetupTimeoutToken);
                var paperRejectionTask = authenticationFixture.PaperServer.Container.ExpectTextAsync(ExpectedKickReason, game.StartedAt, Timeouts.SetupTimeoutToken);

                await game.JoinServerExpectingFailureAsync(_proxyEndPoint, Timeouts.SetupTimeoutToken);

                await authenticationFailureTask;
                await paperRejectionTask;
                await playerDisconnectionTask;
            }
            finally
            {
                authenticationFixture.VoidProxy.AssertNoWarningOrHigherLogsSince(voidLogWindowStartedAt);
            }
        }, portableMinecraftClientFixture.Api, authenticationFixture.VoidProxy, authenticationFixture.PaperServer);
    }
}
