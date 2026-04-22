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

public abstract class DirectConnectionTestBase(PaperFixture paperFixture, PortableMinecraftClientFixture portableMinecraftClientFixture) : IntegrationUnitBase, IClassFixture<PortableMinecraftClientFixture>
{
    private const string ExpectedText = "hello void!";

    private readonly EndPoint _serverEndPoint = new IPEndPoint(IPAddress.Loopback, paperFixture.Server1.Port);

    protected async Task RunAsync(ProtocolVersion protocolVersion)
    {
        if (!portableMinecraftClientFixture.Api.SupportedVersions.Contains(protocolVersion))
            Assert.Skip($"Protocol version {protocolVersion} is not supported by the client, skipping test.");

        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";

        await LoggedExecutorAsync(async () =>
        {
            await using (var game = await portableMinecraftClientFixture.Api.RunGameAsync(_serverEndPoint, protocolVersion, Timeouts.SetupTimeoutToken))
            {
                await game.SendTextMessageAsync(expectedText, Timeouts.StepTimeoutToken);
                await paperFixture.Server1.ExpectTextAsync(expectedText, lookupHistory: true, Timeouts.StepTimeoutToken);
            }

            Assert.Contains(paperFixture.Server1.Logs, line => line.Contains(expectedText));
        }, portableMinecraftClientFixture.Api, paperFixture.Server1);
    }
}
