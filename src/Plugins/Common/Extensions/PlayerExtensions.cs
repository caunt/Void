using Microsoft.Extensions.DependencyInjection;
using Void.Common.Network;
using Void.Minecraft.Network;
using Void.Proxy.Api.Events.Minecraft;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Plugins.Common.Extensions;

public static class PlayerExtensions
{
    public static async ValueTask SetPhaseAsync(this IPlayer player, Side side, Phase phase, IMinecraftChannel channel, CancellationToken cancellationToken)
    {
        if (side is Side.Client)
            player.Phase = phase;

        var events = player.Context.Services.GetRequiredService<IEventService>();
        await events.ThrowAsync(new PhaseChangedEvent(player, side, channel, phase), cancellationToken);
    }
}
