using Microsoft.IO;

namespace Void.Proxy.API.Network.IO.Messages.Binary;

public interface IBinaryMessage : IMinecraftMessage
{
    public int Id { get; }
    public RecyclableMemoryStream Stream { get; }
}