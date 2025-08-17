namespace Void.Proxy.Api;

public interface IRunOptions
{
    public string WorkingDirectory { get; }
    public string[] Arguments { get; }
}
