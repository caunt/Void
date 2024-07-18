using Void.Proxy.API.Network.IO.Messages;

namespace Void.Proxy.API.Network.IO.Streams;

public interface IMinecraftBufferedMessageStream : IMinecraftManualStream, IMinecraftStream
{
    public BufferedBinaryMessage ReadAsMessage(int maxSize = 2048);
    public ValueTask<BufferedBinaryMessage> ReadAsMessageAsync(int maxSize = 2048);
    public void WriteAsMessage(BufferedBinaryMessage message);
    public ValueTask WriteAsMessageAsync(BufferedBinaryMessage message);
}