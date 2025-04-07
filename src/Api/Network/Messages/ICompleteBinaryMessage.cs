using Microsoft.IO;
using Void.Common.Network.Messages;

namespace Void.Proxy.Api.Network.Messages;

public interface ICompleteBinaryMessage : INetworkMessage
{
    public RecyclableMemoryStream Stream { get; }
}
