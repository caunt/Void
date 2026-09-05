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
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    private readonly object _gate = new();
    private readonly AsyncLocal<Guid?> _context = new();
    private readonly Dictionary<Guid, Session> _sessions = [];
    private readonly DiagnosticsOptions _options;

    public SessionDiagnostics(DiagnosticsOptions options)
    {
        if (options.MaximumSessions < 1 || options.MaximumSessionMb < 1 || options.MaximumTotalMb < options.MaximumSessionMb)
            throw new ArgumentException("Diagnostics limits must be positive and the total must accommodate one session", nameof(options));

        _options = options;
        // Loading only our manifest-named directories also allows an optional persistent volume.
        if (!System.IO.Directory.Exists(options.Directory))
            return;

        foreach (var directory in System.IO.Directory.EnumerateDirectories(options.Directory))
        {
            if (!Guid.TryParse(Path.GetFileName(directory), out var identifier) || IsLink(directory))
                continue;

            try
            {
                var manifest = Path.Combine(directory, "session.json");
                if (IsLink(manifest))
                    continue;
                var metadata = JsonSerializer.Deserialize<DiagnosticSession>(File.ReadAllText(manifest), JsonOptions);
                if (metadata is null || metadata.SessionId != identifier)
                    continue;
                var session = new Session(metadata with { EndedAt = metadata.EndedAt ?? DateTimeOffset.UtcNow }, directory, "");
                _sessions.Add(identifier, session);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
            {
                Console.Error.WriteLine($"Could not load diagnostic session {identifier}: {exception.Message}");
            }
        }

        Prune();
    }

    public Guid? CurrentSessionId => _context.Value;

    public IDisposable Enter(Guid? identifier)
    {
        var previous = _context.Value;
        _context.Value = identifier;
        return new ContextScope(() => _context.Value = previous);
    }

    public Guid Begin(string launch, string minecraftDirectory)
    {
        lock (_gate)
        {
            var identifier = Guid.NewGuid();
            var session = new Session(new(identifier, Limit(launch, 1024) ?? "", DateTimeOffset.UtcNow, null, null, null, []), Path.Combine(_options.Directory, identifier.ToString()), minecraftDirectory);
            _sessions.Add(identifier, session);
            TryCollect(session, baseline: true);
            SaveManifest(session);
            Prune();
            return identifier;
        }
    }

    public void RegisterSecret(string secret)
    {
        lock (_gate)
            if (CurrentSessionId is { } identifier && _sessions.TryGetValue(identifier, out var session))
                session.Secrets.Add(secret);
    }

    public string Redact(Guid identifier, string value)
    {
        lock (_gate)
        {
            if (!_sessions.TryGetValue(identifier, out var session))
                return value;
            foreach (var secret in session.Secrets)
                value = value.Replace(secret, "[redacted]", StringComparison.Ordinal);
            return value;
        }
    }

    public void Record(GameStatus status)
    {
        lock (_gate)
        {
            if (status.SessionId is not { } identifier || !_sessions.TryGetValue(identifier, out var session))
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
            status = JsonSerializer.Deserialize<GameStatus>(Redact(identifier, JsonSerializer.Serialize(status, JsonOptions)), JsonOptions) ?? status;
            session.Metadata = session.Metadata with { Status = status, LastFailure = status.Failure ?? session.Metadata.LastFailure };
            Append(session, "operations.jsonl", Encoding.UTF8.GetBytes(Redact(identifier, JsonSerializer.Serialize(status, JsonOptions)) + "\n"));
            SaveManifest(session);
        }
    }

    public void WriteOutput(Guid identifier, string stream, string text)
    {
        if (string.IsNullOrEmpty(text))
            return;
        lock (_gate)
            if (_sessions.TryGetValue(identifier, out var session))
                Append(session, $"console-{stream}.log", Encoding.UTF8.GetBytes(Redact(identifier, text)));
    }

    public void Warn(Guid identifier, string warning)
    {
        lock (_gate)
            if (_sessions.TryGetValue(identifier, out var session))
            {
                AddWarning(session, warning);
                SaveManifest(session);
            }
    }

    public void Collect(Guid identifier)
    {
        lock (_gate)
            if (_sessions.TryGetValue(identifier, out var session))
            {
                if (session.Metadata.EndedAt is null)
                    TryCollect(session, baseline: false);
                SaveManifest(session);
            }
    }

    public void Complete(Guid identifier)
    {
        lock (_gate)
            if (_sessions.TryGetValue(identifier, out var session))
            {
                if (session.Metadata.EndedAt is null)
                    TryCollect(session, baseline: false);
                session.Metadata = session.Metadata with { EndedAt = session.Metadata.EndedAt ?? DateTimeOffset.UtcNow };
                SaveManifest(session);
                Prune();
            }
    }

    public void SaveScreenshot(Guid identifier, long operationId, byte[] screenshot)
    {
        lock (_gate)
            if (_sessions.TryGetValue(identifier, out var session))
            {
                SaveFile(session, $"failure-{operationId}.png", screenshot);
                SaveManifest(session);
            }
    }

    public IReadOnlyList<DiagnosticSession> List()
    {
        lock (_gate)
            return _sessions.Values.OrderByDescending(session => session.Metadata.StartedAt).Select(session => session.Metadata).ToArray();
    }

    public async Task<byte[]?> DownloadAsync(Guid identifier, CancellationToken cancellationToken)
    {
        Dictionary<string, byte[]> files = [];
        lock (_gate)
        {
            if (!_sessions.TryGetValue(identifier, out var session))
                return null;
            if (session.Metadata.EndedAt is null)
                TryCollect(session, baseline: false);
            try
            {
                if (System.IO.Directory.Exists(session.Directory))
                    foreach (var file in System.IO.Directory.EnumerateFiles(session.Directory))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (IsLink(file) || Path.GetFileName(file) == "session.json")
                            continue;
                        files[Path.GetFileName(file)] = ReadTail(file, MaximumFileBytes);
                    }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                AddWarning(session, $"Some evidence could not be read: {exception.Message}");
            }
            files["session.json"] = Encoding.UTF8.GetBytes(Redact(identifier, JsonSerializer.Serialize(session.Metadata, JsonOptions)));
        }

        // Copy under the lock, then compress without holding up logging or lifecycle changes.
        using var output = new MemoryStream();
        using (var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true))
            foreach (var (name, content) in files)
            {
                await using var destination = archive.CreateEntry(name, CompressionLevel.Fastest).Open();
                await destination.WriteAsync(content, cancellationToken);
            }
        return output.ToArray();
    }

    private void TryCollect(Session session, bool baseline)
    {
        if (string.IsNullOrEmpty(session.MinecraftDirectory))
            return;
        try
        {
            foreach (var directoryName in new[] { "logs", "debug", "crash-reports" })
            {
                var directory = Path.Combine(session.MinecraftDirectory, directoryName);
                if (!System.IO.Directory.Exists(directory) || IsLink(session.MinecraftDirectory) || IsLink(directory))
                    continue;
                foreach (var file in System.IO.Directory.EnumerateFiles(directory))
                {
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
                    SaveFile(session, $"{directoryName}-{name}", Encoding.UTF8.GetBytes(Redact(session.Metadata.SessionId, Encoding.UTF8.GetString(ReadTail(file, MaximumFileBytes)))));
                }
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            AddWarning(session, $"Report collection failed: {exception.Message}");
        }
    }

    private void Append(Session session, string name, byte[] bytes)
    {
        try
        {
            EnsureDirectory(session);
            var path = Path.Combine(session.Directory, name);
            if (IsLink(path))
                throw new IOException("Diagnostic output is a symbolic link");
            if (bytes.Length > MaximumFileBytes)
                AddWarning(session, $"Truncated {name} to its last {MaximumFileBytes} bytes");
            if (File.Exists(path) && new FileInfo(path).Length + bytes.Length > MaximumFileBytes)
            {
                File.Move(path, Path.Combine(session.Directory, $"previous-{name}"), overwrite: true);
                AddWarning(session, $"Older {name} output was rotated; only recent output is retained");
            }
            if (!Fits(session, Math.Min(bytes.Length, MaximumFileBytes)))
                return;
            using var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            stream.Write(bytes.AsSpan(Math.Max(0, bytes.Length - MaximumFileBytes)));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            AddWarning(session, $"Could not store {name}: {exception.Message}");
        }
    }

    private void SaveFile(Session session, string name, byte[] bytes)
    {
        try
        {
            EnsureDirectory(session);
            var path = Path.Combine(session.Directory, name);
            if (IsLink(path))
                throw new IOException("Diagnostic output is a symbolic link");
            var existingLength = File.Exists(path) ? new FileInfo(path).Length : 0;
            if (bytes.Length > MaximumFileBytes || !Fits(session, bytes.Length - existingLength))
            {
                AddWarning(session, $"Omitted {name}: diagnostic size limit");
                return;
            }
            File.WriteAllBytes(path, bytes);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            AddWarning(session, $"Could not store {name}: {exception.Message}");
        }
    }

    private bool Fits(Session session, long additionalBytes)
    {
        // Reserve space for the manifest even after noisy output exhausts the evidence budget.
        if (Size(session) + additionalBytes > (long)_options.MaximumSessionMb * 1024 * 1024 - 262144)
        {
            AddWarning(session, "Session size limit reached; additional evidence was omitted");
            return false;
        }
        Prune(additionalBytes, session.Metadata.SessionId);
        if (_sessions.Values.Sum(Size) + additionalBytes > (long)_options.MaximumTotalMb * 1024 * 1024 - 262144)
        {
            AddWarning(session, "Total size limit reached; additional evidence was omitted");
            return false;
        }
        return true;
    }

    private void SaveManifest(Session session)
    {
        try
        {
            EnsureDirectory(session);
            var path = Path.Combine(session.Directory, "session.json");
            if (IsLink(path))
                throw new IOException("Diagnostic manifest is a symbolic link");
            File.WriteAllText(path, Redact(session.Metadata.SessionId, JsonSerializer.Serialize(session.Metadata, JsonOptions)));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            AddWarning(session, $"Could not store session manifest: {exception.Message}");
        }
    }

    private void Prune(long additionalBytes = 0, Guid? protectedSession = null)
    {
        foreach (var session in _sessions.Values.Where(session => session.Metadata.EndedAt is not null && session.Metadata.SessionId != protectedSession).OrderBy(session => session.Metadata.StartedAt).ToArray())
        {
            if (_sessions.Count <= _options.MaximumSessions && _sessions.Values.Sum(Size) + additionalBytes <= (long)_options.MaximumTotalMb * 1024 * 1024 - 262144)
                break;
            try
            {
                if (System.IO.Directory.Exists(session.Directory) && !IsLink(session.Directory))
                    System.IO.Directory.Delete(session.Directory, recursive: true);
                _sessions.Remove(session.Metadata.SessionId);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                AddWarning(session, $"Could not expire session: {exception.Message}");
            }
        }
    }

    private static string? Limit(string? value, int maximumCharacters = 2048) => value?.Length > maximumCharacters ? value[..maximumCharacters] + " [truncated]" : value;

    private static void AddWarning(Session session, string warning)
    {
        warning = Limit(warning, 256) ?? "";
        if (session.Metadata.Warnings.Count < 64 && !session.Metadata.Warnings.Contains(warning))
            session.Metadata = session.Metadata with { Warnings = [.. session.Metadata.Warnings, warning] };
    }

    private static void EnsureDirectory(Session session)
    {
        if (IsLink(session.Directory))
            throw new IOException("Diagnostic directory is a symbolic link");
        System.IO.Directory.CreateDirectory(session.Directory);
    }

    private static bool IsLink(string path) => (File.Exists(path) || System.IO.Directory.Exists(path)) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0;

    private static long Size(Session session)
    {
        try
        {
            return System.IO.Directory.Exists(session.Directory)
                ? System.IO.Directory.EnumerateFiles(session.Directory).Where(file => !IsLink(file)).Sum(file => new FileInfo(file).Length)
                : 0;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return 0;
        }
    }

    private static byte[] ReadTail(string path, int maximumBytes)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        stream.Seek(Math.Max(0, stream.Length - maximumBytes), SeekOrigin.Begin);
        var bytes = new byte[(int)Math.Min(maximumBytes, stream.Length)];
        var count = 0;
        while (count < bytes.Length)
        {
            var read = stream.Read(bytes.AsSpan(count));
            if (read == 0)
                break;
            count += read;
        }
        return bytes[..count];
    }

    private sealed class Session(DiagnosticSession metadata, string directory, string minecraftDirectory)
    {
        public DiagnosticSession Metadata { get; set; } = metadata;
        public string Directory { get; } = directory;
        public string MinecraftDirectory { get; } = minecraftDirectory;
        public Dictionary<string, (long, DateTime)> Baseline { get; } = [];
        public List<string> Secrets { get; } = [];
    }

    private sealed class ContextScope(Action restore) : IDisposable
    {
        public void Dispose() => restore();
    }
}
