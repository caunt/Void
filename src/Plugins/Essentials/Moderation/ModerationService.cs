using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Commands;
using Void.Proxy.API.Players;
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

                var reason = string.Join(' ', parts[2..]);
                var player = GetPlayerByUsername(parts[1]);

                if (player is null)
                    break;

                await player.KickAsync(reason, cancellationToken);
                break;
        }
    }

    private IPlayer? GetPlayerByUsername(string username)
    {
        return players.All.FirstOrDefault(player =>
        {
            var profile = player.Profile;

            if (profile is null)
                return false;

            return profile.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase);
        });
    }
}
