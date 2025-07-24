namespace Void.Tests.Integration.Connections;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

public abstract class ConnectionFixtureBase : IDisposable
{
    protected readonly string _workingDirectory;
    protected readonly HttpClient _httpClient;

    public TimeSpan Timeout { get; } = TimeSpan.FromMinutes(3);

    public ConnectionFixtureBase(string workingDirectory)
    {
        _httpClient = new();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Void.Tests/1.0");
        _httpClient.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");
        _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

        _workingDirectory = Path.Combine(Path.GetTempPath(), $"{nameof(Tests)}", "IntegrationTests", workingDirectory);

        if (!Directory.Exists(_workingDirectory))
            Directory.CreateDirectory(_workingDirectory);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (Directory.Exists(_workingDirectory))
            Directory.Delete(_workingDirectory, true);

        _httpClient.Dispose();
    }
}
