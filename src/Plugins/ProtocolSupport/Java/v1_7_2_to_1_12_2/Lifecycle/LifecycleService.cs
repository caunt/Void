using Void.Common.Players;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Channels.Extensions;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.Common.Services.Lifecycle;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Extensions;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Lifecycle;

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

    protected override async ValueTask<bool> SendChatMessageAsync(IPlayer player, Component text, CancellationToken cancellationToken)
    {
        if (!await player.IsPlayingAsync(cancellationToken))
            return false;

        var channel = await player.GetChannelAsync(cancellationToken);
        await channel.SendPacketAsync(new ChatMessagePacket { Message = text }, cancellationToken);
        return true;
    }

    protected override async ValueTask<bool> KickPlayerAsync(IPlayer player, Component reason, CancellationToken cancellationToken)
    {
        var channel = await player.GetChannelAsync(cancellationToken);

        if (await player.IsPlayingAsync(cancellationToken))
        {
            await channel.SendPacketAsync(new PlayDisconnectPacket { Reason = reason }, cancellationToken);
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
