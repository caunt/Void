using Microsoft.Extensions.DependencyInjection;
using Void.Common.Network;
using Void.Common.Network.Channels;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Proxy.Api.Events.Services;

namespace Void.Proxy.Plugins.Common.Extensions;

public static class PlayerExtensions
{
    public static async ValueTask SetPhaseAsync(this IMinecraftPlayer player, Side side, Phase phase, INetworkChannel channel, CancellationToken cancellationToken)
    {
        if (side is Side.Client)
            player.Phase = phase;

        var events = player.Context.Services.GetRequiredService<IEventService>();
        await events.ThrowAsync(new PhaseChangedEvent(player, side, channel, phase), cancellationToken);
    }
}
