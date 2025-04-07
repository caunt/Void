using Microsoft.IO;
using Void.Common.Network.Messages;

namespace Void.Proxy.Api.Network.IO.Messages.Binary;

public interface IBufferedBinaryMessage : INetworkMessage
{
    public RecyclableMemoryStream Stream { get; }
}
