using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octokit;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;
using Xunit;

namespace Void.Tests.Integration.Sides;

public abstract class IntegrationSideBase : IIntegrationSide
{
    private const int MaxGitHubReleasesToConsider = 3;
    private static readonly GitHubClient _gitHubClient = new(new ProductHeaderValue($"{nameof(Void)}.{nameof(Tests)}"));

    protected string? _jreBinaryPath;

    protected Process? _process;
    protected readonly List<string> _logs = [];

    public IEnumerable<string> Logs => _logs;

    static IntegrationSideBase()
    {
        if (Environment.GetEnvironmentVariable("GITHUB_TOKEN") is { } token)
            _gitHubClient.Credentials = new Credentials(token);
    }

    public void StartApplication(string fileName, params string[] userArguments)
    {
        if (_process is { HasExited: false })
            throw new IntegrationTestException($"Process for {fileName} is already running.");

        var arguments = new List<string>(userArguments);
        var protocols = new[] { "http", "https" };

        var isJar = fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase);
        var processStartInfo = new ProcessStartInfo(fileName: isJar ? (File.Exists(_jreBinaryPath) ? _jreBinaryPath : throw new IntegrationTestException("JRE is not installed")) : fileName)
        {
            WorkingDirectory = Path.GetDirectoryName(fileName),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = false,
            UseShellExecute = false
        };

        foreach (var protocol in protocols)
        {
            var name = protocol + "_proxy";
            var variants = new[] { name, name.ToUpperInvariant() };

            foreach (var variant in variants)
            {
                var candidate = Environment.GetEnvironmentVariable(variant);

                if (string.IsNullOrWhiteSpace(candidate))
                    continue;

                processStartInfo.Environment[variant] = candidate;

                if (!isJar)
                    continue;

                if (!candidate.Contains("://"))
                    candidate = protocol + "://" + candidate;

                if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
                    continue;

                arguments.Add($"-D{protocol}.proxyHost={uri.Host}");
                arguments.Add($"-D{protocol}.proxyPort={uri.Port}");

                break;
            }
        }

        if (isJar)
        {
            arguments.Add("-jar");
            arguments.Add(fileName);
            arguments.Add("--nogui");
        }

        foreach (var argument in arguments)
            processStartInfo.ArgumentList.Add(argument);

        _process = Process.Start(processStartInfo) ?? throw new IntegrationTestException($"Failed to start process for {fileName} with arguments: {string.Join(' ', arguments)}");

        _process.OutputDataReceived += OnDataReceived;
        _process.ErrorDataReceived += OnDataReceived;

        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_process is not { HasExited: false })
            return;

        await _process.ExitAsync();

        _process.CancelOutputRead();
        _process.CancelErrorRead();

        _process.OutputDataReceived -= OnDataReceived;
        _process.ErrorDataReceived -= OnDataReceived;
    }

    protected async Task ReceiveOutputAsync(Func<string, bool> handler, CancellationToken cancellationToken)
    {
        if (_process is null)
            throw new InvalidOperationException("Application is not started.");

        var taskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken), useSynchronizationContext: false);

        void OnProcessExited(object? sender, EventArgs eventArgs) => taskCompletionSource.TrySetCanceled(cancellationToken);
        void OnDataReceived(object sender, DataReceivedEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Data is not null)
                {
                    if (handler(eventArgs.Data))
                        taskCompletionSource.SetResult();
                }
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        }

        _process.OutputDataReceived += OnDataReceived;
        _process.ErrorDataReceived += OnDataReceived;
        _process.Exited += OnProcessExited;

        try
        {
            await taskCompletionSource.Task;
        }
        finally
        {
            _process.OutputDataReceived -= OnDataReceived;
            _process.ErrorDataReceived -= OnDataReceived;
            _process.Exited -= OnProcessExited;
        }
    }

    protected static async Task<string> GetGitHubRepositoryLatestReleaseAssetAsync(string ownerName, string repositoryName, Predicate<string> assetFilter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var options = new ApiOptions
        {
            PageSize = MaxGitHubReleasesToConsider,
            PageCount = 1,
            StartPage = 1
        };

        var releases = await _gitHubClient.Repository.Release.GetAll(ownerName, repositoryName, options);

        var asset = releases
            .OrderByDescending(release => release.CreatedAt)
            .SelectMany(release => release.Assets)
            .FirstOrDefault(asset => assetFilter(asset.Name));

        Assert.NotNull(asset);

        return asset.BrowserDownloadUrl;
    }

    private void OnDataReceived(object? sender, DataReceivedEventArgs eventArgs)
    {
        if (eventArgs.Data is not null)
            _logs.Add(eventArgs.Data);
    }
}
