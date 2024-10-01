using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Authentication;
using Void.Proxy.API.Events.Services;
using Void.Proxy.Common.Services;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Services;

public class AuthenticationService(ILogger<AuthenticationService> logger, IEventService events) : IPluginService
{
    [Subscribe]
    public void OnAuthenticationStarting(AuthenticationStartingEvent @event)
    {
        if (Plugin.SupportedVersions.Contains(@event.Link.Player.ProtocolVersion))
            @event.Result = AuthenticationSide.Server; // TODO: change to Proxy side
    }

    [Subscribe]
    public async ValueTask OnAuthenticationStarted(AuthenticationStartedEvent @event, CancellationToken cancellationToken)
    {
        if (!Plugin.SupportedVersions.Contains(@event.Link.Player.ProtocolVersion))
            return;

        if (@event.Side is AuthenticationSide.Server)
            return;

        await ValueTask.CompletedTask;
    }
}