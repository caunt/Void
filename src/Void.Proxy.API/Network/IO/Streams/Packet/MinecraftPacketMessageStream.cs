using System.Buffers;
using Void.Proxy.API.Network.IO.Buffers;
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

    public IMinecraftPacket ReadPacket()
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream stream => DecodeNetwork(stream),
            // IMinecraftCompleteMessageStream stream => DecodeCompleteMessage(stream),
            _ => throw new NotImplementedException()
        };
    }

    public async ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default)
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream stream => await DecodeNetworkAsync(stream, cancellationToken),
            // IMinecraftCompleteMessageStream stream => await DecodeCompleteMessageAsync(stream),
            _ => throw new NotImplementedException()
        };
    }

    public void WritePacket(IMinecraftPacket packet)
    {
        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                EncodeNetwork(networkStream, packet);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public async ValueTask WritePacketAsync(IMinecraftPacket packet, CancellationToken cancellationToken = default)
    {
        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                await EncodeNetworkAsync(networkStream, packet, cancellationToken);
                break;
            default:
                throw new NotImplementedException();
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

    private IMinecraftPacket DecodeNetwork(IMinecraftNetworkStream stream)
    {
        var length = stream.ReadVarInt();
        var memoryOwner = MemoryPool<byte>.Shared.Rent(length);
        var memory = memoryOwner.Memory[..length];
        stream.ReadExactly(memory.Span);
        return ToPacket(memory, memoryOwner);
    }

    private async ValueTask<IMinecraftPacket> DecodeNetworkAsync(IMinecraftNetworkStream stream, CancellationToken cancellationToken = default)
    {
        var length = await stream.ReadVarIntAsync(cancellationToken);
        var memoryOwner = MemoryPool<byte>.Shared.Rent(length);
        var memory = memoryOwner.Memory[..length];
        await stream.ReadExactlyAsync(memory, cancellationToken);
        return ToPacket(memory, memoryOwner);
    }

    private void EncodeNetwork(IMinecraftNetworkStream stream, IMinecraftPacket packet)
    {
        var packetId = packet switch
        {
            BinaryPacket binaryPacket => binaryPacket.Id,
            _ when GetRegistry(true) is { } registry && registry.TryGetPacketId(packet, out var id) => id,
            _ => throw new InvalidOperationException($"Cannot find id for packet {packet}")
        };

        using var memoryOwner = packet is BinaryPacket ? null : MemoryPool<byte>.Shared.Rent(2048);

        var memory = memoryOwner switch
        {
            not null => memoryOwner.Memory,
            _ when packet is BinaryPacket binaryPacket => binaryPacket.Memory,
            _ => null
        };

        if (packet is not BinaryPacket)
            TryEncodePacket();

        stream.WriteVarInt(memory.Length + MinecraftBuffer.GetVarIntSize(packetId));
        stream.WriteVarInt(packetId);
        stream.Write(memory.Span);
        return;

        void TryEncodePacket() // divided into local function to support ref struct and Span
        {
            var buffer = new MinecraftBuffer(memory.Span);
            packet.Encode(ref buffer, ProtocolVersion.Latest);
            memory = memory[..buffer.Position];
        }
    }

    private async ValueTask EncodeNetworkAsync(IMinecraftNetworkStream stream, IMinecraftPacket packet, CancellationToken cancellationToken = default)
    {
        var packetId = packet switch
        {
            BinaryPacket binaryPacket => binaryPacket.Id,
            _ when GetRegistry(true) is { } registry && registry.TryGetPacketId(packet, out var id) => id,
            _ => throw new InvalidOperationException($"Cannot find id for packet {packet}")
        };

        using var memoryOwner = packet is BinaryPacket ? null : MemoryPool<byte>.Shared.Rent(2048);

        var memory = memoryOwner switch
        {
            not null => memoryOwner.Memory,
            _ when packet is BinaryPacket binaryPacket => binaryPacket.Memory,
            _ => null
        };

        if (packet is not BinaryPacket)
            TryEncodePacket();

        await stream.WriteVarIntAsync(memory.Length + MinecraftBuffer.GetVarIntSize(packetId), cancellationToken);
        await stream.WriteVarIntAsync(packetId, cancellationToken);
        await stream.WriteAsync(memory, cancellationToken);
        return;

        void TryEncodePacket() // divided into local function to support ref struct and Span
        {
            var buffer = new MinecraftBuffer(memory.Span);
            packet.Encode(ref buffer, ProtocolVersion.Latest);
            memory = memory[..buffer.Position];
        }
    }

    public IMinecraftPacket ToPacket(Memory<byte> memory, IMemoryOwner<byte> memoryOwner)
    {
        var buffer = new MinecraftBuffer(memory.Span);
        var id = buffer.ReadVarInt();

        memory = memory[buffer.Position..];

        var result = GetRegistry() switch
        {
            { } registry when registry.TryCreateDecoder(id, out var decoder) => TryDecodePacket(decoder),
            _ => new BinaryPacket(id, memory, memoryOwner)
        };

        return result;

        IMinecraftPacket TryDecodePacket(DecodeDelegate<IMinecraftPacket> decoder) // divided into local function to support ref struct and Span
        {
            var buffer = new MinecraftBuffer(memory.Span);
            var decodedPacket = decoder(ref buffer, ProtocolVersion.Latest);

            if (buffer.HasData)
                throw new IndexOutOfRangeException($"The packet was not fully read. Bytes read: {buffer.Position}, Total length: {buffer.Length}.");

            return decodedPacket;
        }
    }

    public IPacketRegistry? GetRegistry(bool writing = false)
    {
        return Flow switch
        {
            Direction.Clientbound when writing => RegistryHolder?.ServerboundRegistry,
            Direction.Serverbound when writing => RegistryHolder?.ClientboundRegistry,
            Direction.Clientbound when !writing => RegistryHolder?.ClientboundRegistry,
            Direction.Serverbound when !writing => RegistryHolder?.ServerboundRegistry,
            _ => null
        };
    }
}