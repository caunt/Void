using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Void.Client;

internal sealed partial class GameRuntime
{
    sealed class CurseForgeApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUri;
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public CurseForgeApiClient(HttpClient httpClient, Uri baseUri, string apiKey)
        {
            _httpClient = httpClient;
            _baseUri = baseUri;
            _apiKey = apiKey;
        }

        public async Task<List<CurseForgeProject>> SearchModsAsync(int gameId, string slug, CancellationToken cancellationToken)
        {
            var slugQuery = Uri.EscapeDataString(slug);
            var response = await GetAsync<CurseForgeApiListResponse<CurseForgeProject>>($"v1/mods/search?gameId={gameId}&slug={slugQuery}", cancellationToken);

            return response.Data ?? [];
        }

        public async Task<CurseForgeFile> GetModFileAsync(int modId, int fileId, CancellationToken cancellationToken)
        {
            var response = await GetAsync<CurseForgeApiResponse<CurseForgeFile>>($"v1/mods/{modId}/files/{fileId}", cancellationToken);

            return response.Data ?? throw new InvalidOperationException($"CurseForge file not found: mod {modId}, file {fileId}");
        }

        public async Task<string?> GetModFileDownloadUrlAsync(int modId, int fileId, CancellationToken cancellationToken)
        {
            var response = await GetAsync<CurseForgeApiResponse<string?>>($"v1/mods/{modId}/files/{fileId}/download-url", cancellationToken);

            return response.Data;
        }

        public async Task<List<CurseForgeFile>> GetFilesAsync(List<int> fileIds, CancellationToken cancellationToken)
        {
            var response = await PostAsync<CurseForgeApiListResponse<CurseForgeFile>>("v1/mods/files", new CurseForgeFilesRequest(fileIds), cancellationToken);

            return response.Data ?? [];
        }

        private async Task<T> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_baseUri, relativeUrl));

            return await SendAsync<T>(request, cancellationToken);
        }

        private async Task<T> PostAsync<T>(string relativeUrl, object body, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_baseUri, relativeUrl))
            {
                Content = JsonContent.Create(body, options: _jsonOptions)
            };

            return await SendAsync<T>(request, cancellationToken);
        }

        private async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.TryAddWithoutValidation("x-api-key", _apiKey);

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("CurseForge API returned an empty response");
        }
    }

    record CurseForgeApiListResponse<T>(List<T>? Data);

    record CurseForgeApiResponse<T>(T? Data);

    record CurseForgeProject(int Id, string? Slug);

    record CurseForgeFile(int Id, int ModId, string FileName, string? DownloadUrl);

    record CurseForgeFilesRequest(List<int> FileIds);

    record CurseForgeManifest(CurseForgeMinecraft? Minecraft, string? Overrides, List<CurseForgeManifestFile>? Files);

    record CurseForgeMinecraft(string? Version, List<CurseForgeModLoader>? ModLoaders);

    record CurseForgeModLoader(string? Id, bool? Primary);

    record CurseForgeManifestFile([property: JsonPropertyName("fileID")] int? FileId, bool? Required);

}
