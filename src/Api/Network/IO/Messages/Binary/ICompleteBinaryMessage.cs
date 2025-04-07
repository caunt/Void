using Microsoft.IO;
using Void.Common;

namespace Void.Proxy.Api.Network.IO.Messages.Binary;

public interface ICompleteBinaryMessage : INetworkMessage
{
    public RecyclableMemoryStream Stream { get; }
}
