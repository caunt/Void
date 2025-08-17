namespace Void.Tests.Integration.Sides.Proxies;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    private readonly string _originalWorkingDirectory;

    public IEnumerable<string> Logs => _logWriter.Lines;

    public void ClearLogs()
    {
        _logWriter.Clear();
    }

    private VoidProxy(CollectingTextWriter logWriter, Task task, CancellationTokenSource cancellationTokenSource, string originalWorkingDirectory)
    {
        _logWriter = logWriter;
        _task = task;
        _cancellationTokenSource = cancellationTokenSource;
        _originalWorkingDirectory = originalWorkingDirectory;
    }

    public static Task<VoidProxy> CreateAsync(string targetServer, int proxyPort, bool ignoreFileServers = true, bool offlineMode = true, string? instanceName = null, CancellationToken cancellationToken = default)
    {
        return CreateAsync([targetServer], proxyPort, ignoreFileServers, offlineMode, instanceName, cancellationToken);
    }

    public static async Task<VoidProxy> CreateAsync(IEnumerable<string> targetServers, int proxyPort, bool ignoreFileServers = true, bool offlineMode = true, string? instanceName = null, CancellationToken cancellationToken = default)
    {
        instanceName ??= nameof(VoidProxy);

        RuntimeHelpers.RunClassConstructor(typeof(EntryPoint).TypeHandle);

        var originalWorkingDirectory = Directory.GetCurrentDirectory();
        var workingDirectory = Path.Combine(originalWorkingDirectory, instanceName);

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        Directory.SetCurrentDirectory(workingDirectory);

        var logWriter = new CollectingTextWriter();
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationToken = cancellationTokenSource.Token;

        var args = new List<string>
        {
            "--port", proxyPort.ToString(),
            "--logging", "Debug"
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

        var task = EntryPoint.RunAsync(logWriter: logWriter, cancellationToken: cancellationToken, args: [.. args]);

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

        return new VoidProxy(logWriter, task, cancellationTokenSource, originalWorkingDirectory);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await _cancellationTokenSource.CancelAsync();
        await _task;

        Directory.SetCurrentDirectory(_originalWorkingDirectory);
    }
}
