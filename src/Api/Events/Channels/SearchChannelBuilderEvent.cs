using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Channels;

/// <summary>
/// Requests a channel builder capable of decoding a player's initial network data.
/// </summary>
/// <param name="Player">The player whose channel is being constructed.</param>
/// <param name="Buffer">The initial bytes available for protocol detection. Listeners must not retain the memory after event processing completes.</param>
public record SearchChannelBuilderEvent(IPlayer Player, Memory<byte> Buffer) : IScopedEventWithResult<ChannelBuilder>
{
    /// <summary>
    /// Gets or sets the channel builder selected by a listener.
    /// </summary>
    /// <value>The selected builder, or <see langword="null" /> when no listener recognizes the buffered protocol.</value>
    public ChannelBuilder? Result { get; set; }
}
