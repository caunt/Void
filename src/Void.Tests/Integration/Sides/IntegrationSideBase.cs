using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Void.Tests.Exceptions;
using Void.Tests.Extensions;

namespace Void.Tests.Integration.Sides;

public abstract class IntegrationSideBase(string? jreBinaryPath = null) : IIntegrationSide
{
    protected Process? _process;
    protected readonly List<string> _logs = [];

    public IEnumerable<string> Logs => _logs;

    public abstract Task RunAsync(CancellationToken cancellationToken);
    public abstract Task SetupAsync(HttpClient client, CancellationToken cancellationToken = default);

    public void StartApplication(string fileName, params string[] userArguments)
    {
        var arguments = new List<string>(userArguments);
        var protocols = new string[] { "http", "https" };

        var isJar = fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase);
        var processStartInfo = new ProcessStartInfo(fileName: isJar ? (File.Exists(jreBinaryPath) ? jreBinaryPath : throw new IntegrationTestException("JRE is not installed")) : fileName)
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
            var variants = new string[] { name, name.ToUpperInvariant() };

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

        _process = Process.Start(processStartInfo) ?? throw new IntegrationTestException($"Failed to start process for {fileName} with arguments: {string.Join(" ", arguments)}");
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_process is not { HasExited: false })
            return;

        await _process.ExitAsync();
    }

    protected async Task HandleOutputAsync(Func<string, bool> handler, CancellationToken cancellationToken)
    {
        if (_process is null)
            throw new InvalidOperationException("Application is not started.");

        var taskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken), useSynchronizationContext: false);

        void Handler(object sender, DataReceivedEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Data != null)
                {
                    _logs.Add(eventArgs.Data);

                    if (!handler(eventArgs.Data))
                        return;

                    taskCompletionSource.SetResult();
                }
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        }

        _process.OutputDataReceived += Handler;
        _process.ErrorDataReceived += Handler;

        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        try
        {
            await taskCompletionSource.Task;
        }
        finally
        {
            _process.CancelOutputRead();
            _process.CancelErrorRead();

            _process.OutputDataReceived -= Handler;
            _process.ErrorDataReceived -= Handler;
        }
    }
}
