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

public abstract class ProxiedConnectionTestBase(PaperFixture paperFixture, VoidFixture voidFixture, PortableMinecraftClientFixture portableMinecraftClientFixture) : IntegrationUnitBase, IClassFixture<VoidFixture>, IClassFixture<PortableMinecraftClientFixture>
{
    private const string ExpectedText = "hello proxied void!";

    private readonly EndPoint _proxyEndPoint = new IPEndPoint(IPAddress.Loopback, voidFixture.VoidProxy.Port);

    protected async Task RunAsync(ProtocolVersion protocolVersion)
    {
        if (!portableMinecraftClientFixture.Api.SupportedVersions.Contains(protocolVersion))
            Assert.Skip($"Protocol version {protocolVersion} is not supported by the client, skipping test.");

        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";

        await LoggedExecutorAsync(async () =>
        {
            await using (var game = await portableMinecraftClientFixture.Api.RunGameAsync(nameof(ProxiedConnectionTestBase), _proxyEndPoint, protocolVersion, Timeouts.SetupTimeoutToken))
            {
                await game.SendTextMessageAsync(expectedText, Timeouts.StepTimeoutToken);
                await paperFixture.Server1.ExpectTextAsync(expectedText, lookupHistory: true, Timeouts.StepTimeoutToken);
            }

            Assert.Contains(paperFixture.Server1.Logs, line => line.Contains(expectedText));
        }, portableMinecraftClientFixture.Api, voidFixture.VoidProxy, paperFixture.Server1);
    }
}
