using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Plugins.Common.Events;

public record LoginPluginMessageEvent(IPlayer Player, ILink Link, string Channel, byte[] Data) : IScopedEventWithResult<bool>
{
    /// <summary>
    /// Gets or sets the response data as a byte array.
    /// </summary>
    public byte[]? Response { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the processing on the client was successful.
    /// </summary>
    public bool Successful { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the response should be sent.
    /// </summary>
    public bool Result { get; set; }
}
