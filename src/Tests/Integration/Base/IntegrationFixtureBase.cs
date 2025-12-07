namespace Void.Tests.Integration.Base;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

public abstract class IntegrationFixtureBase : IDisposable
{
    protected readonly string _workingDirectory;
    protected readonly HttpClient _httpClient;

    public TimeSpan Timeout { get; } = TimeSpan.FromMinutes(10);

    public IntegrationFixtureBase(string workingDirectory)
    {
        _httpClient = new();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Void.Tests/1.0 (https://github.com/caunt/Void)");
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
