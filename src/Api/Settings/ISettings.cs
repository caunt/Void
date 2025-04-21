using System.Net;
using Microsoft.Extensions.Logging;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Settings;

public interface ISettings
{
    public IPAddress Address { get; }
    public int Port { get; }
    public int CompressionThreshold { get; }
    public int KickTimeout { get; }
    public LogLevel LogLevel { get; }
    public IEnumerable<IServer> Servers { get; }
}
