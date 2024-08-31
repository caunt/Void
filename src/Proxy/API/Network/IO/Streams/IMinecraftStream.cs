namespace Void.Proxy.API.Network.IO.Streams;

public interface IMinecraftStream : IMinecraftStreamBase
{
    public IMinecraftStreamBase? BaseStream { get; set; }
}