using Microsoft.IO;

namespace Void.Proxy.Api.Network.Messages;

/// <summary>
/// Represents a complete binary message stored in a recyclable memory stream.
/// </summary>
public interface ICompleteBinaryMessage : INetworkMessage
{
    /// <summary>
    /// Gets the stream containing the complete message payload.
    /// </summary>
    public RecyclableMemoryStream Stream { get; }
}
