namespace Void.Proxy.Api.Network.IO.Streams;

public interface IMinecraftStream : IMinecraftStreamBase
{
    public IMinecraftStreamBase? BaseStream { get; set; }
}