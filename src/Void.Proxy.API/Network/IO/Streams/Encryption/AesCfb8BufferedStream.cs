﻿using System.Buffers;
using System.Security.Cryptography;
using Void.Proxy.API.Network.IO.Messages;

namespace Void.Proxy.API.Network.IO.Streams.Encryption;

public class AesCfb8BufferedStream : IMinecraftBufferedMessageStream
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

    public IMinecraftStreamBase? BaseStream { get; set; }

    public int Read(Span<byte> output)
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream networkStream => DecryptNetwork(networkStream, output),
            IMinecraftBufferedMessageStream bufferedStream => DecryptBuffer(bufferedStream, output),
            _ => throw new NotImplementedException()
        };
    }

    public async ValueTask<int> ReadAsync(Memory<byte> output)
    {
        return BaseStream switch
        {
            IMinecraftNetworkStream networkStream => await DecryptNetworkAsync(networkStream, output),
            IMinecraftBufferedMessageStream bufferedStream => await DecryptBufferAsync(bufferedStream, output),
            _ => throw new NotImplementedException()
        };
    }

    public void ReadExactly(Span<byte> output)
    {
        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                DecryptNetworkExactly(networkStream, output);
                break;
            case IMinecraftBufferedMessageStream bufferedStream:
                DecryptBufferExactly(bufferedStream, output);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public async ValueTask ReadExactlyAsync(Memory<byte> output)
    {
        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                await DecryptNetworkExactlyAsync(networkStream, output);
                break;
            case IMinecraftBufferedMessageStream bufferedStream:
                await DecryptBufferExactlyAsync(bufferedStream, output);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void Write(Span<byte> span)
    {
        // switch
        // using var memoryOwner = MemoryPool<byte>.Shared.Rent(span.Length);
        // var memory = memoryOwner.Memory[..span.Length];
        // Encrypt(span, memory.Span);
    }

    public ValueTask WriteAsync(Memory<byte> memory)
    {
        throw new NotImplementedException();
    }

    public void Flush()
    {
        BaseStream?.FlushAsync();
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

    public BufferedBinaryMessage ReadAsMessage(int size = 2048)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(size);
        var memory = memoryOwner.Memory[..size];

        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                DecryptNetworkExactly(networkStream, memory.Span);
                break;
            case IMinecraftBufferedMessageStream bufferedStream:
                DecryptBufferExactly(bufferedStream, memory.Span);
                break;
            default:
                throw new NotImplementedException();
        }

        return new BufferedBinaryMessage(memory, memoryOwner);
    }

    public async ValueTask<BufferedBinaryMessage> ReadAsMessageAsync(int size = 2048)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(size);
        var memory = memoryOwner.Memory[..size];

        switch (BaseStream)
        {
            case IMinecraftNetworkStream networkStream:
                await DecryptNetworkExactlyAsync(networkStream, memory);
                break;
            case IMinecraftBufferedMessageStream bufferedStream:
                await DecryptBufferExactlyAsync(bufferedStream, memory);
                break;
            default:
                throw new NotImplementedException();
        }

        return new BufferedBinaryMessage(memory, memoryOwner);
    }

    public void WriteAsMessage(BufferedBinaryMessage message)
    {
    }

    public async ValueTask WriteAsMessageAsync(BufferedBinaryMessage message)
    {
    }

    public void Dispose()
    {
        _aes.Dispose();
        BaseStream?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        _aes.Dispose();
        if (BaseStream != null)
            await BaseStream.DisposeAsync();
    }

    private int DecryptNetwork(IMinecraftNetworkStream stream, Span<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var span = memoryOwner.Memory[..output.Length].Span;
        var length = stream.Read(span);
        Decrypt(span, output);
        return length;
    }

    private int DecryptBuffer(IMinecraftBufferedMessageStream stream, Span<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var span = memoryOwner.Memory[..output.Length].Span;
        var length = stream.Read(span);
        Decrypt(span, output);
        return length;
    }

    private async Task<int> DecryptNetworkAsync(IMinecraftNetworkStream stream, Memory<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        var length = await stream.ReadAsync(memory);
        Decrypt(memory.Span, output.Span);
        return length;
    }

    private async Task<int> DecryptBufferAsync(IMinecraftBufferedMessageStream stream, Memory<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        var length = await stream.ReadAsync(memory);
        Decrypt(memory.Span, output.Span);
        return length;
    }

    private async Task DecryptNetworkExactlyAsync(IMinecraftNetworkStream stream, Memory<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        await stream.ReadExactlyAsync(memory);
        Decrypt(memory.Span, output.Span);
    }

    private async Task DecryptBufferExactlyAsync(IMinecraftBufferedMessageStream stream, Memory<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        await stream.ReadExactlyAsync(memory);
        Decrypt(memory.Span, output.Span);
    }

    private void DecryptNetworkExactly(IMinecraftNetworkStream stream, Span<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        stream.ReadExactly(memory.Span);
        Decrypt(memory.Span, output);
    }

    private void DecryptBufferExactly(IMinecraftBufferedMessageStream stream, Span<byte> output)
    {
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(output.Length);
        var memory = memoryOwner.Memory[..output.Length];
        stream.ReadExactly(memory.Span);
        Decrypt(memory.Span, output);
    }

    private void Encrypt(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Span<byte> buffer = stackalloc byte[BlockSize];
        for (var idx = 0; idx < input.Length; ++idx)
        {
            _aes.EncryptEcb(_writeStreamIv, buffer, PaddingMode.None);

            var current = (byte)(buffer[0] ^ input[idx]);
            output[idx] = current;

            Buffer.BlockCopy(_writeStreamIv, 1, _writeStreamIv, 0, BlockSize - 1);
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

            Buffer.BlockCopy(_readStreamIv, 1, _readStreamIv, 0, BlockSize - 1);
            _readStreamIv[BlockSize - 1] = input[idx];
        }
    }
}