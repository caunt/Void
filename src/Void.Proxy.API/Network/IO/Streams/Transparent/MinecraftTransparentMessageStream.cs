using System.Buffers;
using Void.Proxy.API.Network.IO.Messages;

namespace Void.Proxy.API.Network.IO.Streams.Transparent;

public class MinecraftTransparentMessageStream : IMinecraftBufferedMessageStream
{
    public IMinecraftStreamBase? BaseStream { get; set; }

    public int Read(Span<byte> span)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException();

        return stream.Read(span);
    }

    public async ValueTask<int> ReadAsync(Memory<byte> memory)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException();

        return await stream.ReadAsync(memory);
    }

    public void ReadExactly(Span<byte> span)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException();

        stream.ReadExactly(span);
    }

    public async ValueTask ReadExactlyAsync(Memory<byte> memory)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException();

        await stream.ReadExactlyAsync(memory);
    }

    public void Write(Span<byte> span)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException();

        stream.Write(span);
    }

    public async ValueTask WriteAsync(Memory<byte> memory)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException();

        await stream.WriteAsync(memory);
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

    public BufferedBinaryMessage ReadAsMessage(int maxSize = 2048)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException();

        using var memoryOwner = MemoryPool<byte>.Shared.Rent(maxSize);
        var memory = memoryOwner.Memory[..maxSize];
        var length = Read(memory.Span);

        return new BufferedBinaryMessage(memory[..length], memoryOwner);
    }

    public async ValueTask<BufferedBinaryMessage> ReadAsMessageAsync(int maxSize = 2048)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException();

        using var memoryOwner = MemoryPool<byte>.Shared.Rent(maxSize);
        var memory = memoryOwner.Memory[..maxSize];
        var length = await ReadAsync(memory);

        return new BufferedBinaryMessage(memory[..length], memoryOwner);
    }

    public void WriteAsMessage(BufferedBinaryMessage message)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException();

        Write(message.Memory.Span);
    }

    public async ValueTask WriteAsMessageAsync(BufferedBinaryMessage message)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException();

        await WriteAsync(message.Memory);
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
}