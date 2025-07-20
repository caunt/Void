using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Void.Tests.Extensions;

public static class HttpClientExtensions
{
    public static async Task DownloadFileAsync(this HttpClient client, string url, string destination, CancellationToken cancellationToken)
    {
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken = default);
        response.EnsureSuccessStatusCode();

        await using var fileStream = File.Create(destination);
        await response.Content.CopyToAsync(fileStream, cancellationToken);
    }
}
