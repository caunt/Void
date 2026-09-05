using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Void.IntegrationTests.Infrastructure.IO;

internal static class ClientDiagnosticsDownloader
{
    public static async Task DownloadAsync(HttpClient httpClient, Guid? sessionId, string directory, TimeSpan? timeout = null)
    {
        using var cancellation = new CancellationTokenSource(timeout ?? TimeSpan.FromSeconds(30));
        string? temporaryPath = null;
        try
        {
            Directory.CreateDirectory(directory);
            if (sessionId is not { } identifier)
                throw new InvalidOperationException("The client did not return a diagnostic session ID; its image may predate the diagnostics API.");

            var destination = Path.Combine(directory, $"client-diagnostics-{identifier}.zip");
            temporaryPath = destination + ".tmp";
            using var response = await httpClient.GetAsync($"/api/game/diagnostics/{identifier}", HttpCompletionOption.ResponseHeadersRead, cancellation.Token);
            response.EnsureSuccessStatusCode();
            if (response.Content.Headers.ContentType?.MediaType is not "application/zip")
                throw new InvalidOperationException("The diagnostics endpoint did not return a ZIP archive.");

            await using (var output = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
                await response.Content.CopyToAsync(output, cancellation.Token);
            File.Move(temporaryPath, destination, overwrite: true);
            Console.WriteLine($"Minecraft diagnostics saved to {destination}");
        }
        catch (Exception exception)
        {
            var message = $"Minecraft diagnostic collection failed for session {sessionId}: {exception}";
            Console.WriteLine(message);
            try
            {
                Directory.CreateDirectory(directory);
                await File.WriteAllTextAsync(Path.Combine(directory, "client-diagnostics-error.txt"), message, CancellationToken.None);
            }
            catch (Exception writeException)
            {
                Console.WriteLine($"Could not save diagnostic collection error: {writeException.Message}");
            }
        }
        finally
        {
            try
            {
                if (temporaryPath is not null)
                    File.Delete(temporaryPath);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Could not remove incomplete diagnostic download: {exception.Message}");
            }
        }
    }
}
