using Microsoft.IO;
using System.Diagnostics.CodeAnalysis;
using Void.Common.Network;
using Void.Common.Network.Streams;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Streams.Packet;
using Void.Minecraft.Network.Streams.Packet.Extensions;
using Void.Minecraft.Network.Streams.Packet.Registries;
using Void.Minecraft.Network.Streams.Packet.Transformations;
using Void.Proxy.Api.Network.Streams.Manual;
using Void.Proxy.Api.Network.Streams.Manual.Binary;
using Void.Proxy.Api.Network.Streams.Recyclable;
using Void.Proxy.Plugins.Common.Network.Messages.Binary;
using Void.Proxy.Plugins.Common.Network.Streams.Extensions;
using Void.Proxy.Plugins.Common.Network.Streams.Packet.Transformations;

namespace Void.Proxy.Plugins.Common.Network.Streams.Packet;

public class MinecraftPacketMessageStream : RecyclableStream, IMinecraftPacketMessageStream
{
    public ProtocolVersion ProtocolVersion => SystemRegistryHolder?.ProtocolVersion ?? ProtocolVersion.Oldest;
    public IMessageStreamBase? BaseStream { get; set; }
    public bool CanRead => BaseStream?.CanRead ?? false;
    public bool CanWrite => BaseStream?.CanWrite ?? false;
    public bool IsAlive => BaseStream?.IsAlive ?? false;
    public IMinecraftPacketSystemRegistry? SystemRegistryHolder { get; set; }
    public IMinecraftPacketPluginsRegistry? PluginsRegistryHolder { get; set; }
    public IMinecraftPacketPluginsTransformations? TransformationsHolder { get; set; }

    public IMinecraftPacket ReadPacket()
    {
        return BaseStream switch
        {
            IManualStream stream => DecodeManual(stream),
            ICompleteMessageStream stream => DecodeCompleteMessage(stream),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public async ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default)
    {
        return BaseStream switch
        {
            IManualStream stream => await DecodeManualAsync(stream, cancellationToken),
            ICompleteMessageStream stream => await DecodeCompleteMessageAsync(stream, cancellationToken),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public void WritePacket(IMinecraftPacket packet, Side origin)
    {
        switch (BaseStream)
        {
            case IManualStream manualStream:
                EncodeManual(manualStream, packet, origin);
                break;
            case ICompleteMessageStream completeMessageStream:
                EncodeCompleteMessage(completeMessageStream, packet, origin);
                break;
            default:
                throw new NotSupportedException(BaseStream?.GetType().FullName);
        }
    }

    public async ValueTask WritePacketAsync(IMinecraftPacket packet, Side origin, CancellationToken cancellationToken = default)
    {
        switch (BaseStream)
        {
            case IManualStream manualStream:
                await EncodeManualAsync(manualStream, packet, origin, cancellationToken);
                break;
            case ICompleteMessageStream completeMessageStream:
                await EncodeCompleteMessageAsync(completeMessageStream, packet, origin, cancellationToken);
                break;
            default:
                throw new NotSupportedException(BaseStream?.GetType().FullName);
        }
    }

    public void Dispose()
    {
        BaseStream?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (BaseStream != null)
            await BaseStream.DisposeAsync();

        GC.SuppressFinalize(this);
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

    private IMinecraftPacket DecodeCompleteMessage(ICompleteMessageStream stream)
    {
        var message = stream.ReadMessage();
        return DecodePacket(message.Stream);
    }

    private async ValueTask<IMinecraftPacket> DecodeCompleteMessageAsync(ICompleteMessageStream stream, CancellationToken cancellationToken = default)
    {
        var message = await stream.ReadMessageAsync(cancellationToken);
        return DecodePacket(message.Stream);
    }

    private void EncodeCompleteMessage(ICompleteMessageStream stream, IMinecraftPacket packet, Side origin)
    {
        stream.WriteMessage(new CompleteBinaryMessage(EncodePacket(packet, origin)));
    }

    private async ValueTask EncodeCompleteMessageAsync(ICompleteMessageStream stream, IMinecraftPacket packet, Side origin, CancellationToken cancellationToken = default)
    {
        await stream.WriteMessageAsync(new CompleteBinaryMessage(EncodePacket(packet, origin)), cancellationToken);
    }

    private IMinecraftPacket DecodeManual(IManualStream manualStream)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();

        var packetLength = manualStream.ReadVarInt();
        var packetBuffer = stream.GetSpan(packetLength);

        manualStream.ReadExactly(packetBuffer[..packetLength]);
        stream.Advance(packetLength);

        return DecodePacket(stream);
    }

    private async ValueTask<IMinecraftPacket> DecodeManualAsync(IManualStream manualStream, CancellationToken cancellationToken = default)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();

        var packetLength = await manualStream.ReadVarIntAsync(cancellationToken);
        var packetBuffer = stream.GetMemory(packetLength);

        await manualStream.ReadExactlyAsync(packetBuffer[..packetLength], cancellationToken);
        stream.Advance(packetLength);

        return DecodePacket(stream);
    }

    private void EncodeManual(IManualStream manualStream, IMinecraftPacket packet, Side origin)
    {
        using var stream = EncodePacketWithLength(packet, origin);

        foreach (var memory in stream.GetReadOnlySequence())
            manualStream.Write(memory.Span);
    }

    private async ValueTask EncodeManualAsync(IManualStream manualStream, IMinecraftPacket packet, Side origin, CancellationToken cancellationToken = default)
    {
        await using var stream = EncodePacketWithLength(packet, origin);

        foreach (var memory in stream.GetReadOnlySequence())
            await manualStream.WriteAsync(memory, cancellationToken);
    }

    public IMinecraftPacket DecodePacket(RecyclableMemoryStream stream)
    {
        var buffer = new MinecraftBuffer(stream.GetReadOnlySequence());
        var id = buffer.ReadVarInt();

        // Set packet data position for further usage. Stream property Length is preserved.
        stream.Position = buffer.Position;

        if (SystemRegistryHolder?.Read is not { } registry || !registry.TryCreateDecoder(id, out var decoder))
            return new MinecraftBinaryPacket(id, stream);

        var packet = decoder(ref buffer, ProtocolVersion);
        stream.Dispose();

        if (buffer.HasData)
            throw new IndexOutOfRangeException($"{packet} packet was not fully read. Bytes read: {buffer.Position}, Total length: {buffer.Length}.");

        return packet;
    }

    private RecyclableMemoryStream EncodePacketWithLength(IMinecraftPacket packet, Side origin)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();
        using var packetStream = EncodePacket(packet, origin);

        var length = (int)packetStream.Position;
        EncodeVarInt(stream, length);

        packetStream.Position = 0;
        packetStream.CopyTo(stream);

        return stream;
    }

    private RecyclableMemoryStream EncodePacket(IMinecraftPacket packet, Side origin)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();

        if (packet is MinecraftBinaryPacket binaryPacket)
        {
            EncodeVarInt(stream, binaryPacket.Id);
            binaryPacket.Stream.CopyTo(stream);
        }
        else
        {
            if (!TryGetPacketId(packet, out var id))
                throw new InvalidOperationException($"{packet} is not registered:\n" +
                    $"{nameof(SystemRegistryHolder)} [{string.Join(", ", SystemRegistryHolder?.Write?.PacketTypes.Select(type => type.Name) ?? [])}]\n" +
                    $"{nameof(PluginsRegistryHolder)} [{string.Join(", ", PluginsRegistryHolder?.All.SelectMany(registry => registry.PacketTypes).Select(type => type.Name) ?? [])}]");

            EncodeVarInt(stream, id);

            var buffer = new MinecraftBuffer(stream);
            var wrapper = new MinecraftBinaryPacketWrapper(new MinecraftBinaryPacket(id, stream), origin);

            var position = stream.Position;
            packet.Encode(ref buffer, ProtocolVersion);

            if (TryGetTransformations(packet, out var transformations))
            {
                foreach (var transformation in transformations)
                {
                    stream.Position = position;
                    transformation(wrapper);
                    wrapper.Reset();
                }
            }

            stream.SetLength(stream.Position);
            stream.Position = position;

            wrapper.WriteProcessedValues(ref buffer);
        }

        return stream;
    }

    private static void EncodeVarInt(RecyclableMemoryStream stream, int id)
    {
        foreach (var @byte in MinecraftBuffer.EnumerateVarInt(id))
            stream.WriteByte(@byte);
    }

    private bool TryGetPacketId(IMinecraftPacket packet, out int id)
    {
        id = 0;

        if (SystemRegistryHolder?.Write is { } systemRegistry)
        {
            if (systemRegistry.TryGetPacketId(packet, out id))
                return true;
        }

        if (PluginsRegistryHolder is { } pluginsRegistries)
        {
            foreach (var registry in pluginsRegistries.All)
            {
                if (registry.TryGetPacketId(packet, out id))
                    return true;
            }
        }

        return false;
    }

    private bool TryGetTransformations(IMinecraftPacket packet, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformations)
    {
        transformations = null;

        if (PluginsRegistryHolder is null)
            return false;

        if (TransformationsHolder is null)
            return false;

        return PluginsRegistryHolder.TryGetTransformations(TransformationsHolder, packet, TransformationType.Downgrade, out transformations);
    }
}
