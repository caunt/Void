using Microsoft.IO;
using Void.Proxy.API.Network.IO.Messages.Binary;

namespace Void.Proxy.Plugins.Common.Network.IO.Messages.Binary;

public class BufferedBinaryMessage(RecyclableMemoryStream stream) : IBufferedBinaryMessage
{
    public RecyclableMemoryStream Stream => stream;

    public void Dispose()
    {
        stream.Dispose();
    }
}