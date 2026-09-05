using System.Collections.Concurrent;
using Nito.AsyncEx;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Void.Client;

internal sealed class DiagnosticsOptions
{
    public string Directory { get; set; } = "/var/lib/void-client/diagnostics";
    public int MaximumSessions { get; set; } = 10;
    public int MaximumTotalMb { get; set; } = 256;
    public int MaximumSessionMb { get; set; } = 32;

    public static DiagnosticsOptions FromConfiguration(IConfiguration configuration)
    {
        var defaults = new DiagnosticsOptions();
        return new DiagnosticsOptions
        {
            Directory = configuration.GetValue<string>("VOID_DIAGNOSTICS_DIRECTORY") ?? defaults.Directory,
            MaximumSessions = configuration.GetValue("VOID_DIAGNOSTICS_MAXIMUM_SESSIONS", defaults.MaximumSessions),
            MaximumTotalMb = configuration.GetValue("VOID_DIAGNOSTICS_MAXIMUM_TOTAL_MB", defaults.MaximumTotalMb),
            MaximumSessionMb = configuration.GetValue("VOID_DIAGNOSTICS_MAXIMUM_SESSION_MB", defaults.MaximumSessionMb)
        };
    }
}

internal sealed record DiagnosticSession(Guid SessionId, string Launch, DateTimeOffset StartedAt, DateTimeOffset? EndedAt, GameStatus? Status, ClientFailure? LastFailure, IReadOnlyList<string> Warnings)
{
    public string DownloadUrl => $"/api/game/diagnostics/{SessionId}";
}

/// <summary>Owns bounded session evidence independently of the current game lifecycle.</summary>
internal sealed class SessionDiagnostics
{
    private const int MaximumFileBytes = 2 * 1024 * 1024;
    private const int ManifestReserveBytes = 256 * 1024;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    private readonly AsyncLock _retentionLock = new();
    private readonly AsyncLocal<Guid?> _context = new();
    private readonly ConcurrentDictionary<Guid, Session> _sessions = new();
    private readonly DiagnosticsOptions _options;
    private bool _initialized;
    private long _storedBytes;

    public SessionDiagnostics(DiagnosticsOptions options)
    {
        if (options.MaximumSessions < 1 || options.MaximumSessionMb < 1 || options.MaximumTotalMb < options.MaximumSessionMb)
            throw new ArgumentException("Diagnostics limits must be positive and the total must accommodate one session", nameof(options));
        _options = options;
    }

    public Guid? CurrentSessionId => _context.Value;

    public IDisposable Enter(Guid? identifier)
    {
        var previous = _context.Value;
        _context.Value = identifier;
        return new ContextScope(() => _context.Value = previous);
    }

    public async Task<Guid> BeginAsync(string launch, string minecraftDirectory, CancellationToken cancellationToken = default)
    {
        await InitializeAsync(cancellationToken);
        // Retention always acquires its lock before any session lock. Session operations never acquire it.
        await PruneAsync((long)_options.MaximumSessionMb * 1024 * 1024, cancellationToken);
        var identifier = Guid.NewGuid();
        var session = new Session(new(identifier, Limit(launch, 1024) ?? "", DateTimeOffset.UtcNow, null, null, null, []), Path.Combine(_options.Directory, identifier.ToString()), minecraftDirectory);
        using (await session.Lock.LockAsync(cancellationToken))
        {
            _sessions.TryAdd(identifier, session);
            await CollectReportsAsync(session, baseline: true, cancellationToken);
            await SaveManifestAsync(session, cancellationToken);
        }
        await PruneAsync(0, cancellationToken);
        return identifier;
    }

    public async Task RegisterSecretAsync(string secret, CancellationToken cancellationToken = default)
    {
        if (CurrentSessionId is not { } identifier || !_sessions.TryGetValue(identifier, out var session))
            return;
        using (await session.Lock.LockAsync(cancellationToken))
            session.Secrets.Add(secret);
    }

    public async Task<string> RedactAsync(Guid identifier, string value, CancellationToken cancellationToken = default)
    {
        if (!_sessions.TryGetValue(identifier, out var session))
            return value;
        using (await session.Lock.LockAsync(cancellationToken))
            return Redact(session, value);
    }

    public async Task RecordAsync(GameStatus status, CancellationToken cancellationToken = default)
    {
        if (status.SessionId is not { } identifier || !_sessions.TryGetValue(identifier, out var session))
            return;
        using (await session.Lock.LockAsync(cancellationToken))
        {
            if (!_sessions.ContainsKey(identifier))
                return;
            var original = status;
            status = status with
            {
                Message = Limit(status.Message),
                Error = Limit(status.Error),
                Failure = status.Failure is { } failure ? failure with { Message = Limit(failure.Message) ?? "", StackTrace = Limit(failure.StackTrace, 8192) ?? "" } : null,
                Warnings = status.Warnings.Take(8).Select(warning => Limit(warning, 256) ?? "").ToArray()
            };
            if (original.Error != status.Error || original.Failure?.StackTrace != status.Failure?.StackTrace)
                AddWarning(session, "Long failure details were truncated in retained metadata");
            status = JsonSerializer.Deserialize<GameStatus>(Redact(session, JsonSerializer.Serialize(status, JsonOptions)), JsonOptions) ?? status;
            session.Metadata = session.Metadata with { Status = status, LastFailure = status.Failure ?? session.Metadata.LastFailure };
            await WriteFileAsync(session, "operations.jsonl", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(status, JsonOptions) + "\n"), append: true, cancellationToken);
            await SaveManifestAsync(session, cancellationToken);
        }
    }

    public async Task WriteOutputAsync(Guid identifier, string stream, string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(text) || !_sessions.TryGetValue(identifier, out var session))
            return;
        using (await session.Lock.LockAsync(cancellationToken))
        {
            if (!_sessions.ContainsKey(identifier))
                return;
            await WriteFileAsync(session, $"console-{stream}.log", Encoding.UTF8.GetBytes(Redact(session, text)), append: true, cancellationToken);
        }
    }

    public async Task WarnAsync(Guid identifier, string warning, CancellationToken cancellationToken = default)
    {
        if (!_sessions.TryGetValue(identifier, out var session))
            return;
        using (await session.Lock.LockAsync(cancellationToken))
        {
            if (!_sessions.ContainsKey(identifier))
                return;
            AddWarning(session, warning);
            await SaveManifestAsync(session, cancellationToken);
        }
    }

    public async Task CollectAsync(Guid identifier, CancellationToken cancellationToken = default)
    {
        if (!_sessions.TryGetValue(identifier, out var session))
            return;
        using (await session.Lock.LockAsync(cancellationToken))
        {
            if (!_sessions.ContainsKey(identifier))
                return;
            if (session.Metadata.EndedAt is null)
                await CollectReportsAsync(session, baseline: false, cancellationToken);
            await SaveManifestAsync(session, cancellationToken);
        }
    }

    public async Task CompleteAsync(Guid identifier, CancellationToken cancellationToken = default)
    {
        if (!_sessions.TryGetValue(identifier, out var session))
            return;
        using (await session.Lock.LockAsync(cancellationToken))
        {
            if (!_sessions.ContainsKey(identifier))
                return;
            if (session.Metadata.EndedAt is null)
                await CollectReportsAsync(session, baseline: false, cancellationToken);
            session.Metadata = session.Metadata with { EndedAt = session.Metadata.EndedAt ?? DateTimeOffset.UtcNow };
            await SaveManifestAsync(session, cancellationToken);
        }
        await PruneAsync(0, cancellationToken);
    }

    public async Task SaveScreenshotAsync(Guid identifier, long operationId, byte[] screenshot, CancellationToken cancellationToken = default)
    {
        if (!_sessions.TryGetValue(identifier, out var session))
            return;
        using (await session.Lock.LockAsync(cancellationToken))
        {
            if (!_sessions.ContainsKey(identifier))
                return;
            await WriteFileAsync(session, $"failure-{operationId}.png", screenshot, append: false, cancellationToken);
            await SaveManifestAsync(session, cancellationToken);
        }
    }

    public async Task<IReadOnlyList<DiagnosticSession>> ListAsync(CancellationToken cancellationToken = default)
    {
        await InitializeAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        // Immutable metadata snapshots do not need to wait for a session's file I/O.
        return _sessions.Values.Select(session => session.Metadata).OrderByDescending(session => session.StartedAt).ToArray();
    }

    public async Task<byte[]?> DownloadAsync(Guid identifier, CancellationToken cancellationToken)
    {
        await InitializeAsync(cancellationToken);
        if (!_sessions.TryGetValue(identifier, out var session))
            return null;
        Dictionary<string, byte[]> files = [];
        using (await session.Lock.LockAsync(cancellationToken))
        {
            if (!_sessions.ContainsKey(identifier))
                return null;
            if (session.Metadata.EndedAt is null)
                await CollectReportsAsync(session, baseline: false, cancellationToken);
            try
            {
                if (System.IO.Directory.Exists(session.Directory))
                    foreach (var file in System.IO.Directory.EnumerateFiles(session.Directory))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (IsLink(file) || Path.GetFileName(file) == "session.json")
                            continue;
                        files[Path.GetFileName(file)] = await ReadTailAsync(file, MaximumFileBytes, cancellationToken);
                    }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                AddWarning(session, $"Some evidence could not be read: {exception.Message}");
            }
            files["session.json"] = Encoding.UTF8.GetBytes(Redact(session, JsonSerializer.Serialize(session.Metadata, JsonOptions)));
        }

        using var output = new MemoryStream();
        using (var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true))
            foreach (var (name, content) in files)
            {
                await using var destination = archive.CreateEntry(name, CompressionLevel.Fastest).Open();
                await destination.WriteAsync(content, cancellationToken);
            }
        return output.ToArray();
    }

    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (Volatile.Read(ref _initialized))
            return;
        using (await _retentionLock.LockAsync(cancellationToken))
        {
            if (_initialized)
                return;
            try
            {
                if (System.IO.Directory.Exists(_options.Directory))
                    foreach (var directory in System.IO.Directory.EnumerateDirectories(_options.Directory))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (!Guid.TryParse(Path.GetFileName(directory), out var identifier) || IsLink(directory) || _sessions.ContainsKey(identifier))
                            continue;
                        try
                        {
                            var manifest = Path.Combine(directory, "session.json");
                            if (IsLink(manifest))
                                continue;
                            var metadata = JsonSerializer.Deserialize<DiagnosticSession>(await File.ReadAllTextAsync(manifest, cancellationToken), JsonOptions);
                            if (metadata is null || metadata.SessionId != identifier)
                                continue;
                            var session = new Session(metadata with { EndedAt = metadata.EndedAt ?? DateTimeOffset.UtcNow }, directory, "");
                            foreach (var file in System.IO.Directory.EnumerateFiles(directory).Where(file => !IsLink(file)))
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                session.FileSizes[Path.GetFileName(file)] = new FileInfo(file).Length;
                            }
                            session.StoredBytes = session.FileSizes.Values.Sum();
                            if (_sessions.TryAdd(identifier, session))
                                Interlocked.Add(ref _storedBytes, session.StoredBytes);
                        }
                        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
                        {
                            Console.Error.WriteLine($"Could not load diagnostic session {identifier}: {exception.Message}");
                        }
                    }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                Console.Error.WriteLine($"Could not load diagnostic history: {exception.Message}");
            }
            Volatile.Write(ref _initialized, true);
        }
        await PruneAsync(0, cancellationToken);
    }

    private async Task CollectReportsAsync(Session session, bool baseline, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(session.MinecraftDirectory))
            return;
        try
        {
            foreach (var directoryName in new[] { "logs", "debug", "crash-reports" })
            {
                cancellationToken.ThrowIfCancellationRequested();
                var directory = Path.Combine(session.MinecraftDirectory, directoryName);
                if (!System.IO.Directory.Exists(directory) || IsLink(session.MinecraftDirectory) || IsLink(directory))
                    continue;
                foreach (var file in System.IO.Directory.EnumerateFiles(directory))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var name = Path.GetFileName(file);
                    if (IsLink(file) || !(directoryName == "logs" && name is "latest.log" or "debug.log"
                        || directoryName == "debug" && name.StartsWith("disconnect-", StringComparison.Ordinal) && name.EndsWith(".txt", StringComparison.Ordinal)
                        || directoryName == "crash-reports" && name.StartsWith("crash-", StringComparison.Ordinal) && name.EndsWith(".txt", StringComparison.Ordinal)))
                        continue;
                    var information = new FileInfo(file);
                    var fingerprint = (information.Length, information.LastWriteTimeUtc);
                    if (baseline)
                    {
                        session.Baseline[file] = fingerprint;
                        continue;
                    }
                    if (session.Baseline.TryGetValue(file, out var original) && original == fingerprint)
                        continue;
                    if (information.Length > MaximumFileBytes)
                        AddWarning(session, $"Truncated {directoryName}/{name} to its last {MaximumFileBytes} bytes");
                    var content = await ReadTailAsync(file, MaximumFileBytes, cancellationToken);
                    await WriteFileAsync(session, $"{directoryName}-{name}", Encoding.UTF8.GetBytes(Redact(session, Encoding.UTF8.GetString(content))), append: false, cancellationToken);
                }
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            AddWarning(session, $"Report collection failed: {exception.Message}");
        }
    }

    private async Task WriteFileAsync(Session session, string name, byte[] bytes, bool append, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        long reservedBytes = 0;
        var path = Path.Combine(session.Directory, name);
        var originalLength = session.FileSizes.GetValueOrDefault(name);
        try
        {
            if (IsLink(session.Directory) || IsLink(path))
                throw new IOException("Diagnostic output is a symbolic link");
            System.IO.Directory.CreateDirectory(session.Directory);
            if (bytes.Length > MaximumFileBytes)
            {
                AddWarning(session, $"Omitted or truncated {name}: diagnostic file size limit");
                if (!append)
                    return;
                bytes = bytes[^MaximumFileBytes..];
            }
            if (append && originalLength + bytes.Length > MaximumFileBytes && File.Exists(path))
            {
                var previousName = $"previous-{name}";
                File.Move(path, Path.Combine(session.Directory, previousName), overwrite: true);
                var removedBytes = session.FileSizes.GetValueOrDefault(previousName);
                session.FileSizes[previousName] = originalLength;
                session.FileSizes[name] = 0;
                session.StoredBytes -= removedBytes;
                Interlocked.Add(ref _storedBytes, -removedBytes);
                originalLength = 0;
                AddWarning(session, $"Older {name} output was rotated; only recent output is retained");
            }
            var additionalBytes = append ? bytes.Length : Math.Max(0, bytes.Length - originalLength);
            var reserve = name == "session.json" ? 0 : ManifestReserveBytes;
            if (session.StoredBytes + additionalBytes > (long)_options.MaximumSessionMb * 1024 * 1024 - reserve || !Reserve(additionalBytes, reserve, cancellationToken))
            {
                AddWarning(session, "Diagnostic size limit reached; additional evidence was omitted");
                return;
            }
            reservedBytes = additionalBytes;
            await using var stream = new FileStream(path, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read, 81920, useAsync: true);
            await stream.WriteAsync(bytes, cancellationToken);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            AddWarning(session, $"Could not store {name}: {exception.Message}");
        }
        finally
        {
            // Reconcile reservations even after cancellation or a partial write.
            var actualLength = originalLength;
            try
            {
                actualLength = File.Exists(path) && !IsLink(path) ? new FileInfo(path).Length : 0;
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                AddWarning(session, $"Could not measure {name}: {exception.Message}");
            }
            var difference = actualLength - originalLength;
            session.FileSizes[name] = actualLength;
            session.StoredBytes += difference;
            Interlocked.Add(ref _storedBytes, difference - reservedBytes);
        }
    }

    private bool Reserve(long bytes, int manifestReserve, CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var total = Volatile.Read(ref _storedBytes);
            if (total + bytes > (long)_options.MaximumTotalMb * 1024 * 1024 - manifestReserve)
                return false;
            if (Interlocked.CompareExchange(ref _storedBytes, total + bytes, total) == total)
                return true;
        }
    }

    private Task SaveManifestAsync(Session session, CancellationToken cancellationToken) => WriteFileAsync(session, "session.json", Encoding.UTF8.GetBytes(Redact(session, JsonSerializer.Serialize(session.Metadata, JsonOptions))), append: false, cancellationToken);

    private async Task PruneAsync(long reserveBytes, CancellationToken cancellationToken)
    {
        using (await _retentionLock.LockAsync(cancellationToken))
            foreach (var session in _sessions.Values.Where(session => session.Metadata.EndedAt is not null).OrderBy(session => session.Metadata.StartedAt).ToArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_sessions.Count <= _options.MaximumSessions && Volatile.Read(ref _storedBytes) + reserveBytes <= (long)_options.MaximumTotalMb * 1024 * 1024)
                    break;
                using (await session.Lock.LockAsync(cancellationToken))
                {
                    try
                    {
                        if (System.IO.Directory.Exists(session.Directory) && !IsLink(session.Directory))
                            foreach (var file in System.IO.Directory.EnumerateFiles(session.Directory))
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                File.Delete(file);
                                var removedBytes = session.FileSizes.GetValueOrDefault(Path.GetFileName(file));
                                session.FileSizes.Remove(Path.GetFileName(file));
                                session.StoredBytes -= removedBytes;
                                Interlocked.Add(ref _storedBytes, -removedBytes);
                            }
                        if (System.IO.Directory.Exists(session.Directory) && !IsLink(session.Directory))
                            System.IO.Directory.Delete(session.Directory);
                        if (_sessions.TryRemove(session.Metadata.SessionId, out _))
                            Interlocked.Add(ref _storedBytes, -session.StoredBytes);
                    }
                    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                    {
                        AddWarning(session, $"Could not expire session: {exception.Message}");
                    }
                }
            }
    }

    private static string Redact(Session session, string value)
    {
        foreach (var secret in session.Secrets)
            value = value.Replace(secret, "[redacted]", StringComparison.Ordinal);
        return value;
    }

    private static string? Limit(string? value, int maximumCharacters = 2048) => value?.Length > maximumCharacters ? value[..maximumCharacters] + " [truncated]" : value;

    private static void AddWarning(Session session, string warning)
    {
        warning = Limit(warning, 256) ?? "";
        if (session.Metadata.Warnings.Count < 64 && !session.Metadata.Warnings.Contains(warning))
            session.Metadata = session.Metadata with { Warnings = [.. session.Metadata.Warnings, warning] };
    }

    private static bool IsLink(string path) => (File.Exists(path) || System.IO.Directory.Exists(path)) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0;

    private static async Task<byte[]> ReadTailAsync(string path, int maximumBytes, CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 81920, useAsync: true);
        stream.Seek(Math.Max(0, stream.Length - maximumBytes), SeekOrigin.Begin);
        var bytes = new byte[(int)Math.Min(maximumBytes, stream.Length)];
        var count = 0;
        while (count < bytes.Length)
        {
            var read = await stream.ReadAsync(bytes.AsMemory(count), cancellationToken);
            if (read == 0)
                break;
            count += read;
        }
        return bytes[..count];
    }

    private sealed class Session(DiagnosticSession metadata, string directory, string minecraftDirectory)
    {
        private DiagnosticSession _metadata = metadata;
        public AsyncLock Lock { get; } = new();
        public DiagnosticSession Metadata
        {
            get => Volatile.Read(ref _metadata);
            set => Volatile.Write(ref _metadata, value);
        }
        public string Directory { get; } = directory;
        public string MinecraftDirectory { get; } = minecraftDirectory;
        public long StoredBytes { get; set; }
        public Dictionary<string, long> FileSizes { get; } = [];
        public Dictionary<string, (long, DateTime)> Baseline { get; } = [];
        public List<string> Secrets { get; } = [];
    }

    private sealed class ContextScope(Action restore) : IDisposable
    {
        public void Dispose() => restore();
    }
}
