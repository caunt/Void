using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Void.Client;
using Xunit;

namespace Void.Client.UnitTests;

public class SessionDiagnosticsTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"void-diagnostics-tests-{Guid.NewGuid()}");

    private SessionDiagnostics Create(int maximumSessions = 10, int maximumSessionMb = 32, int maximumTotalMb = 256) => new(new DiagnosticsOptions
    {
        Directory = Path.Combine(_directory, "evidence"),
        MaximumSessions = maximumSessions,
        MaximumSessionMb = maximumSessionMb,
        MaximumTotalMb = maximumTotalMb
    });

    [Fact]
    public async Task ReportsAndFailureSurviveStopAndAnotherLaunchWithoutMixingSessions()
    {
        var gameDirectory = Path.Combine(_directory, "minecraft");
        Directory.CreateDirectory(Path.Combine(gameDirectory, "debug"));
        await File.WriteAllTextAsync(Path.Combine(gameDirectory, "debug", "disconnect-old.txt"), "old report", TestContext.Current.CancellationToken);
        var diagnostics = Create();
        var first = diagnostics.Begin("vanilla:1.21", gameDirectory);
        var failure = new GameClientException("client.connect.rejected", "connect", "connection.rejected", "bad packet").Failure;
        diagnostics.Record(new(GameState.Ready, 2, "connect", OperationState.Failed, 1, null, null, "failed", failure.Message, failure, [], DateTimeOffset.UtcNow, first));
        diagnostics.WriteOutput(first, "stderr", "first output");
        await File.WriteAllTextAsync(Path.Combine(gameDirectory, "debug", "disconnect-new.txt"), "original report", TestContext.Current.CancellationToken);
        diagnostics.Complete(first);
        diagnostics.Record(new(GameState.Idle, 3, "stop", OperationState.Succeeded, null, 0, null, "stopped", null, null, [], DateTimeOffset.UtcNow, first));
        var second = diagnostics.Begin("vanilla:1.22", gameDirectory);
        await File.WriteAllTextAsync(Path.Combine(gameDirectory, "debug", "disconnect-new.txt"), "overwritten report", TestContext.Current.CancellationToken);
        diagnostics.WriteOutput(second, "stdout", "second output");
        diagnostics.WriteOutput(first, "stderr", " late first output");

        var firstFiles = await ReadArchiveAsync(diagnostics, first);
        Assert.Equal("original report", firstFiles["debug-disconnect-new.txt"]);
        Assert.DoesNotContain("debug-disconnect-old.txt", firstFiles.Keys);
        Assert.Equal("first output late first output", firstFiles["console-stderr.log"]);
        Assert.Contains("client.connect.rejected", firstFiles["session.json"]);
        Assert.Contains("\"operation\":\"connect\"", firstFiles["operations.jsonl"]);
        Assert.Contains("\"operation\":\"stop\"", firstFiles["operations.jsonl"]);
        var secondFiles = await ReadArchiveAsync(diagnostics, second);
        Assert.DoesNotContain("console-stderr.log", secondFiles.Keys);
        Assert.Equal("second output", secondFiles["console-stdout.log"]);
    }

    [Fact]
    public async Task RetentionExpiresOldestCompletedSessionAndReloadsStoredEvidence()
    {
        var diagnostics = Create(maximumSessions: 1);
        var first = diagnostics.Begin("first", "");
        diagnostics.WriteOutput(first, "stdout", "old");
        diagnostics.Complete(first);
        var second = diagnostics.Begin("second", "");
        diagnostics.WriteOutput(second, "stdout", "new");
        diagnostics.Complete(second);
        Assert.Null(await diagnostics.DownloadAsync(first, CancellationToken.None));
        Assert.Equal(second, Assert.Single(diagnostics.List()).SessionId);
        var reloaded = Create(maximumSessions: 1);
        Assert.Equal("new", (await ReadArchiveAsync(reloaded, second))["console-stdout.log"]);
    }

    [Fact]
    public async Task StorageFailureStillReturnsManifestWithCollectionWarning()
    {
        Directory.CreateDirectory(_directory);
        var blockedDirectory = Path.Combine(_directory, "file");
        await File.WriteAllTextAsync(blockedDirectory, "not a directory", TestContext.Current.CancellationToken);
        var diagnostics = new SessionDiagnostics(new DiagnosticsOptions { Directory = blockedDirectory });
        var sessionId = diagnostics.Begin("failed preparation", "");
        diagnostics.WriteOutput(sessionId, "stderr", "failure");
        var files = await ReadArchiveAsync(diagnostics, sessionId);
        Assert.Contains("Could not store", files["session.json"]);
    }

    [Fact]
    public async Task OversizedOutputIsBoundedAndDescribedInManifest()
    {
        var diagnostics = Create(maximumSessionMb: 1, maximumTotalMb: 1);
        var sessionId = diagnostics.Begin("noisy client", "");
        for (var index = 0; index < 32; index++)
            diagnostics.WriteOutput(sessionId, "stderr", new string('x', 65536));
        var files = await ReadArchiveAsync(diagnostics, sessionId);
        Assert.Contains("size limit reached", files["session.json"]);
        Assert.True(Directory.EnumerateFiles(Path.Combine(_directory, "evidence"), "*", SearchOption.AllDirectories).Sum(file => new FileInfo(file).Length) <= 1024 * 1024);
    }

    [Fact]
    public async Task TokensAndSymlinkedReportsAreExcluded()
    {
        var gameDirectory = Path.Combine(_directory, "minecraft");
        Directory.CreateDirectory(Path.Combine(gameDirectory, "debug"));
        var diagnostics = Create();
        var identifier = diagnostics.Begin("test", gameDirectory);
        using (diagnostics.Enter(identifier))
            diagnostics.RegisterSecret("private-agent-token");
        diagnostics.WriteOutput(identifier, "stdout", "token=private-agent-token");
        await File.WriteAllTextAsync(Path.Combine(gameDirectory, "debug", "disconnect-real.txt"), "private-agent-token", TestContext.Current.CancellationToken);
        var secretFile = Path.Combine(_directory, "credentials.txt");
        await File.WriteAllTextAsync(secretFile, "private credentials", TestContext.Current.CancellationToken);
        if (!OperatingSystem.IsWindows())
            File.CreateSymbolicLink(Path.Combine(gameDirectory, "debug", "disconnect-link.txt"), secretFile);
        var files = await ReadArchiveAsync(diagnostics, identifier);
        Assert.DoesNotContain("debug-disconnect-link.txt", files.Keys);
        Assert.All(files.Values, value => Assert.DoesNotContain("private-agent-token", value));
        Assert.Equal("[redacted]", files["debug-disconnect-real.txt"]);
    }

    [Fact]
    public async Task ParallelDownloadsAndOutputRemainReadable()
    {
        var diagnostics = Create();
        var identifier = diagnostics.Begin("test", "");
        await Task.WhenAll(Enumerable.Range(0, 12).Select(async index =>
        {
            diagnostics.WriteOutput(identifier, "stdout", $"line {index}\n");
            var files = await ReadArchiveAsync(diagnostics, identifier);
            using var manifest = JsonDocument.Parse(files["session.json"]);
            Assert.Equal(identifier, manifest.RootElement.GetProperty("sessionId").GetGuid());
        }));
    }

    [Fact]
    public async Task OutputPumpRedactsTokensAcrossReadsAndPreservesConsoleOutput()
    {
        var diagnostics = Create();
        var identifier = diagnostics.Begin("test", "");
        var token = new string('a', 64);
        var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
        using (diagnostics.Enter(identifier))
        {
            diagnostics.RegisterSecret(token);
            diagnostics.RegisterSecret(encodedToken);
        }
        var text = new string('x', 4000) + token + " " + encodedToken + " end";
        using var reader = new ChunkedReader(text);
        using var console = new StringWriter();
        var runtime = new GameRuntime(diagnostics);
        await runtime.PumpOutputAsync(reader, console, identifier, "stderr");
        Assert.Equal(text, console.ToString());
        var retained = (await ReadArchiveAsync(diagnostics, identifier))["console-stderr.log"];
        Assert.DoesNotContain(token, retained);
        Assert.DoesNotContain(encodedToken, retained);
        Assert.EndsWith("[redacted] [redacted] end", retained);
    }

    [Fact]
    public async Task LateCollectionCannotOverwriteCompletedSessionReports()
    {
        var gameDirectory = Path.Combine(_directory, "minecraft");
        Directory.CreateDirectory(Path.Combine(gameDirectory, "debug"));
        var report = Path.Combine(gameDirectory, "debug", "disconnect-test.txt");
        var diagnostics = Create();
        var first = diagnostics.Begin("first", gameDirectory);
        await File.WriteAllTextAsync(report, "first report", TestContext.Current.CancellationToken);
        diagnostics.Complete(first);
        var endedAt = diagnostics.List()[0].EndedAt;
        diagnostics.Begin("second", gameDirectory);
        await File.WriteAllTextAsync(report, "second report", TestContext.Current.CancellationToken);
        diagnostics.Collect(first);
        diagnostics.Complete(first);
        Assert.Equal("first report", (await ReadArchiveAsync(diagnostics, first))["debug-disconnect-test.txt"]);
        Assert.Equal(endedAt, diagnostics.List().Single(session => session.SessionId == first).EndedAt);
    }

    private sealed class ChunkedReader(string text) : StringReader(text)
    {
        public override ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default) => base.ReadAsync(buffer[..Math.Min(buffer.Length, 47)], cancellationToken);
    }

    private static async Task<Dictionary<string, string>> ReadArchiveAsync(SessionDiagnostics diagnostics, Guid identifier)
    {
        var bytes = await diagnostics.DownloadAsync(identifier, CancellationToken.None);
        Assert.NotNull(bytes);
        using var archive = new ZipArchive(new MemoryStream(bytes));
        var files = new Dictionary<string, string>();
        foreach (var entry in archive.Entries)
        {
            using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
            files[entry.FullName] = await reader.ReadToEndAsync();
        }
        return files;
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
        GC.SuppressFinalize(this);
    }
}
