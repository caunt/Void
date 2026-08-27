using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Void.Client;

internal sealed class GameTextRecognizer : IAsyncDisposable
{
    private const string PythonPath = "/opt/paddleocr/bin/python";
    private const string WorkerPath = "/opt/portableminecraftclient/ocr-worker.py";
    private const string ResponsePrefix = "VOID_OCR_RESPONSE ";

    private readonly SemaphoreSlim _lock = new(1, 1);
    private Process? _process;
    private long _requestId;

    public async Task<IReadOnlyList<RecognizedText>> RecognizeAsync(byte[] image, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            for (var attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    return await RecognizeCoreAsync(image, cancellationToken);
                }
                catch (Exception exception) when (exception is IOException or JsonException or InvalidOperationException)
                {
                    if (attempt is 0)
                    {
                        await StopWorkerAsync();
                        continue;
                    }

                    throw new OcrRecognitionException("OCR worker failed after restart", exception);
                }
            }

            throw new InvalidOperationException("OCR worker did not return a result");
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _lock.WaitAsync();

        try
        {
            await StopWorkerAsync();
        }
        finally
        {
            _lock.Release();
            _lock.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    private async Task<IReadOnlyList<RecognizedText>> RecognizeCoreAsync(byte[] image, CancellationToken cancellationToken)
    {
        var process = EnsureWorker();
        var requestId = Interlocked.Increment(ref _requestId);
        var request = JsonSerializer.Serialize(new OcrRequest(requestId, Convert.ToBase64String(image)), JsonSerializerOptions.Web);
        await process.StandardInput.WriteLineAsync(request.AsMemory(), cancellationToken);
        await process.StandardInput.FlushAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await process.StandardOutput.ReadLineAsync(cancellationToken)
                       ?? throw new IOException("OCR worker exited without returning a response");

            if (!line.StartsWith(ResponsePrefix, StringComparison.Ordinal))
                continue;

            var response = JsonSerializer.Deserialize<OcrResponse>(line[ResponsePrefix.Length..], JsonSerializerOptions.Web)
                           ?? throw new JsonException("OCR worker returned an empty response");

            if (response.Id != requestId)
                continue;

            if (!string.IsNullOrWhiteSpace(response.Error))
                throw new InvalidOperationException($"OCR worker failed: {response.Error}");

            return response.Items ?? [];
        }

        cancellationToken.ThrowIfCancellationRequested();
        return [];
    }

    private Process EnsureWorker()
    {
        if (_process is { HasExited: false })
            return _process;

        var process = new Process
        {
            StartInfo = new ProcessStartInfo(PythonPath, WorkerPath)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            },
            EnableRaisingEvents = true
        };

        process.ErrorDataReceived += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
                Console.Error.WriteLine($"[Void.Client.OCR] {args.Data}");
        };

        if (!process.Start())
            throw new InvalidOperationException("OCR worker could not be started");

        process.BeginErrorReadLine();
        _process = process;
        return process;
    }

    private async Task StopWorkerAsync()
    {
        if (_process is null)
            return;

        try
        {
            _process.StandardInput.Close();

            if (!_process.HasExited)
            {
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                try
                {
                    await _process.WaitForExitAsync(timeout.Token);
                }
                catch (OperationCanceledException)
                {
                    _process.Kill(entireProcessTree: true);
                    await _process.WaitForExitAsync();
                }
            }
        }
        finally
        {
            _process.Dispose();
            _process = null;
        }
    }

    private sealed record OcrRequest(long Id, string Image);
    private sealed record OcrResponse(long? Id, IReadOnlyList<RecognizedText>? Items, string? Error);
}

internal sealed record RecognizedText(string Text, double Confidence, IReadOnlyList<IReadOnlyList<double>> Polygon);

internal sealed class OcrRecognitionException(string message, Exception innerException) : Exception(message, innerException);

internal enum ConnectionTextAction
{
    Multiplayer,
    Proceed,
    DirectConnection,
    ServerAddress,
    JoinServer,
    Back,
    BackToGame
}

internal enum ConnectionNavigationKind
{
    BackToGame,
    JoinServer,
    DirectConnection,
    Proceed,
    Multiplayer,
    Back
}

internal sealed record ConnectionNavigationSelection(ConnectionNavigationKind Kind, ConnectionTextAction TextAction);

internal static class ConnectionNavigationSelector
{
    public static ConnectionNavigationSelection? Select(IReadOnlyDictionary<ConnectionTextAction, ConnectionTextMatch> matches, bool hasServerAddressField)
    {
        if (matches.ContainsKey(ConnectionTextAction.BackToGame))
            return new(ConnectionNavigationKind.BackToGame, ConnectionTextAction.BackToGame);

        if (hasServerAddressField && matches.ContainsKey(ConnectionTextAction.ServerAddress) && matches.ContainsKey(ConnectionTextAction.JoinServer))
            return new(ConnectionNavigationKind.JoinServer, ConnectionTextAction.JoinServer);

        if (matches.ContainsKey(ConnectionTextAction.DirectConnection))
            return new(ConnectionNavigationKind.DirectConnection, ConnectionTextAction.DirectConnection);

        if (matches.ContainsKey(ConnectionTextAction.Proceed))
            return new(ConnectionNavigationKind.Proceed, ConnectionTextAction.Proceed);

        if (matches.ContainsKey(ConnectionTextAction.Multiplayer))
            return new(ConnectionNavigationKind.Multiplayer, ConnectionTextAction.Multiplayer);

        if (matches.ContainsKey(ConnectionTextAction.Back))
            return new(ConnectionNavigationKind.Back, ConnectionTextAction.Back);

        return null;
    }
}

internal sealed record ConnectionTextMatch(ConnectionTextAction Action, string Text, double Confidence, double Similarity, OcrRectangle Bounds);

internal readonly record struct OcrRectangle(int Left, int Top, int Width, int Height)
{
    public int Right => Left + Width;
    public int Bottom => Top + Height;
    public int CenterX => Left + Width / 2;
    public int CenterY => Top + Height / 2;
}

internal static class ConnectionTextMatcher
{
    private const double MinimumConfidence = 0.65;
    private const double MinimumSimilarity = 0.80;

    private static readonly IReadOnlyDictionary<ConnectionTextAction, string> Labels = new Dictionary<ConnectionTextAction, string>
    {
        [ConnectionTextAction.Multiplayer] = "multiplayer",
        [ConnectionTextAction.Proceed] = "proceed",
        [ConnectionTextAction.DirectConnection] = "directconnection",
        [ConnectionTextAction.ServerAddress] = "serveraddress",
        [ConnectionTextAction.JoinServer] = "joinserver",
        [ConnectionTextAction.Back] = "back",
        [ConnectionTextAction.BackToGame] = "backtogame"
    };

    public static IReadOnlyDictionary<ConnectionTextAction, ConnectionTextMatch> Match(IEnumerable<RecognizedText> recognizedTexts)
    {
        var matches = new Dictionary<ConnectionTextAction, ConnectionTextMatch>();

        foreach (var recognizedText in recognizedTexts)
        {
            if (recognizedText.Confidence < MinimumConfidence || !TryGetBounds(recognizedText.Polygon, out var bounds))
                continue;

            var normalizedText = Normalize(recognizedText.Text);

            if (normalizedText.Length is 0)
                continue;

            foreach (var (action, label) in Labels)
            {
                var similarity = CalculateSimilarity(normalizedText, label);

                if (similarity < MinimumSimilarity)
                    continue;

                var match = new ConnectionTextMatch(action, recognizedText.Text, recognizedText.Confidence, similarity, bounds);

                if (!matches.TryGetValue(action, out var existing)
                    || match.Similarity > existing.Similarity
                    || match.Similarity == existing.Similarity && match.Confidence > existing.Confidence)
                    matches[action] = match;
            }
        }

        return matches;
    }

    internal static string Normalize(string text)
    {
        var builder = new StringBuilder(text.Length);

        foreach (var character in text.Normalize(NormalizationForm.FormKD))
        {
            if (char.IsLetterOrDigit(character))
                builder.Append(char.ToLower(character, CultureInfo.InvariantCulture));
        }

        return builder.ToString();
    }

    internal static double CalculateSimilarity(string left, string right)
    {
        if (left == right)
            return 1;

        if (left.Length is 0 || right.Length is 0)
            return 0;

        var previous = new int[right.Length + 1];
        var current = new int[right.Length + 1];

        for (var index = 0; index <= right.Length; index++)
            previous[index] = index;

        for (var leftIndex = 1; leftIndex <= left.Length; leftIndex++)
        {
            current[0] = leftIndex;

            for (var rightIndex = 1; rightIndex <= right.Length; rightIndex++)
            {
                var substitutionCost = left[leftIndex - 1] == right[rightIndex - 1] ? 0 : 1;
                current[rightIndex] = Math.Min(
                    Math.Min(current[rightIndex - 1] + 1, previous[rightIndex] + 1),
                    previous[rightIndex - 1] + substitutionCost);
            }

            (previous, current) = (current, previous);
        }

        return 1.0 - (double)previous[right.Length] / Math.Max(left.Length, right.Length);
    }

    private static bool TryGetBounds(IReadOnlyList<IReadOnlyList<double>> polygon, out OcrRectangle bounds)
    {
        var points = polygon.Where(point => point.Count >= 2).ToArray();

        if (points.Length is 0)
        {
            bounds = default;
            return false;
        }

        var left = (int)Math.Floor(points.Min(point => point[0]));
        var top = (int)Math.Floor(points.Min(point => point[1]));
        var right = (int)Math.Ceiling(points.Max(point => point[0]));
        var bottom = (int)Math.Ceiling(points.Max(point => point[1]));

        if (right <= left || bottom <= top)
        {
            bounds = default;
            return false;
        }

        bounds = new OcrRectangle(left, top, right - left, bottom - top);
        return true;
    }
}
