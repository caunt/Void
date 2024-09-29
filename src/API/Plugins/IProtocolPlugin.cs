using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.API.Plugins;

public interface IProtocolPlugin : IPlugin
{
    public static abstract IEnumerable<ProtocolVersion> SupportedVersions { get; }
}