using Microsoft.IO;

namespace Void.Proxy.API.Network.IO.Messages;

public class CompleteBinaryMessage(RecyclableMemoryStream recyclableMemoryStream) : IMinecraftMessage
{
    public RecyclableMemoryStream Stream => recyclableMemoryStream;

    public void Dispose()
    {
        recyclableMemoryStream.Dispose();
    }
}