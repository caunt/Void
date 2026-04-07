namespace Void.IntegrationTests.Infrastructure.Harness.Sides;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.IO;
using Void.Proxy;
using Xunit;

public record VoidProxy(CollectingTextWriter LogWriter, VoidEntryPoint.RunResult RunResult, CancellationTokenSource CancellationTokenSource) : IIntegrationSide
{
    public IEnumerable<string> Logs => LogWriter.Lines;
    public int Port => RunResult.ListeningPort;

    public static Task<VoidProxy> CreateAsync(string workingDirectory, string targetServer, int proxyPort = 0, bool ignoreFileServers = true, bool offlineMode = true, CancellationToken cancellationToken = default)
    {
        return CreateAsync(workingDirectory, [targetServer], proxyPort, ignoreFileServers, offlineMode, cancellationToken);
    }

    public static async Task<VoidProxy> CreateAsync(string workingDirectory, IEnumerable<string> targetServers, int proxyPort = 0, bool ignoreFileServers = true, bool offlineMode = true, CancellationToken cancellationToken = default)
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

        var result = await VoidEntryPoint.RunAsync(new VoidEntryPoint.RunOptions { WorkingDirectory = workingDirectory, Arguments = [.. args], LogWriter = logWriter }, cancellationTokenSource.Token);

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

        return new VoidProxy(logWriter, result, cancellationTokenSource);
    }

    public void ClearLogs()
    {
        LogWriter.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await CancellationTokenSource.CancelAsync();
        await RunResult.CompletionTask;
        await RunResult.DisposeAsync();
    }
}
