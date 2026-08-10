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
using Void.Proxy.Api.Events.Links;
using Xunit;

public record VoidProxy(CollectingTextWriter LogWriter, VoidEntryPoint.RunResult RunResult, CancellationTokenSource CancellationTokenSource) : IIntegrationSide
{
    public string LogFileName => "void-proxy.log";
    public IEnumerable<string> Logs => LogWriter.Lines;
    public int Port => RunResult.ListeningPort;

    public static Task<VoidProxy> CreateAsync(string workingDirectory, string targetServer, int proxyPort = 0, bool ignoreFileServers = true, bool offlineMode = true, CancellationToken cancellationToken = default)
    {
        return CreateAsync(workingDirectory, [targetServer], proxyPort, ignoreFileServers, offlineMode, cancellationToken);
    }

    public static async Task<VoidProxy> CreateAsync(string workingDirectory, IEnumerable<string> targetServers, int proxyPort = 0, bool ignoreFileServers = true, bool offlineMode = true, CancellationToken cancellationToken = default)
    {
        var logWriter = new CollectingTextWriter();
        var cancellationTokenSource = new CancellationTokenSource();

        workingDirectory = Path.Combine(workingDirectory, nameof(VoidProxy));

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

    public Task<IEnumerable<string>> ReadLogsAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IEnumerable<string>>(LogWriter.GetLinesSince(since));
    }

    public void AssertNoWarningOrHigherLogsSince(DateTime since)
    {
        var unexpectedLogs = LogWriter.GetLinesSince(since).Where(line =>
            line.Contains(" WRN] ", StringComparison.Ordinal) ||
            line.Contains(" ERR] ", StringComparison.Ordinal) ||
            line.Contains(" FTL] ", StringComparison.Ordinal));

        Assert.Empty(unexpectedLogs);
    }

    public async Task WaitForPlayerDisconnectionAsync(string username, CancellationToken cancellationToken = default)
    {
        var stateLock = new Lock();
        var disconnected = false;

        await LogWriter.WaitForLineAsync(line =>
        {
            using var _ = stateLock.EnterScope();

            if (!disconnected)
            {
                disconnected = line.Contains($"Player {username} disconnected", StringComparison.Ordinal);
                return false;
            }

            return line.Contains($"Completed invoking {nameof(LinkStoppedEvent)} event", StringComparison.Ordinal);
        }, cancellationToken);
    }

    public async Task WaitForServerConnectionAndKeepAliveAsync(string username, string serverName, CancellationToken cancellationToken = default)
    {
        const string keepAliveRequestMarker = "Sending Keep Alive request ";
        var stateLock = new Lock();
        var connected = false;
        long? requestId = null;

        await LogWriter.WaitForLineAsync(line =>
        {
            using var _ = stateLock.EnterScope();

            if (!line.Contains(username, StringComparison.Ordinal))
                return false;

            if (!connected)
            {
                connected = line.Contains($"connected to {serverName}", StringComparison.Ordinal);
                return false;
            }

            if (requestId is null)
            {
                var markerIndex = line.IndexOf(keepAliveRequestMarker, StringComparison.Ordinal);

                if (markerIndex < 0)
                    return false;

                var requestIdText = line[(markerIndex + keepAliveRequestMarker.Length)..].Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

                if (long.TryParse(requestIdText, out var parsedRequestId))
                    requestId = parsedRequestId;

                return false;
            }

            return line.Contains($"Keep Alive hit {requestId.Value} received", StringComparison.Ordinal);
        }, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await CancellationTokenSource.CancelAsync();
        await RunResult.CompletionTask;
        await RunResult.DisposeAsync();
    }
}
