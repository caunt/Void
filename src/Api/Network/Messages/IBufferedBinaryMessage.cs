using Microsoft.IO;

namespace Void.Proxy.Api.Network.Messages;

public interface IBufferedBinaryMessage : INetworkMessage
{
    public RecyclableMemoryStream Stream { get; }
}
