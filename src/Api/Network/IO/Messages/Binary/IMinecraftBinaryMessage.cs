namespace Void.Proxy.Api.Network.IO.Messages.Binary;

public interface IMinecraftBinaryMessage : IMinecraftMessage
{
    public int Id { get; }
    public MemoryStream Stream { get; }
}
