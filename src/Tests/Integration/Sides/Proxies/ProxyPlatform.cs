namespace Void.Tests.Integration.Sides.Proxies;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Void.Proxy;
using Void.Tests.Integration.Sides;
using Void.Tests.Streams;

public class ProxyPlatform(string serverAddress, int port, bool ignoreFileServers = true, bool offlineMode = true) : IIntegrationSide
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly CollectingTextWriter _logs = new();

    public IEnumerable<string> Logs => _logs.Lines;

    public Task SetupAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, cancellationToken).Token;

        var args = new List<string>
        {
            "--server", serverAddress,
            "--port", port.ToString()
        };

        if (ignoreFileServers)
            args.Add("--ignore-file-servers");

        if (ignoreFileServers)
            args.Add("--offline");

        await EntryPoint.RunAsync(logWriter: _logs, cancellationToken: cancellationToken, args: [.. args]);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await cancellationTokenSource.CancelAsync();
    }
}
