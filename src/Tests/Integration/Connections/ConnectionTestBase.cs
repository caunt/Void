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
using Void.Tests.Integration.Sides;

public abstract class ConnectionTestBase : IDisposable
{
    private static readonly string WorkingDirectory = Path.Combine(Path.GetTempPath(), $"{nameof(Tests)}", "IntegrationTests");
    protected readonly HttpClient _httpClient = new();

    public ConnectionTestBase()
    {
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Void.Tests/1.0");
        _httpClient.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");
        _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

        if (!Directory.Exists(WorkingDirectory))
            Directory.CreateDirectory(WorkingDirectory);
    }

    public async Task ExecuteAsync(IIntegrationServer server, IIntegrationClient client, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            server.SetupAsync(WorkingDirectory, _httpClient, cancellationToken),
            client.SetupAsync(WorkingDirectory, _httpClient, cancellationToken));

        Task serverTask, clientTask;

        try
        {
            // Start the server
            serverTask = server.RunAsync(cancellationToken);
            var completedServerTask = await Task.WhenAny(serverTask, server.ServerLoadingTask);

            if (completedServerTask == serverTask)
                await serverTask; // Rethrow server exceptions

            // Start the client (which automatically sends the message to the server)
            clientTask = client.RunAsync(cancellationToken);

            // Wait for the server to exit (it will exit when it receives the expected message)
            await serverTask;
        }
        catch (Exception exception)
        {
            throw new IntegrationTestException(exception.Message + $"\nServer logs:\n{string.Join("\n", server.Logs)}\n\n\nClient logs:\n{string.Join("\n", client.Logs)}", exception);
        }
    }

    public async Task ExecuteAsync(IIntegrationServer server, IIntegrationSide proxy, IIntegrationClient client, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            server.SetupAsync(WorkingDirectory, _httpClient, cancellationToken),
            proxy.SetupAsync(WorkingDirectory, _httpClient, cancellationToken),
            client.SetupAsync(WorkingDirectory, _httpClient, cancellationToken));

        Task serverTask, proxyTask, clientTask;

        try
        {
            serverTask = server.RunAsync(cancellationToken);
            var completedServerTask = await Task.WhenAny(serverTask, server.ServerLoadingTask);

            if (completedServerTask == serverTask)
                await serverTask;

            proxyTask = proxy.RunAsync(cancellationToken);

            clientTask = client.RunAsync(cancellationToken);

            await serverTask;
        }
        catch (Exception exception)
        {
            throw new IntegrationTestException(exception.Message + $"\nServer logs:\n{string.Join("\n", server.Logs)}\n\nProxy logs:\n{string.Join("\n", proxy.Logs)}\n\nClient logs:\n{string.Join("\n", client.Logs)}", exception);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (Directory.Exists(WorkingDirectory))
            Directory.Delete(WorkingDirectory, true);
    }
}
