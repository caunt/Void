using Void.Proxy.API.Network.IO.Messages.Binary;

namespace Void.Proxy.API.Network.IO.Streams.Manual.Binary;

public interface IMinecraftCompleteMessageStream : IMinecraftStream
{
    public ICompleteBinaryMessage ReadMessage();
    public ValueTask<ICompleteBinaryMessage> ReadMessageAsync(CancellationToken cancellationToken = default);
    public void WriteMessage(ICompleteBinaryMessage message);
    public ValueTask WriteMessageAsync(ICompleteBinaryMessage message, CancellationToken cancellationToken = default);
}