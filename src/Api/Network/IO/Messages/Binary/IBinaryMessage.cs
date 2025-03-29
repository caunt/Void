namespace Void.Proxy.Api.Network.IO.Messages.Binary;

public interface IBinaryMessage : IMinecraftMessage
{
    public int Id { get; }
    public MemoryStream Stream { get; }
}