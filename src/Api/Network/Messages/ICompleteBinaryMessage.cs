using Microsoft.IO;

namespace Void.Proxy.Api.Network.Messages;

public interface ICompleteBinaryMessage : INetworkMessage
{
    public RecyclableMemoryStream Stream { get; }
}
