using System.Net.Sockets;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Exceptions;
using Void.Proxy.Api.Network.Streams.Manual.Network;

namespace Void.Proxy.Plugins.Common.Network.Streams.Network;

public class SimpleNetworkStream(NetworkStream baseStream, TimeSpan writeTimeout, TimeSpan readTimeout) : INetworkStream
{
    private Memory<byte> _nextBuffer = Memory<byte>.Empty;
    private bool _isClosed = false;

    public NetworkStream BaseStream => baseStream;
    public TimeSpan WriteTimeout => writeTimeout;
    public TimeSpan ReadTimeout => readTimeout;
    public bool CanRead => BaseStream.CanRead;
    public bool CanWrite => BaseStream.CanWrite;
    public bool IsAlive => IsAliveComplex();

    public SimpleNetworkStream(NetworkStream baseStream) : this(baseStream, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15))
    {
        // Intentionally left blank
    }

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
            return read > 0 ? read : throw new StreamClosedException();
        }

        var length = Math.Min(_nextBuffer.Length, span.Length);
        _nextBuffer.Span[..length].CopyTo(span);
        _nextBuffer = _nextBuffer[length..];
        return length;
    }

    public async ValueTask<int> ReadAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithTimeout(async (cancellationToken) =>
        {
            if (_nextBuffer is not { Length: > 0 })
            {
                var read = await baseStream.ReadAsync(memory, cancellationToken);
                return read > 0 ? read : throw new StreamClosedException();
            }

            var length = Math.Min(_nextBuffer.Length, memory.Length);
            _nextBuffer[..length].CopyTo(memory);
            _nextBuffer = _nextBuffer[length..];
            return length;
        }, Operation.Read, cancellationToken);
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
            try
            {
                var length = _nextBuffer.Length;
                _nextBuffer.Span.CopyTo(span[..length]);
                _nextBuffer = Memory<byte>.Empty;
                baseStream.ReadExactly(span[length..]);
            }
            catch (Exception exception) when (exception is EndOfStreamException or IOException { InnerException: SocketException } or ObjectDisposedException)
            {
                throw new StreamClosedException();
            }
        }
    }

    public async ValueTask ReadExactlyAsync(Memory<byte> memory, CancellationToken cancellationToken = default)
    {
        await ExecuteWithTimeout(async (cancellationToken) =>
        {
            if (_nextBuffer.Length >= memory.Length)
            {
                _nextBuffer[..memory.Length].CopyTo(memory);
                _nextBuffer = _nextBuffer[memory.Length..];
            }
            else
            {
                try
                {
                    var length = _nextBuffer.Length;
                    _nextBuffer.CopyTo(memory[..length]);
                    _nextBuffer = Memory<byte>.Empty;
                    await baseStream.ReadExactlyAsync(memory[length..], cancellationToken);
                }
                catch (Exception exception) when (exception is EndOfStreamException or IOException { InnerException: SocketException } or ObjectDisposedException)
                {
                    throw new StreamClosedException();
                }
            }
        }, Operation.Read, cancellationToken);
    }

    public void Write(ReadOnlySpan<byte> span)
    {
        try
        {
            baseStream.Write(span);
        }
        catch (Exception exception) when (exception is EndOfStreamException or IOException { InnerException: SocketException } or ObjectDisposedException)
        {
            throw new StreamClosedException();
        }
    }

    public async ValueTask WriteAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
    {
        await ExecuteWithTimeout(async (cancellationToken) =>
        {
            try
            {
                await baseStream.WriteAsync(memory, cancellationToken);
            }
            catch (Exception exception) when (exception is EndOfStreamException or IOException { InnerException: SocketException } or ObjectDisposedException)
            {
                throw new StreamClosedException();
            }
        }, Operation.Write, cancellationToken);
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
            {
                if (received is 0)
                    throw new StreamClosedException();

                throw new IOException($"Socket read received length ({received}) is not equal to expected length ({expected})");
            }

            PrependBuffer(buffer);
        }

        var available = baseStream.Socket.Available;
        var poll = baseStream.Socket.Poll(0, SelectMode.SelectRead);

        if (poll && available == 0)
            return false;

        return true;
    }

    private async ValueTask ExecuteWithTimeout(Func<CancellationToken, ValueTask> function, Operation operation, CancellationToken cancellationToken)
    {
        await ExecuteWithTimeout<int>(async linkedCancellationToken =>
        {
            await function(linkedCancellationToken);
            return 0;
        }, operation, cancellationToken);
    }

    private async ValueTask<TResult> ExecuteWithTimeout<TResult>(Func<CancellationToken, ValueTask<TResult>> function, Operation operation, CancellationToken cancellationToken)
    {
        using var operationCancellationTokenSource = new CancellationTokenSource();
        operationCancellationTokenSource.CancelAfter(operation is Operation.Read ? readTimeout : writeTimeout);

        using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, operationCancellationTokenSource.Token);

        try
        {
            return await function(linkedCancellationTokenSource.Token);
        }
        catch (OperationCanceledException) when (operationCancellationTokenSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new StreamTimeoutException(operation);
        }
    }
}
