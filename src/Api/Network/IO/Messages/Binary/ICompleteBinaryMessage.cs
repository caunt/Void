using Microsoft.IO;
using Void.Common.Network.Messages;

namespace Void.Proxy.Api.Network.IO.Messages.Binary;

public interface ICompleteBinaryMessage : INetworkMessage
{
    public RecyclableMemoryStream Stream { get; }
}
