﻿namespace Void.Proxy.API.Network.IO.Streams;

public interface IMinecraftStreamBase : IDisposable, IAsyncDisposable
{
    public void Flush();
    public ValueTask FlushAsync(CancellationToken cancellationToken = default);
    public void Close();
}