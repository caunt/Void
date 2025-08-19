namespace Void.Tests.Integration.Sides.Proxies;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Void.Proxy;
using Void.Tests.Streams;
using Xunit;

public class VoidProxy : IIntegrationSide
{
    private readonly CollectingTextWriter _logWriter;
    private readonly Task _task;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public IEnumerable<string> Logs => _logWriter.Lines;

    public void ClearLogs()
    {
        _logWriter.Clear();
    }

    private VoidProxy(CollectingTextWriter logWriter, Task task, CancellationTokenSource cancellationTokenSource)
    {
        _logWriter = logWriter;
        _task = task;
        _cancellationTokenSource = cancellationTokenSource;
    }

    public static Task<VoidProxy> CreateAsync(string workingDirectory, string targetServer, int proxyPort, bool ignoreFileServers = true, bool offlineMode = true, CancellationToken cancellationToken = default)
    {
        return CreateAsync(workingDirectory, [targetServer], proxyPort, ignoreFileServers, offlineMode, cancellationToken);
    }

    public static async Task<VoidProxy> CreateAsync(string workingDirectory, IEnumerable<string> targetServers, int proxyPort, bool ignoreFileServers = true, bool offlineMode = true, CancellationToken cancellationToken = default)
    {
        var logWriter = new CollectingTextWriter();
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        workingDirectory = Path.Combine(workingDirectory, nameof(VoidProxy));
        cancellationToken = cancellationTokenSource.Token;

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        var args = new List<string>
        {
            "--port", proxyPort.ToString(),
            "--logging", "Trace"
        };

        foreach (var targetServer in targetServers)
        {
            args.Add("--server");
            args.Add(targetServer);
        }

        if (ignoreFileServers)
            args.Add("--ignore-file-servers");

        if (offlineMode)
            args.Add("--offline");

        var task = EntryPoint.RunAsync(new EntryPoint.RunOptions { WorkingDirectory = workingDirectory, Arguments = [.. args], LogWriter = logWriter }, cancellationTokenSource.Token);

        // Wait for the proxy to start, because it takes some time to listen on the port

        try
        {
            while (logWriter.Lines.All(line => !line.Contains("Proxy started")))
                await Task.Delay(1_000, cancellationToken);
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidProxy)} failed to start. Logs:\n{logWriter.Text}\n{exception}");
        }

        return new VoidProxy(logWriter, task, cancellationTokenSource);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await _cancellationTokenSource.CancelAsync();
        await _task;
    }
}
