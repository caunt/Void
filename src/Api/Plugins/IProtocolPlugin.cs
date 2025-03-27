using Void.Minecraft.Network;

namespace Void.Proxy.Api.Plugins;

public interface IProtocolPlugin : IPlugin
{
    public static abstract IEnumerable<ProtocolVersion> SupportedVersions { get; }
}