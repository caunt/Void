using System.Net.Sockets;
using Void.Proxy.Api.Network.Streams.Manual.Network;

namespace Void.Benchmarks.Streams;

internal class MinecraftMemoryStream : INetworkStream
{
    private readonly MemoryStream _memoryStream = new();
    public NetworkStream BaseStream => throw new NotSupportedException();
    public bool CanRead => true;
    public bool CanWrite => true;
    public bool IsAlive => true;

    public void PrependBuffer(Memory<byte> buffer)
    {
        throw new NotSupportedException();
    }

    public void Dispose()
    {
        _memoryStream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _memoryStream.DisposeAsync();
    }

    public void Flush()
    {
        _memoryStream.Flush();
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        await _memoryStream.FlushAsync(cancellationToken);
    }

    public void Close()
    {
        _memoryStream.Close();
    }

    public int Read(Span<byte> span)
    {
        return _memoryStream.Read(span);
    }

    public async ValueTask<int> ReadAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
    {
        return await _memoryStream.ReadAsync(memory, cancellationToken);
    }

    public void ReadExactly(Span<byte> span)
    {
        _memoryStream.ReadExactly(span);
    }

    public async ValueTask ReadExactlyAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
    {
        await _memoryStream.ReadExactlyAsync(memory, cancellationToken);
    }

    public void Write(ReadOnlySpan<byte> span)
    {
        _memoryStream.Write(span);
    }

    public async ValueTask WriteAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
    {
        await _memoryStream.WriteAsync(memory, cancellationToken);
    }

    public void Reset(int length = -1)
    {
        _memoryStream.Position = 0;

        if (length >= 0)
            _memoryStream.SetLength(length);
    }
}
