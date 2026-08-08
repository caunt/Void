using Microsoft.IO;

namespace Void.Proxy.Api.Network.Messages;

/// <summary>
/// Represents a binary message whose data is accumulated in a recyclable memory stream.
/// </summary>
public interface IBufferedBinaryMessage : INetworkMessage
{
    /// <summary>
    /// Gets the stream containing the currently buffered message data.
    /// </summary>
    public RecyclableMemoryStream Stream { get; }
}
