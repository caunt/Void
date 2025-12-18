using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Events;
using Void.Minecraft.Events.Chat;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Channels.Extensions;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Extensions;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Extensions;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Extensions;

namespace Void.Minecraft.Players.Extensions;

public static class PlayerExtensions
{
    extension(IPlayer player)
    {
        private MinecraftPlayer AsMinecraft => TryGetMinecraftPlayer(player, out var minecraftPlayer) ? minecraftPlayer : throw new InvalidOperationException($"Player is not a {nameof(MinecraftPlayer)}.");

        public bool IsMinecraft => player is MinecraftPlayer;
        public ProtocolVersion ProtocolVersion => player.AsMinecraft.ProtocolVersion;
        public Phase Phase { get => player.AsMinecraft.Phase; set => player.AsMinecraft.Phase = value; }
        public GameProfile? Profile { get => player.AsMinecraft.Profile; set => player.AsMinecraft.Profile = value; }
        public IdentifiedKey? IdentifiedKey { get => player.AsMinecraft.IdentifiedKey; set => player.AsMinecraft.IdentifiedKey = value; }
        public ILogger Logger
        {
            get
            {
                var contextBuilder = new StringBuilder();
                contextBuilder.Append(player.GetType().Name);
                contextBuilder.Append(" <");
                contextBuilder.Append(player);
                contextBuilder.Append('>');

                var loggerFactory = player.Context.Services.GetRequiredService<ILoggerFactory>();
                return loggerFactory.CreateLogger(contextBuilder.ToString());
            }
        }

        public async ValueTask<ChatSendResult> SendChatMessageAsync(Component message, CancellationToken cancellationToken = default)
        {
            var events = player.Context.Services.GetRequiredService<IEventService>();
            return await events.ThrowWithResultAsync(new ChatMessageSendEvent(player.AsMinecraft, message, Side.Proxy), cancellationToken);
        }

        public async ValueTask KickAsync(Component? reason = null, CancellationToken cancellationToken = default)
        {
            var players = player.Context.Services.GetRequiredService<IPlayerService>();

            if (player.TryGetMinecraftPlayer(out var minecraftPlayer))
                await players.KickPlayerAsync(player, new MinecraftPlayerKickEvent(player, reason), cancellationToken);
            else
                await players.KickPlayerAsync(player, reason?.SerializeLegacy(), cancellationToken);
        }

        public async ValueTask SendPacketAsync<T>(T packet, CancellationToken cancellationToken = default) where T : IMinecraftMessage
        {
            var channel = await player.GetChannelAsync(cancellationToken);
            await channel.SendPacketAsync(packet, cancellationToken);
        }

        public void RegisterPacket<T>(params MinecraftPacketIdMapping[] mappings) where T : IMinecraftPacket
        {
            player.RegisterPacket<T>(Operation.Any, mappings);
        }

        public void RegisterPacket<T>(Operation operation, params MinecraftPacketIdMapping[] mappings) where T : IMinecraftPacket
        {
            if (typeof(T).IsAssignableTo(typeof(IMinecraftClientboundPacket)))
            {
                player.RegisterPacket<T>(Direction.Clientbound, operation, mappings);
                return;
            }

            if (typeof(T).IsAssignableTo(typeof(IMinecraftServerboundPacket)))
            {
                player.RegisterPacket<T>(Direction.Serverbound, operation, mappings);
                return;
            }

            throw new InvalidOperationException($"Packet {typeof(T).Name} is neither Clientbound nor Serverbound. Specify the direction with {nameof(RegisterPacket)}<{typeof(T).Name}>(Direction, ...).");
        }

        public void RegisterPacket<T>(Direction direction, params MinecraftPacketIdMapping[] mappings) where T : IMinecraftPacket
        {
            player.RegisterPacket<T>(direction, Operation.Any, mappings);
        }

        public void RegisterPacket<T>(Direction direction, Operation operation, params MinecraftPacketIdMapping[] mappings) where T : IMinecraftPacket
        {
            var plugin = player.Context.Services.GetRequiredService<IPluginService>().GetPluginFromType<T>();

            if (player.Link is { } link)
            {
                var (fromChannel, toChannel) = direction switch
                {
                    Direction.Clientbound => (link.ServerChannel, link.PlayerChannel),
                    Direction.Serverbound => (link.PlayerChannel, link.ServerChannel),
                    _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
                };

                if (operation.HasFlag(Operation.Read))
                    player.RegisterPacket<T>(fromChannel, Operation.Read, mappings);

                if (operation.HasFlag(Operation.Write))
                    player.RegisterPacket<T>(toChannel, Operation.Write, mappings);
            }
            else if (direction is Direction.Serverbound) // If no link created yet, we still should have the player channel
            {
                if (player.Context.Channel is null)
                    throw new InvalidOperationException($"Cannot register {nameof(Direction.Serverbound)} {typeof(T)} packet without a Player channel.");

                if (operation.HasFlag(Operation.Write))
                    throw new InvalidOperationException($"Cannot register {nameof(Direction.Serverbound)} {typeof(T)} packet for {Operation.Write} operation without a Server channel.");

                if (operation.HasFlag(Operation.Read))
                    player.RegisterPacket<T>(player.Context.Channel, Operation.Read, mappings);
            }
            else
            {
                throw new InvalidOperationException($"Cannot register {nameof(Direction.Clientbound)} {typeof(T)} packet without a Server channel.");
            }
        }

        public void RegisterPacket<T>(INetworkChannel channel, Operation operation, params MinecraftPacketIdMapping[] mappings) where T : IMinecraftPacket
        {
            channel.MinecraftRegistries.PacketIdPlugins
                .Get(operation, player.Context.Services
                    .GetRequiredService<IPluginService>()
                    .GetPluginFromType<T>())
                .RegisterPacket<T>(player.AsMinecraft.ProtocolVersion, mappings);
        }

        [Obsolete($"Use {nameof(RegisterTransformations)}<T>({nameof(INetworkChannel)}, {nameof(MinecraftPacketTransformationMapping)}[]) instead.")]
        public void RegisterTransformations<T>(params MinecraftPacketTransformationMapping[] mappings) where T : IMinecraftPacket
        {
            var plugin = player.Context.Services.GetRequiredService<IPluginService>().GetPluginFromType<T>();
            var link = player.Link ?? throw new InvalidOperationException("Cannot register packet transformations without an established link.");

            link.PlayerChannel.MinecraftRegistries.PacketTransformationsPlugins.Get(plugin).RegisterTransformations<T>(player.AsMinecraft.ProtocolVersion, mappings);
            link.ServerChannel.MinecraftRegistries.PacketTransformationsPlugins.Get(plugin).RegisterTransformations<T>(player.AsMinecraft.ProtocolVersion, mappings);
        }

        public void RegisterTransformations<T>(INetworkChannel channel, params MinecraftPacketTransformationMapping[] mappings) where T : IMinecraftPacket
        {
            var plugin = player.Context.Services.GetRequiredService<IPluginService>().GetPluginFromType<T>();
            channel.MinecraftRegistries.PacketTransformationsPlugins.Get(plugin).RegisterTransformations<T>(player.AsMinecraft.ProtocolVersion, mappings);
        }

        internal bool TryGetMinecraftPlayer([MaybeNullWhen(false)] out MinecraftPlayer minecraftPlayer)
        {
            minecraftPlayer = player as MinecraftPlayer;
            return minecraftPlayer is not null;
        }
    }
}
