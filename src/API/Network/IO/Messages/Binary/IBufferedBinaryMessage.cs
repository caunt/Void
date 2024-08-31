using Microsoft.IO;

namespace Void.Proxy.API.Network.IO.Messages.Binary;

public interface IBufferedBinaryMessage : IMinecraftMessage
{
    public RecyclableMemoryStream Stream { get; }
}