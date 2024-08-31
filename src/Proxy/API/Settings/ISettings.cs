using System.Net;
using Microsoft.Extensions.Logging;
using Void.Proxy.API.Forwarding;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Settings;

public interface ISettings
{
    public int ConfigVersion { get; }
    public IPAddress Address { get; }
    public int Port { get; }
    public int CompressionThreshold { get; }
    public LogLevel LogLevel { get; }
    public IForwarding Forwarding { get; }
    public List<IServer> Servers { get; }

    public ValueTask LoadAsync(string fileName = "settings.ini", bool createDefault = true, CancellationToken cancellationToken = default);

    public ValueTask SaveAsync(string fileName = "settings.ini", CancellationToken cancellationToken = default);
}