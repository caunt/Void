using Void.Proxy.API.Network.IO.Memory;
using Void.Proxy.API.Network.IO.Messages;

namespace Void.Proxy.API.Network.IO.Streams.Transparent;

public class MinecraftTransparentMessageStream : IMinecraftBufferedMessageStream
{
    public IMinecraftStreamBase? BaseStream { get; set; }

    public int Read(Span<byte> span)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException(BaseStream?.GetType().FullName);

        return stream.Read(span);
    }

    public async ValueTask<int> ReadAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException(BaseStream?.GetType().FullName);

        return await stream.ReadAsync(memory, cancellationToken);
    }

    public void ReadExactly(Span<byte> span)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException(BaseStream?.GetType().FullName);

        stream.ReadExactly(span);
    }

    public async ValueTask ReadExactlyAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException(BaseStream?.GetType().FullName);

        await stream.ReadExactlyAsync(memory, cancellationToken);
    }

    public void Write(Span<byte> span)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException(BaseStream?.GetType().FullName);

        stream.Write(span);
    }

    public async ValueTask WriteAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotImplementedException(BaseStream?.GetType().FullName);

        await stream.WriteAsync(memory, cancellationToken);
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

    public BufferedBinaryMessage ReadAsMessage(int maxSize = 2048)
    {
        if (BaseStream is not IMinecraftNetworkStream)
            throw new NotImplementedException(BaseStream?.GetType().FullName);

        var holder = MemoryHolder.RentExact(maxSize);
        var length = Read(holder.Slice.Span);

        holder.Slice = holder.Slice[..length];

        return new BufferedBinaryMessage(holder);
    }

    public async ValueTask<BufferedBinaryMessage> ReadAsMessageAsync(int maxSize = 2048, CancellationToken cancellationToken = default)
    {
        if (BaseStream is not IMinecraftNetworkStream)
            throw new NotImplementedException(BaseStream?.GetType().FullName);

        var holder = MemoryHolder.RentExact(maxSize);
        var length = await ReadAsync(holder.Slice, cancellationToken);

        holder.Slice = holder.Slice[..length];

        return new BufferedBinaryMessage(holder);
    }

    public void WriteAsMessage(BufferedBinaryMessage message)
    {
        if (BaseStream is not IMinecraftNetworkStream)
            throw new NotImplementedException(BaseStream?.GetType().FullName);

        Write(message.Holder.Slice.Span);
    }

    public async ValueTask WriteAsMessageAsync(BufferedBinaryMessage message, CancellationToken cancellationToken = default)
    {
        if (BaseStream is not IMinecraftNetworkStream)
            throw new NotImplementedException(BaseStream?.GetType().FullName);

        await WriteAsync(message.Holder.Slice, cancellationToken);
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