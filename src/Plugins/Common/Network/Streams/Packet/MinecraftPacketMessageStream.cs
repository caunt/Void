using Microsoft.IO;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries;
using Void.Minecraft.Network.Registries.PacketId.Extensions;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Streams.Packet;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Streams;
using Void.Proxy.Api.Network.Streams.Manual;
using Void.Proxy.Api.Network.Streams.Manual.Binary;
using Void.Proxy.Api.Network.Streams.Recyclable;
using Void.Proxy.Plugins.Common.Network.Messages.Binary;
using Void.Proxy.Plugins.Common.Network.Registries;
using Void.Proxy.Plugins.Common.Network.Registries.Transformations.Mappings;
using Void.Proxy.Plugins.Common.Network.Streams.Extensions;

namespace Void.Proxy.Plugins.Common.Network.Streams.Packet;

public class MinecraftPacketMessageStream : RecyclableStream, IMinecraftPacketMessageStream
{
    public IMessageStreamBase? BaseStream { get; set; }
    public bool CanRead => BaseStream?.CanRead ?? false;
    public bool CanWrite => BaseStream?.CanWrite ?? false;
    public bool IsAlive => BaseStream?.IsAlive ?? false;
    public IRegistryHolder Registries { get; } = new RegistryHolder();

    public IMinecraftPacket ReadPacket(Side origin)
    {
        return BaseStream switch
        {
            IManualStream stream => DecodeManual(stream, origin),
            ICompleteMessageStream stream => DecodeCompleteMessage(stream, origin),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public async ValueTask<IMinecraftPacket> ReadPacketAsync(Side origin, CancellationToken cancellationToken = default)
    {
        return BaseStream switch
        {
            IManualStream stream => await DecodeManualAsync(stream, origin, cancellationToken),
            ICompleteMessageStream stream => await DecodeCompleteMessageAsync(stream, origin, cancellationToken),
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

    private IMinecraftPacket DecodeCompleteMessage(ICompleteMessageStream stream, Side origin)
    {
        var message = stream.ReadMessage();
        return DecodePacket(message.Stream, origin);
    }

    private async ValueTask<IMinecraftPacket> DecodeCompleteMessageAsync(ICompleteMessageStream stream, Side origin, CancellationToken cancellationToken = default)
    {
        var message = await stream.ReadMessageAsync(cancellationToken);
        return DecodePacket(message.Stream, origin);
    }

    private void EncodeCompleteMessage(ICompleteMessageStream stream, IMinecraftPacket packet, Side origin)
    {
        stream.WriteMessage(new CompleteBinaryMessage(EncodePacket(packet, origin)));
    }

    private async ValueTask EncodeCompleteMessageAsync(ICompleteMessageStream stream, IMinecraftPacket packet, Side origin, CancellationToken cancellationToken = default)
    {
        await stream.WriteMessageAsync(new CompleteBinaryMessage(EncodePacket(packet, origin)), cancellationToken);
    }

    private IMinecraftPacket DecodeManual(IManualStream manualStream, Side origin)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();

        var packetLength = manualStream.ReadVarInt();
        var packetBuffer = stream.GetSpan(packetLength);

        manualStream.ReadExactly(packetBuffer[..packetLength]);
        stream.Advance(packetLength);

        return DecodePacket(stream, origin);
    }

    private async ValueTask<IMinecraftPacket> DecodeManualAsync(IManualStream manualStream, Side origin, CancellationToken cancellationToken = default)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();

        var packetLength = await manualStream.ReadVarIntAsync(cancellationToken);
        var packetBuffer = stream.GetMemory(packetLength);

        await manualStream.ReadExactlyAsync(packetBuffer[..packetLength], cancellationToken);
        stream.Advance(packetLength);

        return DecodePacket(stream, origin);
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

    public IMinecraftPacket DecodePacket(RecyclableMemoryStream stream, Side origin)
    {
        stream.Position = 0;

        var buffer = new MinecraftBuffer(stream);
        var id = buffer.ReadVarInt();

        // Set packet data position for further usage. Stream property Length is preserved.
        stream.Position = buffer.Position;

        if (Registries.PacketIdSystem?.Read is not { } registry || !registry.TryCreateDecoder(id, out var packetType, out var decoder))
            return new MinecraftBinaryPacket(id, stream);

        // Console.WriteLine(stream.Position + " " + stream.Length + " " + Convert.ToHexString(buffer.Dump()));
        if (TryGetTransformations(packetType, TransformationType.Upgrade, out var transformations))
        {
            var position = stream.Position;
            var wrapper = new MinecraftBinaryPacketWrapper(new MinecraftBinaryPacket(id, stream), origin);

            foreach (var transformation in transformations)
            {
                transformation(wrapper);
                wrapper.Reset();
                stream.Position = position;
            }

            wrapper.WriteProcessedValues(ref buffer);
            stream.SetLength(stream.Position);
            stream.Position = position;
        }

        var packet = decoder(ref buffer, Registries.ProtocolVersion);

        if (buffer.HasData)
            throw new IndexOutOfRangeException($"{packet} packet was not fully read. Bytes read: {buffer.Position}, Total length: {buffer.Length}.");

        stream.Dispose();
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
                throw new InvalidOperationException($"{packet} is not registered:\n{Registries.PrintPackets()}");

            EncodeVarInt(stream, id);

            var buffer = new MinecraftBuffer(stream);
            var wrapper = new MinecraftBinaryPacketWrapper(new MinecraftBinaryPacket(id, stream), origin);

            var position = stream.Position;
            packet.Encode(ref buffer, Registries.ProtocolVersion);

            if (TryGetTransformations(packet.GetType(), TransformationType.Downgrade, out var transformations))
            {
                foreach (var transformation in transformations)
                {
                    stream.Position = position;
                    transformation(wrapper);
                    wrapper.Reset();
                }
            }

            stream.Position = position;
            wrapper.WriteProcessedValues(ref buffer);
            stream.SetLength(stream.Position);
        }

        return stream;
    }

    private static void EncodeVarInt(RecyclableMemoryStream stream, int id)
    {
        Span<byte> buffer = stackalloc byte[5];
        var length = id.AsVarInt(buffer);
        stream.Write(buffer[..length]);
    }

    private bool TryGetPacketId(IMinecraftPacket packet, out int id)
    {
        id = 0;

        if (Registries.PacketIdSystem?.Write is { } systemRegistry)
        {
            if (systemRegistry.TryGetPacketId(packet, out id))
                return true;
        }

        if (Registries.PacketIdPlugins is { } pluginsRegistries)
        {
            foreach (var registry in pluginsRegistries.All)
            {
                if (registry.TryGetPacketId(packet, out id))
                    return true;
            }
        }

        return false;
    }

    private bool TryGetTransformations(Type packetType, TransformationType type, [MaybeNullWhen(false)] out MinecraftPacketTransformation[] transformations)
    {
        if (Registries.PacketTransformationsSystem.All.TryGetTransformations(packetType, type, out transformations))
            return true;

        if (Registries.PacketIdPlugins.TryGetTransformations(Registries.PacketTransformationsPlugins, packetType, type, out transformations))
            return true;

        return false;
    }
}
