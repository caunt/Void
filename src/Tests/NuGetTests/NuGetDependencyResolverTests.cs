using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Proxy;
using Void.Tests.Streams;
using Xunit;

namespace Void.Tests.NuGetTests;

public class NuGetDependencyResolverTests
{
    [Fact]
    public async Task EntryPoint_WithValidNuGetRepository_ProbesSuccessfully()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await VoidEntryPoint.RunAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--repository", "https://api.nuget.org/v3/index.json"],
            WorkingDirectory = nameof(EntryPoint_WithValidNuGetRepository_ProbesSuccessfully)
        }, cancellationTokenSource.Token);

        try
        {
            Assert.Equal(0, exitCode);
            Assert.DoesNotContain(logs.Lines, line => line.Contains("is not responding"));
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidEntryPoint)} failed to run or stop successfully.\n{exception}\nLogs:\n{logs.Text}");
        }
    }

    [Fact]
    public async Task EntryPoint_WithInvalidNuGetRepository_LogsWarning()
    {
        var logs = new CollectingTextWriter();

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var exitCode = await VoidEntryPoint.RunAsync(new VoidEntryPoint.RunOptions
        {
            LogWriter = logs,
            Arguments = ["--repository", "https://invalid-nuget-repository-that-does-not-exist.example.com/v3/index.json"],
            WorkingDirectory = nameof(EntryPoint_WithInvalidNuGetRepository_LogsWarning)
        }, cancellationTokenSource.Token);

        try
        {
            Assert.Equal(0, exitCode);
            Assert.Contains(logs.Lines, line => line.Contains("is not responding") || line.Contains("returned non-success status code"));
        }
        catch (Exception exception)
        {
            Assert.Fail($"{nameof(VoidEntryPoint)} failed to run or stop successfully.\n{exception}\nLogs:\n{logs.Text}");
        }
    }
}
