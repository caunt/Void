using Microsoft.Extensions.Logging;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Network;
using Void.Proxy.Plugins.ForwardingSupport.ProxyCompatibleForge.Packets;

namespace Void.Proxy.Plugins.ForwardingSupport.ProxyCompatibleForge.Services;

public class CompatibilityService(ILogger<CompatibilityService> logger) : IEventListener
{
    [Subscribe]
    public static void OnPhaseChanged(PhaseChangedEvent @event)
    {
        if (@event.Phase is not Phase.Play)
            return;

        @event.Player.RegisterPacket<CommandsPacket>(
            @event.Channel,
            @event.Side switch
            {
                Side.Client => Operation.Write,
                Side.Server => Operation.Read,
                _ => throw new InvalidOperationException($"Invalid side changed phase: {@event.Side}")
            },
            PacketIdDefinitions.ClientboundCommands);
    }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        if (@event.Message is not CommandsPacket packet)
            return;

        Console.WriteLine(@event);
        logger.LogDebug("Received CommandsPacket: {Packet}", packet);
    }
}
