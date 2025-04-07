using Microsoft.IO;

namespace Void.Proxy.Api.Network.IO.Messages.Binary;

public interface ICompleteBinaryMessage : IMinecraftMessage
{
    public RecyclableMemoryStream Stream { get; }
}
