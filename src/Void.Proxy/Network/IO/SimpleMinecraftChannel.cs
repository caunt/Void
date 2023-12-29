using System.Buffers;
using Void.Proxy.API.Network.IO;

namespace Void.Proxy.Network.IO;

public class SimpleMinecraftChannel(Stream baseStream) : IMinecraftChannel
{
    public bool CanRead => baseStream.CanRead;
    public bool CanWrite => baseStream.CanWrite;

    private Memory<byte>? _nextBuffer;

    public void Inject(Memory<byte> buffer)
    {
        if (_nextBuffer.HasValue)
            _nextBuffer = ((byte[])[.. _nextBuffer.Value.ToArray(), .. buffer.ToArray()]).AsMemory(); // TODO do not allocate
        else
            _nextBuffer = buffer;
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        await baseStream.FlushAsync(cancellationToken);
    }

    public async ValueTask<MinecraftMessage> ReadMessageAsync(CancellationToken cancellationToken = default)
    {
        var memoryOwner = MemoryPool<byte>.Shared.Rent(4096); // pass from stack above?
        var memory = memoryOwner.Memory;

        int length;
        if (_nextBuffer.HasValue)
        {
            var buffer = _nextBuffer.Value;

            if (buffer.Length <= memory.Length)
                length = buffer.Length;
            else
                length = memory.Length;

            buffer.CopyTo(memory); // can be not copied?

            if (buffer.Length == length)
                _nextBuffer = null;
            else
                _nextBuffer = buffer[length..];
        }
        else
        {
            length = await baseStream.ReadAsync(memory, cancellationToken);
        }

        return new(memory[..length], memoryOwner);
    }

    public async ValueTask WriteMessageAsync(MinecraftMessage message, CancellationToken cancellationToken = default)
    {
        await baseStream.WriteAsync(message.Memory, cancellationToken);
    }
}