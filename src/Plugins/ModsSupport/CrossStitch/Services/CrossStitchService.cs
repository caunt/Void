using Microsoft.Extensions.Logging;
using Void.Minecraft.Commands.Brigadier.Registry;
using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;

namespace Void.Proxy.Plugins.ModsSupport.CrossStitch.Services;

public class CrossStitchService(ILogger<CrossStitchService> logger) : IEventListener
{
    public static readonly ArgumentSerializerMapping ModArgumentMapping = new("crossstitch:mod_argument", ProtocolVersion.MINECRAFT_1_19, parserId: -256);

    [Subscribe]
    public void OnProxyStarting(ProxyStartingEvent @event)
    {
        logger.LogTrace("Registering CrossStitch mod argument type serializer...");
        ArgumentSerializerRegistry.Register(ModArgumentMapping, typeof(CrossStitchModArgumentType), CrossStitchModArgumentSerializer.Instance);
    }
}
