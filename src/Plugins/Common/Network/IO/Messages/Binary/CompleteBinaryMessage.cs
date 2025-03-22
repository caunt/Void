using Microsoft.IO;
using Void.Proxy.API.Network.IO.Messages.Binary;

namespace Void.Proxy.Plugins.Common.Network.IO.Messages.Binary;

public class CompleteBinaryMessage(RecyclableMemoryStream recyclableMemoryStream) : ICompleteBinaryMessage
{
    public RecyclableMemoryStream Stream => recyclableMemoryStream;

    public void Dispose()
    {
        recyclableMemoryStream.Dispose();
        GC.SuppressFinalize(this);
    }
}