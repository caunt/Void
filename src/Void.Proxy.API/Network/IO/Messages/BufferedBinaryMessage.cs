using Microsoft.IO;

namespace Void.Proxy.API.Network.IO.Messages;

public class BufferedBinaryMessage(RecyclableMemoryStream stream) : IMinecraftMessage
{
    public RecyclableMemoryStream Stream => stream;

    public void Dispose()
    {
        stream.Dispose();
    }
}