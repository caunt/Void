using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Servers;
using Xunit;

namespace Void.Tests.Integration.Connections;

public class DirectConnectionTests(DirectConnectionTests.PaperMccFixture fixture) : IClassFixture<DirectConnectionTests.PaperMccFixture>
{
    private const string ExpectedText = "hello void!";

    [Fact]
    public async Task MccConnectsToPaperServer()
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        await fixture.Client.SendTextMessageAsync("localhost:25565", ProtocolVersion.MINECRAFT_1_20_3, expectedText, cancellationTokenSource.Token);
        await fixture.Server.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

        Assert.Contains(fixture.Server.Logs, line => line.Contains(expectedText));
    }

    [Theory]
    [MemberData(nameof(MinecraftConsoleClient.SupportedVersions), MemberType = typeof(MinecraftConsoleClient))]
    public async Task MccConnectsToPaperServer_WithProtocolVersion(ProtocolVersion protocolVersion)
    {
        var expectedText = $"{ExpectedText} test #{Random.Shared.Next()}";
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        await fixture.Client.SendTextMessageAsync("localhost:25565", protocolVersion, expectedText, cancellationTokenSource.Token);
        await fixture.Server.ExpectTextAsync(expectedText, lookupHistory: true, cancellationTokenSource.Token);

        Assert.Contains(fixture.Server.Logs, line => line.Contains(expectedText));
    }

    public class PaperMccFixture : ConnectionTestBase, IAsyncLifetime
    {
        public PaperMccFixture() : base(nameof(DirectConnectionTests))
        {
        }

        public PaperServer Server { get; private set; } = null!;
        public MinecraftConsoleClient Client { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(3));

            Server = await PaperServer.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
            Client = await MinecraftConsoleClient.CreateAsync(_workingDirectory, _httpClient, cancellationToken: cancellationTokenSource.Token);
        }

        public async Task DisposeAsync()
        {
            if (Client is not null)
                await Client.DisposeAsync();

            if (Server is not null)
                await Server.DisposeAsync();
        }
    }
}
