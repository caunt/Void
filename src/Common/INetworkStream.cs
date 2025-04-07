namespace Void.Common;

public interface INetworkStream : INetworkStreamBase
{
    public INetworkStreamBase? BaseStream { get; set; }
}
