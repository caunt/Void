using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Authentication;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Links;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.Common.Services;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Services;

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
        if (@event.Side is AuthenticationSide.Server)
            return;

        var message = await @event.Link.PlayerChannel.ReadMessageAsync(cancellationToken);

        if (message is HandshakePacket handshake)
            await SimulateMessageFlowAsync(Direction.Serverbound, @event.Link, handshake, cancellationToken);

        if (message is LoginStartPacket loginStart)
            await SimulateMessageFlowAsync(Direction.Serverbound, @event.Link, loginStart, cancellationToken);

        logger.LogInformation("{MessageType}", message.ToString());

        // this will get stuck after login start packet
        await OnAuthenticationStarted(@event, cancellationToken);
    }

    public async ValueTask SimulateMessageFlowAsync(Direction direction, ILink link, IMinecraftMessage message, CancellationToken cancellationToken)
    {
        var cancelled = await events.ThrowWithResultAsync(new MessageReceivedEvent
        {
            Direction = Direction.Serverbound,
            From = Side.Client,
            To = Side.Server,
            Link = link,
            Message = message
        }, cancellationToken);

        if (cancelled)
            throw new NotSupportedException("Cancelling incoming packets at authentication on Proxy side is not supported yet");

        await events.ThrowAsync(new MessageSentEvent
        {
            Direction = Direction.Serverbound,
            From = Side.Client,
            To = Side.Server,
            Link = link,
            Message = message
        }, cancellationToken);
    }
}