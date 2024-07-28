using Void.Proxy.API.Network.IO.Messages;

namespace Void.Proxy.API.Network.IO.Streams;

public interface IMinecraftCompleteMessageStream : IMinecraftStream
{
    public CompleteBinaryMessage ReadMessage();
    public ValueTask<CompleteBinaryMessage> ReadMessageAsync(CancellationToken cancellationToken = default);
    public void WriteMessage(CompleteBinaryMessage message);
    public ValueTask WriteMessageAsync(CompleteBinaryMessage message, CancellationToken cancellationToken = default);
}