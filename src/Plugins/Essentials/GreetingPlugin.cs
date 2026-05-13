using Microsoft.Extensions.Logging;
using Void.Minecraft.Components.Text;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Services.Lifecycle;

namespace Void.Proxy.Plugins.Essentials;

public class GreetingPlugin(ILogger<GreetingPlugin> logger) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_7_2, ProtocolVersion.Latest);

    public string Name => "GreetingPlugin";

    [Subscribe]
    public async ValueTask OnPlayerJoinedServer(PlayerJoinedServerEvent @event, CancellationToken cancellationToken)
    {
        var player = @event.Player;
        logger.LogInformation("Player {Player} joined server {Server}", player, @event.Server);

        // Send greeting message to the player
        var greeting = new Component("Welcome to the server, ")
            .Append(player.Name, TextColor.LightPurple)
            .Append("!", TextColor.White);

        // Use the LifecycleService's SendChatMessageAsync method to send the message
        var channel = await player.GetChannelAsync(cancellationToken);
        if (channel != null)
        {
            await channel.SendPacketAsync(new ChatMessagePacket { Message = greeting, Position = 1 }, cancellationToken);
        }
    }
}