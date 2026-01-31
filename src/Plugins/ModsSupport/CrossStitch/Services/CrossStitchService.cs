using Microsoft.Extensions.Logging;
using Void.Minecraft.Events.Chat;
using Void.Proxy.Api.Events;

namespace Void.Proxy.Plugins.ModsSupport.CrossStitch.Services;

public class CrossStitchService(ILogger<CrossStitchService> logger) : IEventListener
{
    [Subscribe]
    public void OnAvailableCommands(AvailableCommandsEvent @event)
    {
        logger.LogDebug("Available commands: {Commands}", @event.Node);
    }
}
