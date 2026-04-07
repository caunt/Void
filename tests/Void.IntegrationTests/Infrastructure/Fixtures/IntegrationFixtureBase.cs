using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public abstract class IntegrationFixtureBase : IDisposable
{
    protected readonly HttpClient _httpClient;

    public TimeSpan SetupTimeout { get; } = TimeSpan.FromMinutes(10);

    public IntegrationFixtureBase()
    {
        _httpClient = new();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Void.Tests/1.0 (https://github.com/caunt/Void)");
        _httpClient.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");
        _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _httpClient.Dispose();
    }
}
