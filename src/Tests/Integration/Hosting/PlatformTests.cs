using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Proxy;
using Void.Tests.Streams;
using Xunit;

namespace Void.Tests.Integration.Hosting;

public class PlatformTests
{
    [Fact]
    public async Task EntryPoint_RunsStopsSuccessfully()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await VoidEntryPoint.RunAsync(new VoidEntryPoint.RunOptions { LogWriter = logs, WorkingDirectory = nameof(EntryPoint_RunsStopsSuccessfully) }, cancellationTokenSource.Token);

        try
        {
            Assert.Equal(0, exitCode);

            Assert.Contains(logs.Lines, line => line.Contains("Proxy started"));
            Assert.Contains(logs.Lines, line => line.Contains("Proxy stopped"));
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidEntryPoint)} failed to run or stop successfully.\n{exception}\nLogs:\n{logs.Text}");
        }
    }

    [Fact]
    public async Task EntryPoint_UsesPortOption()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await VoidEntryPoint.RunAsync(new VoidEntryPoint.RunOptions { LogWriter = logs, Arguments = ["--port", "50000"], WorkingDirectory = nameof(EntryPoint_UsesPortOption) }, cancellationTokenSource.Token);

        try
        {
            Assert.Equal(0, exitCode);

            Assert.Contains(logs.Lines, line => line.Contains("Connection listener started"));
            Assert.Contains(logs.Lines, line => line.Contains("50000"));
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidEntryPoint)} failed to run or stop successfully.\n{exception}\nLogs:\n{logs.Text}");
        }
    }

    [Fact]
    public async Task EntryPoint_UsesInterfaceOption()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await VoidEntryPoint.RunAsync(new VoidEntryPoint.RunOptions { LogWriter = logs, Arguments = ["--interface", "127.0.0.1"], WorkingDirectory = nameof(EntryPoint_UsesInterfaceOption) }, cancellationTokenSource.Token);

        try
        {
            Assert.Equal(0, exitCode);

            Assert.Contains(logs.Lines, line => line.Contains("Connection listener started"));
            Assert.Contains(logs.Lines, line => line.Contains("127.0.0.1"));
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidEntryPoint)} failed to run or stop successfully.\n{exception}\nLogs:\n{logs.Text}");
        }
    }
}
