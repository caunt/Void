using System.Buffers;
using System.Security.Cryptography;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Network.Streams;
using Void.Proxy.Api.Network.Streams.Manual;
using Void.Proxy.Api.Network.Streams.Manual.Binary;
using Void.Proxy.Api.Network.Streams.Recyclable;
using Void.Proxy.Plugins.Common.Network.Messages.Binary;

namespace Void.Proxy.Plugins.Common.Network.Streams.Encryption;

public class AesCfb8BufferedStream : RecyclableStream, IBufferedMessageStream
{
    private const int BlockSize = 16;

    private readonly Aes _aes;
    private readonly byte[] _readStreamIv;
    private readonly byte[] _writeStreamIv;

    public AesCfb8BufferedStream(byte[] key)
    {
        _aes = Aes.Create();
        _aes.Key = key;

        _readStreamIv = new byte[key.Length];
        _writeStreamIv = new byte[key.Length];

        Array.Copy(key, _readStreamIv, _readStreamIv.Length);
        Array.Copy(key, _writeStreamIv, _writeStreamIv.Length);
    }

    public IMessageStreamBase? BaseStream { get; set; }
    public bool CanRead => BaseStream?.CanRead ?? false;
    public bool CanWrite => BaseStream?.CanWrite ?? false;
    public bool IsAlive => BaseStream?.IsAlive ?? false;

    public int Read(Span<byte> output)
    {
        return BaseStream switch
        {
            IManualStream manualStream => DecryptManual(manualStream, output),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public async ValueTask<int> ReadAsync(Memory<byte> output, CancellationToken cancellationToken = default)
    {
        return BaseStream switch
        {
            IManualStream manualStream => await DecryptManualAsync(manualStream, output, cancellationToken),
            _ => throw new NotSupportedException(BaseStream?.GetType().FullName)
        };
    }

    public void ReadExactly(Span<byte> output)
    {
        switch (BaseStream)
        {
            case IManualStream manualStream:
                DecryptManualExactly(manualStream, output);
                break;
            default:
                throw new NotSupportedException(BaseStream?.GetType().FullName);
        }
    }

    public async ValueTask ReadExactlyAsync(Memory<byte> output, CancellationToken cancellationToken = default)
    {
        switch (BaseStream)
        {
            case IManualStream manualStream:
                await DecryptManualExactlyAsync(manualStream, output, cancellationToken);
                break;
            default:
                throw new NotSupportedException(BaseStream?.GetType().FullName);
        }
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        if (BaseStream is not IManualStream manualStream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

        using var memoryOwner = MemoryPool<byte>.Shared.Rent(data.Length);
        var memory = memoryOwner.Memory[..data.Length];
        Encrypt(data, memory.Span);

        manualStream.Write(memory.Span);
    }

    public async ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        if (BaseStream is not IManualStream manualStream)
            throw new NotSupportedException(BaseStream?.GetType().FullName);

        using var memoryOwner = MemoryPool<byte>.Shared.Rent(data.Length);
        var memory = memoryOwner.Memory[..data.Length];
        Encrypt(data.Span, memory.Span);

        await manualStream.WriteAsync(memory, cancellationToken);
    }

    public IBufferedBinaryMessage ReadAsMessage(int size = 2048)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();

        switch (BaseStream)
        {
            case IManualStream manualStream:
                DecryptManualExactly(manualStream, stream.GetSpan(size));
                break;
            default:
                throw new NotSupportedException(BaseStream?.GetType().FullName);
        }

        return new BufferedBinaryMessage(stream);
    }

    public async ValueTask<IBufferedBinaryMessage> ReadAsMessageAsync(int size = 2048, CancellationToken cancellationToken = default)
    {
        var stream = RecyclableMemoryStreamManager.GetStream();

        switch (BaseStream)
        {
            case IManualStream manualStream:
                await DecryptManualExactlyAsync(manualStream, stream.GetMemory(size), cancellationToken);
                break;
            default:
                throw new NotSupportedException(BaseStream?.GetType().FullName);
        }

        return new BufferedBinaryMessage(stream);
    }

    public void WriteAsMessage(IBufferedBinaryMessage message)
    {
        foreach (var memory in message.Stream.GetReadOnlySequence())
            Write(memory.Span);
    }

    public async ValueTask WriteAsMessageAsync(IBufferedBinaryMessage message, CancellationToken cancellationToken = default)
    {
        foreach (var memory in message.Stream.GetReadOnlySequence())
            await WriteAsync(memory, cancellationToken);
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

    public void Dispose()
    {
        _aes.Dispose();
        BaseStream?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        _aes.Dispose();
        if (BaseStream != null)
            await BaseStream.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    private int DecryptManual(IManualStream stream, Span<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var span = memoryOwner.Memory[..output.Length].Span;
        var length = stream.Read(span);
        Decrypt(span, output);
        return length;
    }

    private async Task<int> DecryptManualAsync(IManualStream stream, Memory<byte> output, CancellationToken cancellationToken = default)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        var length = await stream.ReadAsync(memory, cancellationToken);
        Decrypt(memory.Span, output.Span);
        return length;
    }

    private void DecryptManualExactly(IManualStream stream, Span<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        stream.ReadExactly(memory.Span);
        Decrypt(memory.Span, output);
    }

    private async Task DecryptManualExactlyAsync(IManualStream stream, Memory<byte> output, CancellationToken cancellationToken = default)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        await stream.ReadExactlyAsync(memory, cancellationToken);
        Decrypt(memory.Span, output.Span);
    }

    private int EncryptManual(IManualStream stream, Span<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var span = memoryOwner.Memory[..output.Length].Span;
        var length = stream.Read(span);
        Encrypt(span, output);
        return length;
    }

    private async Task<int> EncryptManualAsync(IManualStream stream, Memory<byte> output, CancellationToken cancellationToken = default)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        var length = await stream.ReadAsync(memory, cancellationToken);
        Encrypt(memory.Span, output.Span);
        return length;
    }

    private void EncryptManualExactly(IManualStream stream, Span<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        stream.ReadExactly(memory.Span);
        Encrypt(memory.Span, output);
    }

    private async Task EncryptManualExactlyAsync(IManualStream stream, Memory<byte> output, CancellationToken cancellationToken = default)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        await stream.ReadExactlyAsync(memory, cancellationToken);
        Encrypt(memory.Span, output.Span);
    }

    private void Encrypt(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Span<byte> buffer = stackalloc byte[BlockSize];
        for (var idx = 0; idx < input.Length; ++idx)
        {
            _aes.EncryptEcb(_writeStreamIv, buffer, PaddingMode.None);

            var current = (byte)(buffer[0] ^ input[idx]);
            output[idx] = current;

            _writeStreamIv.AsSpan(1, BlockSize - 1).CopyTo(_writeStreamIv);
            _writeStreamIv[BlockSize - 1] = current;
        }
    }

    private void Decrypt(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Span<byte> buffer = stackalloc byte[BlockSize];
        for (var idx = 0; idx < output.Length; idx++)
        {
            _aes.EncryptEcb(_readStreamIv, buffer, PaddingMode.None);

            var current = (byte)(buffer[0] ^ input[idx]);
            output[idx] = current;

            _readStreamIv.AsSpan(1, BlockSize - 1).CopyTo(_readStreamIv);
            _readStreamIv[BlockSize - 1] = input[idx];
        }
    }
}
