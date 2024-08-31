using Void.Proxy.API.Network.IO.Messages.Binary;

namespace Void.Proxy.API.Network.IO.Streams.Manual.Binary;

public interface IMinecraftBufferedMessageStream : IMinecraftManualStream, IMinecraftStream
{
    public IBufferedBinaryMessage ReadAsMessage(int maxSize = 2048);
    public ValueTask<IBufferedBinaryMessage> ReadAsMessageAsync(int maxSize = 2048, CancellationToken cancellationToken = default);
    public void WriteAsMessage(IBufferedBinaryMessage message);
    public ValueTask WriteAsMessageAsync(IBufferedBinaryMessage message, CancellationToken cancellationToken = default);
}