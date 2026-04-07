using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.IO;
using Void.Proxy;
using Xunit;

namespace Void.IntegrationTests.EntryPoint;

public class VoidEntryPointTests
{
    private const string EarlyExitString = "Proxy started";

    [Fact]
    public async Task EntryPoint_RunsStopsSuccessfully()
    {
        var logs = new CollectingTextWriter();
        var exitCode = await RunVoidAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            WorkingDirectory = nameof(EntryPoint_RunsStopsSuccessfully)
        }, EarlyExitString);

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
        var exitCode = await RunVoidAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--port", "50000"],
            WorkingDirectory = nameof(EntryPoint_UsesPortOption)
        }, EarlyExitString);

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
        var exitCode = await RunVoidAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--interface", "127.0.0.1"],
            WorkingDirectory = nameof(EntryPoint_UsesInterfaceOption)
        }, EarlyExitString);

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

    [Fact]
    public async Task EntryPoint_UsesServerOptionWithoutPort_DefaultsTo25565()
    {
        var logs = new CollectingTextWriter();
        var exitCode = await RunVoidAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--server", "paper.default.svc.cluster.local"],
            WorkingDirectory = nameof(EntryPoint_UsesServerOptionWithoutPort_DefaultsTo25565)
        }, EarlyExitString);

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
        var exitCode = await RunVoidAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--server", "localhost:25566"],
            WorkingDirectory = nameof(EntryPoint_UsesServerOptionWithPort_UsesSpecifiedPort)
        }, EarlyExitString);

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
        var exitCode = await RunVoidAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = [
                "--server", "server1.local",
                "--server", "server2.local:25567",
                "--server", "192.168.1.1"
            ],
            WorkingDirectory = nameof(EntryPoint_UsesMultipleServersWithMixedFormats_ParsesAllCorrectly)
        }, EarlyExitString);

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
        var exitCode = await RunVoidAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--server", "[2001:db8::1]:25565"],
            WorkingDirectory = nameof(EntryPoint_UsesServerOptionWithIPv6_ParsesCorrectly)
        }, EarlyExitString);

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

    [Fact]
    public async Task EntryPoint_UsesServerOptionWithIPv6WithoutPort_DefaultsTo25565()
    {
        var logs = new CollectingTextWriter();
        var exitCode = await RunVoidAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--server", "[2001:db8::1]"],
            WorkingDirectory = nameof(EntryPoint_UsesServerOptionWithIPv6WithoutPort_DefaultsTo25565)
        }, EarlyExitString);

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

    [Fact]
    public async Task EntryPoint_WithValidNuGetRepository_ProbesSuccessfully()
    {
        var logs = new CollectingTextWriter();
        var exitCode = await RunVoidAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--repository", "https://api.nuget.org/v3/index.json"],
            WorkingDirectory = nameof(EntryPoint_WithValidNuGetRepository_ProbesSuccessfully)
        }, EarlyExitString);

        try
        {
            Assert.Equal(0, exitCode);
            Assert.Contains(logs.Lines, line => line.Contains("Proxy started"));
            Assert.Contains(logs.Lines, line => line.Contains("Custom NuGet repositories"));
            Assert.Contains(logs.Lines, line => line.Contains("https://api.nuget.org/v3/index.json") && line.Contains("[Ok]"));
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidEntryPoint)} failed to run or stop successfully.\n{exception}\nLogs:\n{logs.Text}");
        }
    }

    [Fact]
    public async Task EntryPoint_WithInvalidNuGetRepository_ProbesUnsuccessfully()
    {
        var logs = new CollectingTextWriter();
        var exitCode = await RunVoidAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--repository", "http://127.0.0.1:1/v3/index.json"],
            WorkingDirectory = nameof(EntryPoint_WithInvalidNuGetRepository_ProbesUnsuccessfully)
        }, EarlyExitString);

        try
        {
            Assert.Equal(0, exitCode);
            Assert.Contains(logs.Lines, line => line.Contains("Proxy started"));
            Assert.Contains(logs.Lines, line => line.Contains("Custom NuGet repositories"));
            Assert.Contains(logs.Lines, line => line.Contains("http://127.0.0.1:1/v3/index.json") && (line.Contains("[Timeout]") || line.Contains("[NotConnected]")));
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidEntryPoint)} failed to run or stop successfully.\n{exception}\nLogs:\n{logs.Text}");
        }
    }

    private static async Task<int> RunVoidAsync(VoidEntryPoint.RunOptions runOptions, params string[] earlyExitLines)
    {
        return await RunVoidAsync(runOptions, TimeSpan.FromSeconds(10), earlyExitLines);
    }

    private static async Task<int> RunVoidAsync(VoidEntryPoint.RunOptions runOptions, TimeSpan timeout, params string[] earlyExitLines)
    {
        using var cancellationTokenSource = new CancellationTokenSource(timeout);

        if (runOptions.LogWriter is CollectingTextWriter collectingTextWriter)
        {
            var proxyStarted = false;

            collectingTextWriter.OnLine += line =>
            {
                if (!proxyStarted && line.Contains("Proxy started"))
                    proxyStarted = true;

                if (!proxyStarted)
                    return;

                if (!earlyExitLines.Any(line.Contains))
                    return;

                cancellationTokenSource.Cancel();
            };
        }

        await using var result = await VoidEntryPoint.RunAsync(runOptions, cancellationTokenSource.Token);
        return result.ExitCode;
    }
}
