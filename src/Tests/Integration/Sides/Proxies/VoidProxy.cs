namespace Void.Tests.Integration.Sides.Proxies;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Proxy;
using Void.Tests.Streams;

public class VoidProxy : IIntegrationSide
{
    private readonly CollectingTextWriter _logWriter;
    private readonly Task _task;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public IEnumerable<string> Logs => _logWriter.Lines;

    private VoidProxy(CollectingTextWriter logWriter, Task task, CancellationTokenSource cancellationTokenSource)
    {
        _logWriter = logWriter;
        _task = task;
        _cancellationTokenSource = cancellationTokenSource;
    }

    public static Task<VoidProxy> CreateAsync(string targetServer, int proxyPort, bool ignoreFileServers = true, bool offlineMode = true, CancellationToken cancellationToken = default)
    {
        var logWriter = new CollectingTextWriter();
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, cancellationToken).Token;

        var args = new List<string>
        {
            "--server", targetServer,
            "--port", proxyPort.ToString()
        };

        if (ignoreFileServers)
            args.Add("--ignore-file-servers");

        if (offlineMode)
            args.Add("--offline");

        return Task.FromResult(new VoidProxy(logWriter, EntryPoint.RunAsync(logWriter: logWriter, cancellationToken: cancellationToken, args: [.. args]), cancellationTokenSource));
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await _cancellationTokenSource.CancelAsync();
        await _task;
    }
}
