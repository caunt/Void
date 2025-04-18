using Void.Minecraft.Network;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.Common.Plugins;

public interface IProtocolPlugin : IPlugin
{
    public static abstract IEnumerable<ProtocolVersion> SupportedVersions { get; }
}
