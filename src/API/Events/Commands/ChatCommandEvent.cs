using Void.Proxy.API.Links;

namespace Void.Proxy.API.Events.Commands;

public record ChatCommandEvent(ILink Link, string Command, bool IsSigned) : IEventWithResult<bool>
{
    /// <summary>
    ///     <see langword="true" /> if command should not be sent to Server
    /// </summary>
    public bool Result { get; set; }
}
