namespace Void.Tests.Integration.Connections;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Void.Tests.Exceptions;
using Void.Tests.Integration.Sides.Clients;
using Void.Tests.Integration.Sides.Servers;

public abstract class ConnectionTestBase : IDisposable
{
    private static readonly string WorkingDirectory = Path.Combine(Path.GetTempPath(), $"{nameof(Tests)}", "IntegrationTests");
    protected readonly HttpClient _client = new();

    public ConnectionTestBase()
    {
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("Void.Tests/1.0");
        _client.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");
        _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

        if (!Directory.Exists(WorkingDirectory))
            Directory.CreateDirectory(WorkingDirectory);
    }


    public async Task ExecuteAsync(IIntegrationServer server, IIntegrationClient client, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(server.SetupAsync(WorkingDirectory, _client, cancellationToken), client.SetupAsync(WorkingDirectory, _client, cancellationToken));

        Task serverTask, clientTask;

        try
        {
            serverTask = server.RunAsync(cancellationToken);
            var completedServerTask = await Task.WhenAny(serverTask, server.ServerLoadingTask);

            if (completedServerTask == serverTask)
                await serverTask; // Rethrow server exceptions

            clientTask = server.RunAsync(cancellationToken);
            await serverTask;

            // Completion
            await clientTask;

            // try
            // {
            //     await clientTask;
            // }
            // catch (TaskCanceledException)
            // {
            //     // Expected when cancelling after server received expected message
            // }
        }
        catch (Exception exception)
        {
            throw new IntegrationTestException(exception.Message + $"\nServer logs:\n{string.Join("\n", server.Logs)}\n\n\nClient logs:\n{string.Join("\n", client.Logs)}", exception);
        }
    }

    public void Dispose()
    {
        if (Directory.Exists(WorkingDirectory))
            Directory.Delete(WorkingDirectory, true);

        GC.SuppressFinalize(this);
    }
}
