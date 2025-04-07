using Microsoft.IO;
using Void.Common;

namespace Void.Proxy.Api.Network.IO.Messages.Binary;

public interface IBufferedBinaryMessage : INetworkMessage
{
    public RecyclableMemoryStream Stream { get; }
}
