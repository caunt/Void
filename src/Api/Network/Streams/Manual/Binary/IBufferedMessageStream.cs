using Void.Proxy.Api.Network.Messages;

namespace Void.Proxy.Api.Network.Streams.Manual.Binary;

public interface IBufferedMessageStream : IManualStream, IMessageStream
{
    public IBufferedBinaryMessage ReadAsMessage(int maxSize = 2048);
    public ValueTask<IBufferedBinaryMessage> ReadAsMessageAsync(int maxSize = 2048, CancellationToken cancellationToken = default);
    public void WriteAsMessage(IBufferedBinaryMessage message);
    public ValueTask WriteAsMessageAsync(IBufferedBinaryMessage message, CancellationToken cancellationToken = default);
}
