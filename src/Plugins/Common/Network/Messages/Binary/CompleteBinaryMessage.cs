using Microsoft.IO;
using Void.Proxy.Api.Network.Messages;

namespace Void.Proxy.Plugins.Common.Network.Messages.Binary;

public class CompleteBinaryMessage(RecyclableMemoryStream recyclableMemoryStream) : ICompleteBinaryMessage
{
    public RecyclableMemoryStream Stream => recyclableMemoryStream;

    public void Dispose()
    {
        recyclableMemoryStream.Dispose();
        GC.SuppressFinalize(this);
    }
}
