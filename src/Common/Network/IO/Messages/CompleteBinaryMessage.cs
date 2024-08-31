using Microsoft.IO;
using Void.Proxy.API.Network.IO.Messages.Binary;

namespace Void.Proxy.Common.Network.IO.Messages;

public class CompleteBinaryMessage(RecyclableMemoryStream recyclableMemoryStream) : ICompleteBinaryMessage
{
    public RecyclableMemoryStream Stream => recyclableMemoryStream;

    public void Dispose()
    {
        recyclableMemoryStream.Dispose();
    }
}