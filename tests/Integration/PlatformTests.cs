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
    public async Task EntryPoint_RunsStopsSuccessfuly()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await EntryPoint.RunAsync(cancellationToken: cancellationTokenSource.Token);

        Assert.Equal(0, exitCode);

        Assert.Contains(logs.Lines, line => line.Contains("Hosting started"));
        Assert.Contains(logs.Lines, line => line.Contains("Hosting stopped"));
    }
}
