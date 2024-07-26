using System.Buffers;
using System.Security.Cryptography;

namespace Void.Proxy.Network.IO.Encryption;

public class AesCfb8Stream : Stream
{
    private const int BLOCK_SIZE = 16;
    private readonly Aes _aes;
    private readonly byte[] _readStreamIV;
    private readonly byte[] _writeStreamIV;

    private readonly Stream _baseStream;

    public AesCfb8Stream(Stream stream, byte[] key)
    {
        _aes = Aes.Create();
        _aes.Key = key;

        _baseStream = stream;
        _readStreamIV = new byte[key.Length];
        _writeStreamIV = new byte[key.Length];

        Array.Copy(key, _readStreamIV, _readStreamIV.Length);
        Array.Copy(key, _writeStreamIV, _writeStreamIV.Length);
    }

    public override bool CanRead => _baseStream.CanRead;

    public override bool CanSeek => _baseStream.CanSeek;

    public override bool CanWrite => _baseStream.CanWrite;

    public override long Length => _baseStream.Length;

    public override long Position
    {
        get => _baseStream.Position;
        set => _baseStream.Position = value;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _baseStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _baseStream.SetLength(value);
    }

    public override void Flush()
    {
        _baseStream.Flush();
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        await _baseStream.FlushAsync(cancellationToken);
    }

    public override int ReadByte()
    {
        throw new NotSupportedException("Use async methods instead");
    }

    public override int Read(byte[] buffer, int offset, int length)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(length);
        var memory = memoryOwner.Memory[..length].Span;
        _baseStream.ReadExactly(memory);
        Decrypt(memory, buffer.AsSpan(offset, length));
        return length;
    }

    public override int Read(Span<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length].Span;
        _baseStream.ReadExactly(memory);
        Decrypt(memory, output);
        return output.Length;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> output, CancellationToken cancellationToken = default)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        await _baseStream.ReadExactlyAsync(memory, cancellationToken);
        Decrypt(memory.Span, output.Span);
        return output.Length;
    }

    public override void Write(byte[] input, int offset, int length)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(input.Length);
        var memory = memoryOwner.Memory[..input.Length].Span;
        Encrypt(input.AsSpan(offset, length), memory);
        _baseStream.Write(memory);
    }

    public override void Write(ReadOnlySpan<byte> input)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(input.Length);
        var memory = memoryOwner.Memory[..input.Length].Span;
        Encrypt(input, memory);
        _baseStream.Write(memory);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> input, CancellationToken cancellationToken = default)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(input.Length);
        var memory = memoryOwner.Memory[..input.Length];
        Encrypt(input.Span, memory.Span);
        await _baseStream.WriteAsync(memory, cancellationToken);
    }

    private void Encrypt(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Span<byte> buffer = stackalloc byte[BLOCK_SIZE];
        for (var idx = 0; idx < input.Length; ++idx)
        {
            _aes.EncryptEcb(_writeStreamIV, buffer, PaddingMode.None);

            var current = (byte)(buffer[0] ^ input[idx]);
            output[idx] = current;

            Buffer.BlockCopy(_writeStreamIV, 1, _writeStreamIV, 0, BLOCK_SIZE - 1);
            _writeStreamIV[BLOCK_SIZE - 1] = current;
        }
    }

    private void Decrypt(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Span<byte> buffer = stackalloc byte[BLOCK_SIZE];
        for (var idx = 0; idx < output.Length; idx++)
        {
            _aes.EncryptEcb(_readStreamIV, buffer, PaddingMode.None);

            var current = (byte)(buffer[0] ^ input[idx]);
            output[idx] = current;

            Buffer.BlockCopy(_readStreamIV, 1, _readStreamIV, 0, BLOCK_SIZE - 1);
            _readStreamIV[BLOCK_SIZE - 1] = input[idx];
        }
    }
}