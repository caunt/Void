using Void.Common.Network.Streams;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Network.Streams.Manual;

namespace Void.Proxy.Api.Network.Streams.Manual.Binary;

public interface IBufferedMessageStream : IManualStream, INetworkStream
{
    public IBufferedBinaryMessage ReadAsMessage(int maxSize = 2048);
    public ValueTask<IBufferedBinaryMessage> ReadAsMessageAsync(int maxSize = 2048, CancellationToken cancellationToken = default);
    public void WriteAsMessage(IBufferedBinaryMessage message);
    public ValueTask WriteAsMessageAsync(IBufferedBinaryMessage message, CancellationToken cancellationToken = default);
}
