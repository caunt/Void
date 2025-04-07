namespace Void.Common.Network.Streams;

public interface INetworkStream : INetworkStreamBase
{
    public INetworkStreamBase? BaseStream { get; set; }
}
