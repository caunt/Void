using Microsoft.Extensions.Logging;
using System.Net;
using Void.Proxy.Api.Forwarding;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Settings;

public interface ISettings
{
    public int ConfigVersion { get; }
    public IPAddress Address { get; }
    public int Port { get; }
    public int CompressionThreshold { get; }
    public int KickTimeout { get; }
    public LogLevel LogLevel { get; }
    public IForwarding Forwarding { get; }
    public List<IServer> Servers { get; }

    public ValueTask LoadAsync(string fileName = "settings.ini", bool createDefault = true, CancellationToken cancellationToken = default);

    public ValueTask SaveAsync(string fileName = "settings.ini", CancellationToken cancellationToken = default);
}
