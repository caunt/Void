using Void.Proxy.API.Network.IO.Messages.Binary;
using Void.Proxy.API.Network.IO.Streams;
using Void.Proxy.API.Network.IO.Streams.Manual.Binary;
using Void.Proxy.API.Network.IO.Streams.Manual.Network;
using Void.Proxy.API.Network.IO.Streams.Recyclable;
using Void.Proxy.Common.Network.IO.Messages;

namespace Void.Proxy.Common.Network.IO.Streams.Transparent;

public class MinecraftTransparentMessageStream : MinecraftRecyclableStream, IMinecraftBufferedMessageStream
{
    public IMinecraftStreamBase? BaseStream { get; set; }

    public int Read(Span<byte> span)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

        return stream.Read(span);
    }

    public async ValueTask<int> ReadAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

        return await stream.ReadAsync(memory, cancellationToken);
    }

    public void ReadExactly(Span<byte> span)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

        stream.ReadExactly(span);
    }

    public async ValueTask ReadExactlyAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

        await stream.ReadExactlyAsync(memory, cancellationToken);
    }

    public void Write(ReadOnlySpan<byte> span)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

        stream.Write(span);
    }

    public async ValueTask WriteAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
    {
        if (BaseStream is not IMinecraftNetworkStream stream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

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

    public IBufferedBinaryMessage ReadAsMessage(int size = 2048)
    {
        if (BaseStream is not IMinecraftNetworkStream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

        var stream = RecyclableMemoryStreamManager.GetStream();
        var length = Read(stream.GetSpan(size));

        stream.Advance(length);

        return new BufferedBinaryMessage(stream);
    }

    public async ValueTask<IBufferedBinaryMessage> ReadAsMessageAsync(int size = 2048, CancellationToken cancellationToken = default)
    {
        if (BaseStream is not IMinecraftNetworkStream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

        var stream = RecyclableMemoryStreamManager.GetStream();
        var length = await ReadAsync(stream.GetMemory(size), cancellationToken);

        stream.Advance(length);

        return new BufferedBinaryMessage(stream);
    }

    public void WriteAsMessage(IBufferedBinaryMessage message)
    {
        if (BaseStream is not IMinecraftNetworkStream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

        foreach (var chunk in message.Stream.GetReadOnlySequence())
            Write(chunk.Span);
    }

    public async ValueTask WriteAsMessageAsync(IBufferedBinaryMessage message, CancellationToken cancellationToken = default)
    {
        if (BaseStream is not IMinecraftNetworkStream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

        foreach (var chunk in message.Stream.GetReadOnlySequence())
            await WriteAsync(chunk, cancellationToken);
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