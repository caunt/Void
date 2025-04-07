using Microsoft.IO;

namespace Void.Proxy.Api.Network.IO.Messages.Binary;

public interface IBufferedBinaryMessage : IMinecraftMessage
{
    public RecyclableMemoryStream Stream { get; }
}
