using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Void.Tests.Extensions;

public static class HttpClientExtensions
{
    public static async Task DownloadFileAsync(this HttpClient client, string url, string destination, CancellationToken cancellationToken)
    {

        for (var attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                await using var fileStream = File.Create(destination);
                await response.Content.CopyToAsync(fileStream, cancellationToken);

                return;
            }
            catch (HttpRequestException) when (attempt < 2)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
