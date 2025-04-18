using Void.Proxy.Api.Network.Messages;

namespace Void.Proxy.Api.Network.Streams.Manual.Binary;

public interface ICompleteMessageStream : IMessageStream
{
    public ICompleteBinaryMessage ReadMessage();
    public ValueTask<ICompleteBinaryMessage> ReadMessageAsync(CancellationToken cancellationToken = default);
    public void WriteMessage(ICompleteBinaryMessage message);
    public ValueTask WriteMessageAsync(ICompleteBinaryMessage message, CancellationToken cancellationToken = default);
}
