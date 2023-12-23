using System.Buffers;

namespace Void.Proxy.Network.IO;

public readonly struct MinecraftMessage(int packetId, Memory<byte> memory, IMemoryOwner<byte> owner) : IDisposable
{
    public int PacketId { get; } = packetId;
    public int Length { get; } = memory.Length;
    public Memory<byte> Memory { get; } = memory;
    public void Dispose() => owner.Dispose();
}