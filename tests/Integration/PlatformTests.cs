using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Proxy;
using Xunit;

namespace Void.Tests.Integration;

public class PlatformTests
{
    [Fact]
    public async Task EntryPoint_RunsStopsSuccessfuly()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await EntryPoint.RunAsync(cancellationTokenSource.Token);

        Assert.Equal(0, exitCode);
    }
}
