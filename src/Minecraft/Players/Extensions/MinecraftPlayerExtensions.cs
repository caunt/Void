using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Events.Chat;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network.Channels.Extensions;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Extensions;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Extensions;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Extensions;

namespace Void.Minecraft.Players.Extensions;

public static class MinecraftPlayerExtensions
{
    public static async ValueTask<ChatSendResult> SendChatMessageAsync(this IMinecraftPlayer player, Component message, CancellationToken cancellationToken = default)
    {
        var events = player.Context.Services.GetRequiredService<IEventService>();
        return await events.ThrowWithResultAsync(new ChatMessageSendEvent(player, message), cancellationToken);
    }

    public static async ValueTask KickAsync(this IMinecraftPlayer player, Component? reason = null, CancellationToken cancellationToken = default)
    {
        var players = player.Context.Services.GetRequiredService<IPlayerService>();
        await players.KickPlayerAsync(player, reason, cancellationToken);
    }

    public static async ValueTask SendPacketAsync<T>(this IMinecraftPlayer player, T packet, CancellationToken cancellationToken = default) where T : IMinecraftPacket
    {
        var channel = await player.GetChannelAsync(cancellationToken);
        await channel.SendPacketAsync(packet, cancellationToken);
    }

    public static void RegisterPacket<T>(this IMinecraftPlayer player, params MinecraftPacketIdMapping[] mappings) where T : IMinecraftPacket
    {
        var plugin = GetPacketPlugin<T>(player.Context.Services);
        var link = player.GetLink();
        var direction = typeof(T).IsAssignableTo(typeof(IMinecraftClientboundPacket)) ? Direction.Clientbound : Direction.Serverbound;
        var registry = link.GetRegistries(direction).PacketIdPlugins.Get(plugin);

        registry.RegisterPacket<T>(player.ProtocolVersion, mappings);
    }

    public static void RegisterTransformations<T>(this IMinecraftPlayer player, params MinecraftPacketTransformationMapping[] mappings) where T : IMinecraftPacket
    {
        var plugin = GetPacketPlugin<T>(player.Context.Services);
        var link = player.GetLink();

        link.GetRegistries(Direction.Clientbound).PacketTransformationsPlugins.Get(plugin).RegisterTransformations<T>(player.ProtocolVersion, mappings);
        link.GetRegistries(Direction.Serverbound).PacketTransformationsPlugins.Get(plugin).RegisterTransformations<T>(player.ProtocolVersion, mappings);
    }

    private static IPlugin GetPacketPlugin<T>(IServiceProvider services) where T : IMinecraftPacket
    {
        var packetType = typeof(T);
        var plugins = services.GetRequiredService<IPluginService>();

        if (!plugins.TryGetPlugin(packetType, out var plugin))
            throw new InvalidOperationException($"Plugin for packet {packetType.Name} not found.");

        return plugin;
    }
}
