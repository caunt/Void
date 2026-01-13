using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Proxy;
using Void.Tests.Streams;
using Xunit;

namespace Void.Tests.ServersTests;

public class ServerArgumentParsingTests
{
    [Fact]
    public async Task EntryPoint_UsesServerOptionWithoutPort_DefaultsTo25565()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await VoidEntryPoint.RunAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--ignore-file-servers", "--server", "paper.default.svc.cluster.local"],
            WorkingDirectory = nameof(EntryPoint_UsesServerOptionWithoutPort_DefaultsTo25565)
        }, cancellationTokenSource.Token);

        try
        {
            Assert.Equal(0, exitCode);

            Assert.Contains(logs.Lines, line => line.Contains("Registered servers"));
            Assert.Contains(logs.Lines, line => line.Contains("paper.default.svc.cluster.local:25565"));
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidEntryPoint)} failed to run or stop successfully.\n{exception}\nLogs:\n{logs.Text}");
        }
    }

    [Fact]
    public async Task EntryPoint_UsesServerOptionWithPort_UsesSpecifiedPort()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await VoidEntryPoint.RunAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--ignore-file-servers", "--server", "localhost:25566"],
            WorkingDirectory = nameof(EntryPoint_UsesServerOptionWithPort_UsesSpecifiedPort)
        }, cancellationTokenSource.Token);

        try
        {
            Assert.Equal(0, exitCode);

            Assert.Contains(logs.Lines, line => line.Contains("Registered servers"));
            Assert.Contains(logs.Lines, line => line.Contains("localhost:25566"));
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidEntryPoint)} failed to run or stop successfully.\n{exception}\nLogs:\n{logs.Text}");
        }
    }

    [Fact]
    public async Task EntryPoint_UsesMultipleServersWithMixedFormats_ParsesAllCorrectly()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await VoidEntryPoint.RunAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = [
                "--ignore-file-servers",
                "--server", "server1.local",
                "--server", "server2.local:25567",
                "--server", "192.168.1.1"
            ],
            WorkingDirectory = nameof(EntryPoint_UsesMultipleServersWithMixedFormats_ParsesAllCorrectly)
        }, cancellationTokenSource.Token);

        try
        {
            Assert.Equal(0, exitCode);

            Assert.Contains(logs.Lines, line => line.Contains("Registered servers"));
            Assert.Contains(logs.Lines, line => line.Contains("server1.local:25565"));
            Assert.Contains(logs.Lines, line => line.Contains("server2.local:25567"));
            Assert.Contains(logs.Lines, line => line.Contains("192.168.1.1:25565"));
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidEntryPoint)} failed to run or stop successfully.\n{exception}\nLogs:\n{logs.Text}");
        }
    }

    [Fact]
    public async Task EntryPoint_UsesServerOptionWithIPv6_ParsesCorrectly()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await VoidEntryPoint.RunAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--ignore-file-servers", "--server", "[2001:db8::1]:25565"],
            WorkingDirectory = nameof(EntryPoint_UsesServerOptionWithIPv6_ParsesCorrectly)
        }, cancellationTokenSource.Token);

        try
        {
            Assert.Equal(0, exitCode);

            Assert.Contains(logs.Lines, line => line.Contains("Registered servers"));
            Assert.Contains(logs.Lines, line => line.Contains("[2001:db8::1]:25565"));
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidEntryPoint)} failed to run or stop successfully.\n{exception}\nLogs:\n{logs.Text}");
        }
    }
}
