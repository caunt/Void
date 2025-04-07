using Void.Common.Network.Streams;
using Void.Proxy.Api.Network.IO.Messages.Binary;

namespace Void.Proxy.Api.Network.IO.Streams.Manual.Binary;

public interface IMinecraftBufferedMessageStream : IMinecraftManualStream, INetworkStream
{
    public IBufferedBinaryMessage ReadAsMessage(int maxSize = 2048);
    public ValueTask<IBufferedBinaryMessage> ReadAsMessageAsync(int maxSize = 2048, CancellationToken cancellationToken = default);
    public void WriteAsMessage(IBufferedBinaryMessage message);
    public ValueTask WriteAsMessageAsync(IBufferedBinaryMessage message, CancellationToken cancellationToken = default);
}
