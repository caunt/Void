using MinecraftProxy.Network.IO.Encryption;
using System.Buffers;
using System.Net.Sockets;

namespace MinecraftProxy.Network.IO;

public class MinecraftChannel(Stream baseStream)
{
    public bool CanRead => baseStream.CanRead;
    public bool CanWrite => baseStream.CanWrite;

    public void EnableEncryption(byte[] secret)
    {
        if (baseStream is not NetworkStream)
            throw new InvalidOperationException($"Encryption can be enabled only on NetworkStream but channel have {baseStream.GetType()}");

        baseStream = new AesCfb8Stream(baseStream, secret);
    }

    public void EnableCompression(int threshold)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<MinecraftMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
    {
        var length = await baseStream.ReadVarIntAsync(cancellationToken);

        var memoryOwner = MemoryPool<byte>.Shared.Rent(length);
        var memory = memoryOwner.Memory[..length];

        await baseStream.ReadExactlyAsync(memory, cancellationToken);

        return new(memory, memoryOwner);
    }

    public async ValueTask WriteMessageAsync(MinecraftMessage message, CancellationToken cancellationToken = default)
    {
        var length = message.Memory.Length;

        await baseStream.WriteVarIntAsync(length, cancellationToken);
        await baseStream.WriteAsync(message.Memory, cancellationToken);
        await baseStream.FlushAsync(cancellationToken);
    }
}