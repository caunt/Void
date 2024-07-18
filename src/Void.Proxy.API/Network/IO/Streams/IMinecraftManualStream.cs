namespace Void.Proxy.API.Network.IO.Streams;

public interface IMinecraftManualStream
{
    public int Read(Span<byte> span);
    public ValueTask<int> ReadAsync(Memory<byte> memory);
    public void ReadExactly(Span<byte> span);
    public ValueTask ReadExactlyAsync(Memory<byte> memory);
    public void Write(Span<byte> span);
    public ValueTask WriteAsync(Memory<byte> memory);
}