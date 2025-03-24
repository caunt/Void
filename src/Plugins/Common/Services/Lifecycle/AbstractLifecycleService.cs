﻿using System.Text.Json.Nodes;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Chat;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Events;

namespace Void.Proxy.Plugins.Common.Services.Lifecycle;

public abstract class AbstractLifecycleService : IPluginCommonService
{
    [Subscribe]
    public async ValueTask OnChatMessageSend(ChatMessageSendEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        await SendChatMessageAsync(@event.Player, @event.Text, cancellationToken);
    }

    [Subscribe(PostOrder.Last)]
    public async ValueTask OnPlayerKickEvent(PlayerKickEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        var reason = string.IsNullOrWhiteSpace(@event.Reason) ? "You were kicked from proxy" : @event.Reason;

        if (@event.Player.ProtocolVersion < ProtocolVersion.MINECRAFT_1_20_3)
        {
            try
            {
                JsonNode.Parse(reason);
            }
            catch
            {
                reason = $"{{\"text\":\"{reason}\"}}";
            }
        }

        @event.Result = await KickPlayerAsync(@event.Player, reason, cancellationToken);
    }

    [Subscribe]
    public async ValueTask OnPlayerVerifiedEncryption(PlayerVerifiedEncryptionEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Link.Player.ProtocolVersion))
            return;

        await EnableCompressionAsync(@event.Link, cancellationToken);
    }

    protected abstract ValueTask EnableCompressionAsync(ILink link, CancellationToken cancellationToken);
    protected abstract ValueTask<bool> KickPlayerAsync(IPlayer player, string reason, CancellationToken cancellationToken);
    protected abstract ValueTask<bool> SendChatMessageAsync(IPlayer player, string text, CancellationToken cancellationToken);
    protected abstract bool IsSupportedVersion(ProtocolVersion version);
}
