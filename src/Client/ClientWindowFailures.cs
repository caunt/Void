namespace Void.Client;

internal sealed class ExternalProcessException(string fileName, IReadOnlyList<string> arguments, int exitCode, string standardOutput, string standardError)
    : Exception($"{fileName} exited with code {exitCode}: {standardError}")
{
    public string FileName { get; } = fileName;
    public IReadOnlyList<string> Arguments { get; } = arguments;
    public int ExitCode { get; } = exitCode;
    public string StandardOutput { get; } = standardOutput;
    public string StandardError { get; } = standardError;
}

internal readonly record struct MinecraftWindowLease(string Id, long Generation);

internal sealed class StaleMinecraftWindowException(MinecraftWindowLease lease, Exception innerException)
    : Exception($"Minecraft window lease {lease.Generation} ({lease.Id}) is stale", innerException)
{
    public MinecraftWindowLease Lease { get; } = lease;
}

internal static class X11FailureClassifier
{
    public static bool IsExplicitStaleWindow(ExternalProcessException exception)
    {
        return exception.FileName is "xdotool"
            && exception.StandardError.Contains("BadWindow", StringComparison.Ordinal)
            && exception.StandardError.Contains("invalid Window parameter", StringComparison.Ordinal);
    }
}
