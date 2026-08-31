using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Binary;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Network;
using Void.Proxy.Plugins.Common.Network.Entities;

namespace Void.Proxy.Plugins.Common.Services.Entities;

public abstract class AbstractEntityIdService<TEntityIdState> : IPluginCommonService where TEntityIdState : EntityIdState
{
    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        if (@event.Player.Phase is not Phase.Play)
            return;

        if (@event.Message is not IMinecraftBinaryMessage message)
            return;

        var state = @event.Player.Context.Services.GetRequiredService<TEntityIdState>();

        if (!state.TryGetIds(out var clientEntityId, out var serverEntityId) || clientEntityId == serverEntityId)
            return;

        LegacyEntityIdRewriter.Rewrite(message, @event.Player.ProtocolVersion, @event.Direction, serverEntityId, clientEntityId);
    }

    protected abstract bool IsSupportedVersion(ProtocolVersion protocolVersion);
}
