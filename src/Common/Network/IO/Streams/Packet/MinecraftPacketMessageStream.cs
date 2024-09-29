using System.Buffers;
using Microsoft.IO;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Streams;
using Void.Proxy.API.Network.IO.Streams.Extensions;
using Void.Proxy.API.Network.IO.Streams.Manual;
using Void.Proxy.API.Network.IO.Streams.Manual.Binary;
using Void.Proxy.API.Network.IO.Streams.Recyclable;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;
using Void.Proxy.Common.Network.IO.Messages.Binary;
using Void.Proxy.Common.Registries.Packets;

namespace Void.Proxy.Common.Network.IO.Streams.Packet;

public class MinecraftPacketMessageStream : MinecraftRecyclableStream, IMinecraftPacketMessageStream
{
    public ProtocolVersion ProtocolVersion => RegistryHolder?.ProtocolVersion ?? ProtocolVersion.Oldest;
    public IMinecraftStreamBase? BaseStream { get; set; }
    public IPacketRegistryHolder? RegistryHolder { get; set; }
    public Direction? Flow { get; set; }

    public IMinecraftPacket ReadPacket()
    {
        return BaseStream switch
        {
            IMinecraftManualStream stream => DecodeManual(stream),
            IMinecraftCompleteMessageStream stream => DecodeCompleteMessage(stream),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public async ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default)
    {
        return BaseStream switch
        {
            IMinecraftManualStream stream => await DecodeManualAsync(stream, cancellationToken),
            IMinecraftCompleteMessageStream stream => await DecodeCompleteMessageAsync(stream, cancellationToken),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public void WritePacket(IMinecraftPacket packet)
    {
        switch (BaseStream)
        {
            case IMinecraftManualStream manualStream:
                EncodeManual(manualStream, packet);
                break;
            case IMinecraftCompleteMessageStream completeMessageStream:
                EncodeCompleteMessage(completeMessageStream, packet);
                break;
            default:
                throw new NotSupportedException(BaseStream?.GetType().FullName);
        }
    }

    public async ValueTask WritePacketAsync(IMinecraftPacket packet, CancellationToken cancellationToken = default)
    {
        switch (BaseStream)
        {
            case IMinecraftManualStream manualStream:
                await EncodeManualAsync(manualStream, packet, cancellationToken);
                break;
            case IMinecraftCompleteMessageStream completeMessageStream:
                await EncodeCompleteMessageAsync(completeMessageStream, packet, cancellationToken);
                break;
            default:
                throw new NotSupportedException(BaseStream?.GetType().FullName);
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
        return DecodePacket(message.Stream);
    }

    private async ValueTask<IMinecraftPacket> DecodeCompleteMessageAsync(IMinecraftCompleteMessageStream stream, CancellationToken cancellationToken = default)
    {
        var message = await stream.ReadMessageAsync(cancellationToken);
        return DecodePacket(message.Stream);
    }

    private void EncodeCompleteMessage(IMinecraftCompleteMessageStream stream, IMinecraftPacket packet)
    {
        stream.WriteMessage(new CompleteBinaryMessage(EncodePacket(packet)));
    }

    private async ValueTask EncodeCompleteMessageAsync(IMinecraftCompleteMessageStream stream, IMinecraftPacket packet, CancellationToken cancellationToken = default)
    {
        await stream.WriteMessageAsync(new CompleteBinaryMessage(EncodePacket(packet)), cancellationToken);
    }

    private IMinecraftPacket DecodeManual(IMinecraftManualStream manualStream)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();

        var packetLength = manualStream.ReadVarInt();
        var packetBuffer = stream.GetSpan(packetLength);

        manualStream.ReadExactly(packetBuffer[..packetLength]);
        stream.Advance(packetLength);

        return DecodePacket(stream);
    }

    private async ValueTask<IMinecraftPacket> DecodeManualAsync(IMinecraftManualStream manualStream, CancellationToken cancellationToken = default)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();

        var packetLength = await manualStream.ReadVarIntAsync(cancellationToken);
        var packetBuffer = stream.GetMemory(packetLength);

        await manualStream.ReadExactlyAsync(packetBuffer[..packetLength], cancellationToken);
        stream.Advance(packetLength);

        return DecodePacket(stream);
    }

    private void EncodeManual(IMinecraftManualStream manualStream, IMinecraftPacket packet)
    {
        using var stream = EncodePacketWithLength(packet);

        foreach (var memory in stream.GetReadOnlySequence())
            manualStream.Write(memory.Span);
    }

    private async ValueTask EncodeManualAsync(IMinecraftManualStream manualStream, IMinecraftPacket packet, CancellationToken cancellationToken = default)
    {
        await using var stream = EncodePacketWithLength(packet);

        foreach (var memory in stream.GetReadOnlySequence())
            await manualStream.WriteAsync(memory, cancellationToken);
    }

    public IMinecraftPacket DecodePacket(RecyclableMemoryStream stream)
    {
        var buffer = new MinecraftBuffer(stream.GetReadOnlySequence());
        var id = buffer.ReadVarInt();

        // Set packet data position for further usage. Stream property Length is preserved.
        stream.Position = buffer.Position;

        if (RegistryHolder?.GetRegistry(Flow, Operation.Read) is not { } registry || !registry.TryCreateDecoder(id, out var decoder))
            return new BinaryPacket(id, stream);

        var packet = decoder(ref buffer, ProtocolVersion);
        stream.Dispose();

        if (buffer.HasData)
            throw new IndexOutOfRangeException($"{packet} packet was not fully read. Bytes read: {buffer.Position}, Total length: {buffer.Length}.");

        return packet;
    }

    private RecyclableMemoryStream EncodePacketWithLength(IMinecraftPacket packet)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();
        using var packetStream = EncodePacket(packet);

        var length = (int)packetStream.Position;
        EncodeVarInt(stream, length);

        packetStream.Position = 0;
        packetStream.CopyTo(stream);

        return stream;
    }

    private RecyclableMemoryStream EncodePacket(IMinecraftPacket packet)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();

        if (packet is BinaryPacket binaryPacket)
        {
            EncodeVarInt(stream, binaryPacket.Id);
            binaryPacket.Stream.CopyTo(stream);
        }
        else
        {
            if (RegistryHolder?.GetRegistry(Flow, Operation.Write) is not { } registry || !registry.TryGetPacketId(packet, out var id))
                throw new InvalidOperationException($"Cannot find id for packet {packet}");

            EncodeVarInt(stream, id);

            // TODO remove this constant length, implement MinecraftBuffer(IMemoryStream) constructor
            var length = 2048;

            var array = ArrayPool<byte>.Shared.Rent(length);
            var span = array.AsSpan(0, length);

            try
            {
                var buffer = new MinecraftBuffer(span);
                packet.Encode(ref buffer, ProtocolVersion);

                span = span[..buffer.Position];
                stream.Write(span);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        return stream;
    }

    private static void EncodeVarInt(RecyclableMemoryStream stream, int id)
    {
        foreach (var @byte in MinecraftBuffer.EnumerateVarInt(id))
            stream.WriteByte(@byte);
    }
}