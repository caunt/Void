﻿namespace Void.Proxy.Api.Network.Streams.Manual;

public interface IManualStream
{
    public int Read(Span<byte> span);
    public ValueTask<int> ReadAsync(Memory<byte> memory, CancellationToken cancellationToken = default);
    public void ReadExactly(Span<byte> span);
    public ValueTask ReadExactlyAsync(Memory<byte> memory, CancellationToken cancellationToken = default);
    public void Write(ReadOnlySpan<byte> span);
    public ValueTask WriteAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default);
}
