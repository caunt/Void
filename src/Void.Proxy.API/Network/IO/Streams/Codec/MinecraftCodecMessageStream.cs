using System.Buffers;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams.Extensions;

namespace Void.Proxy.API.Network.IO.Streams.Codec;

public class MinecraftCodecMessageStream : IMinecraftPacketMessageStream
{
    public IMinecraftStreamBase? BaseStream { get; set; }

    public IMinecraftPacket ReadPacket()
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream stream => DecodeNetwork(stream),
            // IMinecraftCompleteMessageStream stream => DecodeCompleteMessage(stream),
            _ => throw new NotImplementedException()
        };
    }

    public async ValueTask<IMinecraftPacket> ReadPacketAsync()
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream stream => await DecodeNetworkAsync(stream),
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

    public async ValueTask WritePacketAsync(IMinecraftPacket packet)
    {
        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                await EncodeNetworkAsync(networkStream, packet);
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

    public async ValueTask FlushAsync()
    {
        if (BaseStream != null)
            await BaseStream.FlushAsync();
    }

    public void Close()
    {
        BaseStream?.Close();
    }

    private static IMinecraftPacket DecodeNetwork(IMinecraftNetworkStream stream)
    {
        var length = stream.ReadVarInt();
        var memoryOwner = MemoryPool<byte>.Shared.Rent(length);
        var memory = memoryOwner.Memory[..length];
        stream.ReadExactly(memory.Span);
        return ToBinaryPacket(memory, memoryOwner);
    }

    private static async ValueTask<IMinecraftPacket> DecodeNetworkAsync(IMinecraftNetworkStream stream)
    {
        var length = await stream.ReadVarIntAsync();
        var memoryOwner = MemoryPool<byte>.Shared.Rent(length);
        var memory = memoryOwner.Memory[..length];
        await stream.ReadExactlyAsync(memory);
        return ToBinaryPacket(memory, memoryOwner);
    }

    private static void EncodeNetwork(IMinecraftNetworkStream stream, IMinecraftPacket packet)
    {
        if (packet is not BinaryPacket binaryPacket)
            throw new NotImplementedException();

        stream.WriteVarInt(binaryPacket.Memory.Length + MinecraftBuffer.GetVarIntSize(binaryPacket.Id));
        stream.WriteVarInt(binaryPacket.Id);
        stream.Write(binaryPacket.Memory.Span);
    }

    private static async ValueTask EncodeNetworkAsync(IMinecraftNetworkStream stream, IMinecraftPacket packet)
    {
        if (packet is not BinaryPacket binaryPacket)
            throw new NotImplementedException();

        await stream.WriteVarIntAsync(binaryPacket.Memory.Length + MinecraftBuffer.GetVarIntSize(binaryPacket.Id));
        await stream.WriteVarIntAsync(binaryPacket.Id);
        await stream.WriteAsync(binaryPacket.Memory);
    }

    public static BinaryPacket ToBinaryPacket(Memory<byte> memory, IMemoryOwner<byte> memoryOwner)
    {
        var buffer = new MinecraftBuffer(memory.Span);
        var id = buffer.ReadVarInt();

        return new BinaryPacket(id, memory[buffer.Position..], memoryOwner);
    }
}