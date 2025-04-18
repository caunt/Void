namespace Void.Proxy.Api.Network.Streams;

public interface IMessageStream : IMessageStreamBase
{
    public IMessageStreamBase? BaseStream { get; set; }
}
