using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Commands;
using Void.Proxy.API.Plugins.Services;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Essentials.Platform;

public class PlatformService(IPluginService plugins) : IPluginCommonService
{
    [Subscribe]
    public async ValueTask OnChatCommand(ChatCommandEvent @event, CancellationToken cancellationToken)
    {
        var parts = @event.Command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is 0)
            return;

        switch (parts[0].ToLower())
        {
            case "unload":
                if (parts.Length is 1)
                    break;

                await plugins.UnloadPluginAsync(parts[1], cancellationToken);
                break;
        }
    }
}
