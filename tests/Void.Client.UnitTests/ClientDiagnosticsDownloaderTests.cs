using System.Net;
using System.Net.Http.Headers;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.Client.UnitTests;

public class ClientDiagnosticsDownloaderTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"void-download-tests-{Guid.NewGuid()}");

    [Fact]
    public async Task SavesOnlyRequestedSessionWithoutLeavingTemporaryFile()
    {
        var identifier = Guid.NewGuid();
        using var handler = new FakeHandler((request, _) =>
        {
            Assert.Equal($"/api/game/diagnostics/{identifier}", request.RequestUri?.AbsolutePath);
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent([80, 75, 3, 4]) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            return Task.FromResult(response);
        });
        using var client = new HttpClient(handler) { BaseAddress = new Uri("http://client") };
        await ClientDiagnosticsDownloader.DownloadAsync(client, identifier, _directory);
        Assert.Equal(new byte[] { 80, 75, 3, 4 }, await File.ReadAllBytesAsync(Path.Combine(_directory, $"client-diagnostics-{identifier}.zip"), TestContext.Current.CancellationToken));
        Assert.Single(Directory.EnumerateFiles(_directory));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task UnsupportedEndpointOrMissingSessionLeavesAnErrorNote(bool missingIdentifier)
    {
        using var handler = new FakeHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));
        using var client = new HttpClient(handler) { BaseAddress = new Uri("http://client") };
        await ClientDiagnosticsDownloader.DownloadAsync(client, missingIdentifier ? null : Guid.NewGuid(), _directory);
        Assert.True(File.Exists(Path.Combine(_directory, "client-diagnostics-error.txt")));
        Assert.Empty(Directory.EnumerateFiles(_directory, "*.zip"));
        Assert.Empty(Directory.EnumerateFiles(_directory, "*.tmp"));
    }

    [Fact]
    public async Task TimeoutIsReportedWithoutThrowingFromCleanup()
    {
        using var handler = new FakeHandler(async (_, cancellationToken) =>
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        using var client = new HttpClient(handler) { BaseAddress = new Uri("http://client") };
        await ClientDiagnosticsDownloader.DownloadAsync(client, Guid.NewGuid(), _directory, TimeSpan.FromMilliseconds(50));
        Assert.True(File.Exists(Path.Combine(_directory, "client-diagnostics-error.txt")));
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
        GC.SuppressFinalize(this);
    }

    private sealed class FakeHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => respond(request, cancellationToken);
    }
}
