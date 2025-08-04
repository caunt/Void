using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Proxy;
using Void.Tests.Streams;
using Xunit;

namespace Void.Tests.Integration;

public class PlatformTests
{
    [Fact]
    public async Task EntryPoint_RunsStopsSuccessfully()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await EntryPoint.RunAsync(logWriter: logs, cancellationToken: cancellationTokenSource.Token);

        Assert.Equal(0, exitCode);

        Assert.Contains(logs.Lines, line => line.Contains("Hosting started"));
        Assert.Contains(logs.Lines, line => line.Contains("Hosting stopped"));
    }

    [Fact]
    public async Task EntryPoint_UsesPortOption()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await EntryPoint.RunAsync(logWriter: logs, cancellationToken: cancellationTokenSource.Token, args: ["--port", "50000"]);

        Assert.Equal(0, exitCode);

        Assert.Contains(logs.Lines, line => line.Contains("Connection listener started"));
        Assert.Contains(logs.Lines, line => line.Contains("50000"));
    }

    [Fact]
    public async Task EntryPoint_UsesInterfaceOption()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await EntryPoint.RunAsync(logWriter: logs, cancellationToken: cancellationTokenSource.Token, args: ["--interface", "127.0.0.1"]);

        Assert.Equal(0, exitCode);

        Assert.Contains(logs.Lines, line => line.Contains("Connection listener started"));
        Assert.Contains(logs.Lines, line => line.Contains("127.0.0.1"));
    }
}
