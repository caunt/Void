using Void.Proxy.Api.Events.Commands;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Common.Services.Commands;

public abstract class AbstractCommandService(IEventService events) : IPluginCommonService
{
    public async ValueTask<bool> HandleCommandAsync(ILink link, string command, bool isSigned, CancellationToken cancellationToken)
    {
        var cancelled = await events.ThrowWithResultAsync(new ChatCommandEvent(link, command, isSigned), cancellationToken);

        if (isSigned && cancelled)
            throw new InvalidOperationException("Signed chat commands cannot be cancelled");

        return cancelled;
    }
}
