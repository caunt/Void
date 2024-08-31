using Microsoft.IO;

namespace Void.Proxy.API.Network.IO.Messages.Binary;

public interface ICompleteBinaryMessage : IMinecraftMessage
{
    public RecyclableMemoryStream Stream { get; }
}