using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;

namespace Void.Proxy.API.Plugins;

public interface IProtocolPlugin : IPlugin
{
    public static abstract IEnumerable<ProtocolVersion> SupportedVersions { get; }
}