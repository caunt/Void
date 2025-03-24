using System.Net.Sockets;
using Void.Proxy.Api.Network.IO.Streams.Manual.Network;

namespace Void.Proxy.Plugins.Common.Network.IO.Streams.Network;

public class SimpleNetworkStream(NetworkStream baseStream) : IMinecraftNetworkStream
{
    private Memory<byte> _nextBuffer = Memory<byte>.Empty;
    private bool _isClosed = false;

    public NetworkStream BaseStream => baseStream;
    public bool CanRead => BaseStream.CanRead;
    public bool CanWrite => BaseStream.CanWrite;
    public bool IsAlive => IsAliveComplex();

    public void PrependBuffer(Memory<byte> buffer)
    {
        ArgumentOutOfRangeException.ThrowIfZero(buffer.Length, nameof(buffer));
        _nextBuffer = _nextBuffer.Length > 0 ? ((byte[])[.. _nextBuffer.ToArray(), .. buffer.ToArray()]).AsMemory() : buffer; // TODO do not allocate
    }

    public int Read(Span<byte> span)
    {
        if (_nextBuffer is not { Length: > 0 })
        {
            var read = baseStream.Read(span);
            return read > 0 ? read : throw new EndOfStreamException();
        }

        var length = Math.Min(_nextBuffer.Length, span.Length);
        _nextBuffer.Span[..length].CopyTo(span);
        _nextBuffer = _nextBuffer[length..];
        return length;
    }

    public async ValueTask<int> ReadAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
    {
        if (_nextBuffer is not { Length: > 0 })
        {
            var read = await baseStream.ReadAsync(memory, cancellationToken);
            return read > 0 ? read : throw new EndOfStreamException();
        }

        var length = Math.Min(_nextBuffer.Length, memory.Length);
        _nextBuffer[..length].CopyTo(memory);
        _nextBuffer = _nextBuffer[length..];
        return length;
    }

    public void ReadExactly(Span<byte> span)
    {
        if (_nextBuffer.Length >= span.Length)
        {
            _nextBuffer.Span[..span.Length].CopyTo(span);
            _nextBuffer = _nextBuffer[span.Length..];
        }
        else
        {
            var length = _nextBuffer.Length;
            _nextBuffer.Span.CopyTo(span[..length]);
            _nextBuffer = Memory<byte>.Empty;
            baseStream.ReadExactly(span[length..]);
        }
    }

    public async ValueTask ReadExactlyAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
    {
        if (_nextBuffer.Length >= memory.Length)
        {
            _nextBuffer[..memory.Length].CopyTo(memory);
            _nextBuffer = _nextBuffer[memory.Length..];
        }
        else
        {
            var length = _nextBuffer.Length;
            _nextBuffer.CopyTo(memory[..length]);
            _nextBuffer = Memory<byte>.Empty;
            await baseStream.ReadExactlyAsync(memory[length..], cancellationToken);
        }
    }

    public void Write(ReadOnlySpan<byte> span)
    {
        baseStream.Write(span);
    }

    public async ValueTask WriteAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
    {
        await baseStream.WriteAsync(memory, cancellationToken);
    }

    public void Flush()
    {
        baseStream.Flush();
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        await baseStream.FlushAsync(cancellationToken);
    }

    public void Close()
    {
        _isClosed = true;
        baseStream.Close();
    }

    public void Dispose()
    {
        _isClosed = true;
        baseStream.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        _isClosed = true;
        await baseStream.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    protected bool IsAliveComplex()
    {
        if (_isClosed)
            return false;

        var expected = baseStream.Socket.Available;

        if (expected > 0)
        {
            var buffer = new byte[expected];
            var received = baseStream.Read(buffer);

            if (expected != received)
                throw new IOException($"Socket read received length ({received}) is not equal to expected length ({expected})");

            PrependBuffer(buffer);
        }

        var available = baseStream.Socket.Available;
        var poll = baseStream.Socket.Poll(0, SelectMode.SelectRead);

        if (poll && available == 0)
            return false;

        return true;
    }
}