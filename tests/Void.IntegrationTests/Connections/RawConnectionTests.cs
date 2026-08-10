using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Connections;

public class RawConnectionTests : IntegrationUnitBase
{
    private const string ExpectedResponse = "Hello through Void!";

    [Fact]
    public async Task HttpClientConnectsToHttpServerThroughProxy()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Loopback, 0));

        await using var httpServer = builder.Build();
        httpServer.MapGet("/raw-proxy", () => ExpectedResponse);
        await httpServer.StartAsync(Timeouts.SetupTimeoutToken);

        var httpServerAddress = new Uri(Assert.Single(httpServer.Urls));

        await using var voidProxy = await VoidProxy.CreateAsync(
            Path.Combine(Path.GetTempPath(), nameof(RawConnectionTests), Path.GetRandomFileName()),
            httpServerAddress.Authority,
            cancellationToken: Timeouts.SetupTimeoutToken);

        await LoggedExecutorAsync(async () =>
        {
            var requestStartedAt = DateTime.UtcNow;
            var playerDisconnectedTask = voidProxy.LogWriter.WaitForLineAsync(
                line => line.Contains(" disconnected", StringComparison.Ordinal),
                Timeouts.StepTimeoutToken);

            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri($"http://{IPAddress.Loopback}:{voidProxy.Port}")
            };
            using var request = new HttpRequestMessage(HttpMethod.Get, "/raw-proxy");
            request.Headers.ConnectionClose = true;
            using var response = await httpClient.SendAsync(request, Timeouts.StepTimeoutToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(ExpectedResponse, await response.Content.ReadAsStringAsync(Timeouts.StepTimeoutToken));

            await playerDisconnectedTask;

            var requestLogs = voidProxy.LogWriter.GetLinesSince(requestStartedAt);
            Assert.Contains(requestLogs, line => line.Contains("Channel builder not found", StringComparison.Ordinal));
            Assert.DoesNotContain(requestLogs, line =>
                line.Contains(" ERR] ", StringComparison.Ordinal) ||
                line.Contains(" FTL] ", StringComparison.Ordinal));
        }, voidProxy);
    }
}
