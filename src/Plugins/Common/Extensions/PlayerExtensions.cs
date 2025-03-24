using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Events.Minecraft;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Players;

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
