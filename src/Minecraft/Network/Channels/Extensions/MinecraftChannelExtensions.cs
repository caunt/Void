using System;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Binary;
using Void.Minecraft.Network.Registries;
using Void.Minecraft.Network.Streams.Packet;
using Void.Proxy.Api.Network.Channels;

namespace Void.Minecraft.Network.Channels.Extensions;

/// <summary>
/// Provides Minecraft registry and packet operations for generic network channels.
/// </summary>
public static class MinecraftChannelExtensions
{
    extension(INetworkChannel channel)
    {
        /// <summary>
        /// Gets the registries owned by the channel's Minecraft packet stream.
        /// </summary>
        /// <exception cref="InvalidOperationException">The channel does not contain an <see cref="IMinecraftPacketMessageStream" /> layer.</exception>
        public IRegistryHolder MinecraftRegistries
        {
            get
            {
                if (channel.TryGet<IMinecraftPacketMessageStream>(out var stream))
                    return stream.Registries;

                throw new InvalidOperationException($"{nameof(IMinecraftPacketMessageStream)} is not found on this channel");
            }
        }

        /// <summary>
        /// Writes a Minecraft message through the channel.
        /// </summary>
        /// <remarks>For an <see cref="IMinecraftBinaryMessage" />, this method moves its stream position past the variable-length packet identifier before writing. The new stream position remains observable after the operation.</remarks>
        /// <typeparam name="T">The Minecraft message type.</typeparam>
        /// <param name="packet">The message to write.</param>
        /// <param name="cancellationToken">A token used to cancel channel output.</param>
        /// <returns>A task that completes when the channel has written the message.</returns>
        public async ValueTask SendPacketAsync<T>(T packet, CancellationToken cancellationToken) where T : IMinecraftMessage
        {
            if (packet is IMinecraftBinaryMessage binaryMessage)
                binaryMessage.Stream.Position = binaryMessage.Id.VarIntSize();

            await channel.WriteMessageAsync(packet, cancellationToken);
        }
    }
}
