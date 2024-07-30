using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Memory;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams.Extensions;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Registries.Packets;

namespace Void.Proxy.API.Network.IO.Streams.Packet;

public class MinecraftPacketMessageStream : IMinecraftPacketMessageStream
{
    public IMinecraftStreamBase? BaseStream { get; set; }
    public IPacketRegistryHolder? RegistryHolder { get; set; }
    public Direction? Flow { get; set; }
    public ProtocolVersion ProtocolVersion => RegistryHolder?.GetProtocolVersion(Flow) ?? ProtocolVersion.Oldest;

    public IMinecraftPacket ReadPacket()
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream stream => DecodeNetwork(stream),
            IMinecraftCompleteMessageStream stream => DecodeCompleteMessage(stream),
            _ => throw new NotImplementedException(BaseStream?.GetType().FullName)
        };
    }

    public async ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default)
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream stream => await DecodeNetworkAsync(stream, cancellationToken),
            IMinecraftCompleteMessageStream stream => await DecodeCompleteMessageAsync(stream, cancellationToken),
            _ => throw new NotImplementedException(BaseStream?.GetType().FullName)
        };
    }

    public void WritePacket(IMinecraftPacket packet)
    {
        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                EncodeNetwork(networkStream, packet);
                break;
            case IMinecraftCompleteMessageStream completeMessageStream:
                EncodeCompleteMessage(completeMessageStream, packet);
                break;
            default:
                throw new NotImplementedException(BaseStream?.GetType().FullName);
        }
    }

    public async ValueTask WritePacketAsync(IMinecraftPacket packet, CancellationToken cancellationToken = default)
    {
        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                await EncodeNetworkAsync(networkStream, packet, cancellationToken);
                break;
            case IMinecraftCompleteMessageStream completeMessageStream:
                await EncodeCompleteMessageAsync(completeMessageStream, packet, cancellationToken);
                break;
            default:
                throw new NotImplementedException(BaseStream?.GetType().FullName);
        }
    }

    public void Dispose()
    {
        BaseStream?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (BaseStream != null)
            await BaseStream.DisposeAsync();
    }

    public void Flush()
    {
        BaseStream?.Flush();
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        if (BaseStream != null)
            await BaseStream.FlushAsync(cancellationToken);
    }

    public void Close()
    {
        BaseStream?.Close();
    }

    private IMinecraftPacket DecodeCompleteMessage(IMinecraftCompleteMessageStream stream)
    {
        var message = stream.ReadMessage();
        return DecodePacket(message.Holder);
    }

    private async ValueTask<IMinecraftPacket> DecodeCompleteMessageAsync(IMinecraftCompleteMessageStream stream, CancellationToken cancellationToken = default)
    {
        var message = await stream.ReadMessageAsync(cancellationToken);
        return DecodePacket(message.Holder);
    }

    private void EncodeCompleteMessage(IMinecraftCompleteMessageStream stream, IMinecraftPacket packet)
    {
        stream.WriteMessage(new CompleteBinaryMessage(MemoryHolder.Concatenate(EncodePacket(packet))));
    }

    private async ValueTask EncodeCompleteMessageAsync(IMinecraftCompleteMessageStream stream, IMinecraftPacket packet, CancellationToken cancellationToken = default)
    {
        await stream.WriteMessageAsync(new CompleteBinaryMessage(MemoryHolder.Concatenate(EncodePacket(packet))), cancellationToken);
    }

    private IMinecraftPacket DecodeNetwork(IMinecraftNetworkStream stream)
    {
        var length = stream.ReadVarInt();
        var holder = MemoryHolder.RentExact(length);
        stream.ReadExactly(holder.Slice.Span);
        return DecodePacket(holder);
    }

    private async ValueTask<IMinecraftPacket> DecodeNetworkAsync(IMinecraftNetworkStream stream, CancellationToken cancellationToken = default)
    {
        var length = await stream.ReadVarIntAsync(cancellationToken);
        var holder = MemoryHolder.RentExact(length);
        await stream.ReadExactlyAsync(holder.Slice, cancellationToken);
        return DecodePacket(holder);
    }

    private void EncodeNetwork(IMinecraftNetworkStream stream, IMinecraftPacket packet)
    {
        foreach (var holder in EncodePacketWithLength(packet))
        {
            stream.Write(holder.Slice.Span);
            holder.Dispose();
        }
    }

    private async ValueTask EncodeNetworkAsync(IMinecraftNetworkStream stream, IMinecraftPacket packet, CancellationToken cancellationToken = default)
    {
        foreach (var holder in EncodePacketWithLength(packet))
        {
            await stream.WriteAsync(holder.Slice, cancellationToken);
            holder.Dispose();
        }
    }

    public IMinecraftPacket DecodePacket(MemoryHolder holder)
    {
        var buffer = new MinecraftBuffer(holder.Slice.Span);
        var id = buffer.ReadVarInt();

        if (RegistryHolder?.GetRegistry(Flow, Operation.Read) is { } registry && registry.TryCreateDecoder(id, out var decoder))
        {
            var packet = decoder(ref buffer, ProtocolVersion);

            if (buffer.HasData)
                throw new IndexOutOfRangeException($"The packet was not fully read. Bytes read: {buffer.Position}, Total length: {buffer.Length}.");

            return packet;
        }

        holder.Slice = holder.Slice[buffer.Position..];
        return new BinaryPacket(id, holder);
    }

    private IEnumerable<MemoryHolder> EncodePacketWithLength(IMinecraftPacket packet)
    {
        var slices = new List<MemoryHolder>(2);
        slices.AddRange(EncodePacket(packet));

        yield return EncodeVarInt(slices.Sum(holder => holder.Slice.Length));

        foreach (var slice in slices)
            yield return slice;
    }

    private IEnumerable<MemoryHolder> EncodePacket(IMinecraftPacket packet)
    {
        if (packet is BinaryPacket binaryPacket)
        {
            yield return EncodeVarInt(binaryPacket.Id);
            yield return binaryPacket.Holder;
        }
        else
        {
            if (RegistryHolder?.GetRegistry(Flow, Operation.Write) is not { } registry || !registry.TryGetPacketId(packet, out var id))
                throw new InvalidOperationException($"Cannot find id for packet {packet}");

            yield return EncodeVarInt(id);

            var holder = MemoryHolder.RentExact(2048);
            var buffer = new MinecraftBuffer(holder.Slice.Span);

            packet.Encode(ref buffer, ProtocolVersion.Latest);
            holder.Slice = holder.Slice[..buffer.Position];

            yield return holder;
        }
    }

    private static MemoryHolder EncodeVarInt(int id)
    {
        var holder = MemoryHolder.RentExact(MinecraftBuffer.GetVarIntSize(id));
        var buffer = new MinecraftBuffer(holder.Slice.Span);

        buffer.WriteVarInt(id);

        return holder;
    }
}