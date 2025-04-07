namespace Void.Common.Network.Streams;

public interface IMessageStream : IMessageStreamBase
{
    public IMessageStreamBase? BaseStream { get; set; }
}
