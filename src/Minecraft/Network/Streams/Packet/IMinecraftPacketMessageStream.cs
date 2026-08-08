using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries;

namespace Void.Minecraft.Network.Streams.Packet;

/// <summary>
/// Defines synchronous and asynchronous I/O for decoded Minecraft packets.
/// </summary>
public interface IMinecraftPacketMessageStream : IMinecraftStream
{
    /// <summary>
    /// Gets the packet identifier and transformation registries used by the stream.
    /// </summary>
    public IRegistryHolder Registries { get; }

    /// <summary>
    /// Reads and decodes the next packet from the underlying stream.
    /// </summary>
    /// <returns>The decoded packet. The caller is responsible for its disposal.</returns>
    public IMinecraftPacket ReadPacket();

    /// <summary>
    /// Asynchronously reads and decodes the next packet from the underlying stream.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel packet input.</param>
    /// <returns>The decoded packet. The caller is responsible for its disposal.</returns>
    public ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Encodes and writes a packet to the underlying stream.
    /// </summary>
    /// <param name="packet">The packet to encode and write.</param>
    public void WritePacket(IMinecraftPacket packet);

    /// <summary>
    /// Asynchronously encodes and writes a packet to the underlying stream.
    /// </summary>
    /// <param name="packet">The packet to encode and write.</param>
    /// <param name="cancellationToken">A token used to cancel packet output.</param>
    /// <returns>A task that completes when the packet has been written.</returns>
    public ValueTask WritePacketAsync(IMinecraftPacket packet, CancellationToken cancellationToken = default);
}
