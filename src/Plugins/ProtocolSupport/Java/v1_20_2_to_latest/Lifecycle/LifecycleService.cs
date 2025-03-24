using Void.Proxy.Api.Links;
using Void.Proxy.Api.Links.Extensions;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Channels.Extensions;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.Common.Services.Lifecycle;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Extensions;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Lifecycle;

public class LifecycleService : AbstractLifecycleService
{
    protected override async ValueTask EnableCompressionAsync(ILink link, CancellationToken cancellationToken)
    {
        await link.SendPacketAsync(new SetCompressionPacket { Threshold = 256 }, cancellationToken);
    }

    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }

    protected override async ValueTask<bool> SendChatMessageAsync(IPlayer player, string text, CancellationToken cancellationToken)
    {
        if (!await player.IsPlayingAsync(cancellationToken))
            return false;

        var channel = await player.GetChannelAsync(cancellationToken);
        await channel.SendPacketAsync(new SystemChatMessagePacket { Message = text, Overlay = false }, cancellationToken);

        return true;
    }

    protected override async ValueTask<bool> KickPlayerAsync(IPlayer player, string reason, CancellationToken cancellationToken)
    {
        var channel = await player.GetChannelAsync(cancellationToken);

        if (await player.IsPlayingAsync(cancellationToken))
        {
            await channel.SendPacketAsync(new PlayDisconnectPacket { Reason = reason }, cancellationToken);
        }
        else if (await player.IsConfiguringAsync(cancellationToken))
        {
            await channel.SendPacketAsync(new ConfigurationDisconnectPacket { Reason = reason }, cancellationToken);
        }
        else if (await player.IsLoggingInAsync(cancellationToken))
        {
            await channel.SendPacketAsync(new LoginDisconnectPacket { Reason = reason }, cancellationToken);
        }
        else
        {
            return false;
        }

        return true;
    }
}
