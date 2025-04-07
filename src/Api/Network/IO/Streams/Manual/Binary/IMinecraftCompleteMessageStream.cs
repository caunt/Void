using Void.Common.Network.Streams;
using Void.Proxy.Api.Network.IO.Messages.Binary;

namespace Void.Proxy.Api.Network.IO.Streams.Manual.Binary;

public interface IMinecraftCompleteMessageStream : INetworkStream
{
    public ICompleteBinaryMessage ReadMessage();
    public ValueTask<ICompleteBinaryMessage> ReadMessageAsync(CancellationToken cancellationToken = default);
    public void WriteMessage(ICompleteBinaryMessage message);
    public ValueTask WriteMessageAsync(ICompleteBinaryMessage message, CancellationToken cancellationToken = default);
}
