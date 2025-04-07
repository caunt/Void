using Microsoft.IO;
using Void.Proxy.Api.Network.Messages;

namespace Void.Proxy.Plugins.Common.Network.Messages.Binary;

public class BufferedBinaryMessage(RecyclableMemoryStream stream) : IBufferedBinaryMessage
{
    public RecyclableMemoryStream Stream => stream;

    public void Dispose()
    {
        stream.Dispose();
        GC.SuppressFinalize(this);
    }
}
