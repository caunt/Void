using Void.Common.Players;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Commands;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Essentials.Moderation;

public class ModerationService(IPlayerService players) : IPluginCommonService
{
    [Subscribe]
    public async ValueTask OnChatCommand(ChatCommandEvent @event, CancellationToken cancellationToken)
    {
        var parts = @event.Command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is 0)
            return;

        switch (parts[0].ToLower())
        {
            case "kick":
                @event.Result = true;

                if (parts.Length is 1)
                    break;

                var reason = parts.Length > 2 ? (Component?)string.Join(' ', parts[2..]) : null;
                var player = GetPlayerByUsername(parts[1]);

                if (player is null)
                    break;

                if (!player.TryGetMinecraftPlayer(out var minecraftPlayer))
                    break;

                await minecraftPlayer.KickAsync(reason, cancellationToken);
                break;
        }
    }

    private IPlayer? GetPlayerByUsername(string username)
    {
        return players.All.FirstOrDefault(player =>
        {
            if (!player.TryGetMinecraftPlayer(out var minecraftPlayer))
                return false;

            var profile = minecraftPlayer.Profile;

            if (profile is null)
                return false;

            return profile.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase);
        });
    }
}
