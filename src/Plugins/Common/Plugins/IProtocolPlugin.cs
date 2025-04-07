using Void.Common.Plugins;
using Void.Minecraft.Network;

namespace Void.Proxy.Plugins.Common.Plugins;

public interface IProtocolPlugin : IPlugin
{
    public static abstract IEnumerable<ProtocolVersion> SupportedVersions { get; }
}
