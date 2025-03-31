using Void.Minecraft.Commands;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Commands;

public class CommandService(ILogger<CommandService> logger, IPlayerService players, IHostApplicationLifetime hostApplicationLifetime) : ICommandService
{
    private readonly CommandDispatcher _dispatcher = new();

    public void RegisterDefault()
    {
        _dispatcher.Register(builder => builder.Literal("stop").Executes(StopServer));
        _dispatcher.Register(builder => builder.Literal("kick").Then(builder.Argument("name", Arguments.String()).Executes(KickPlayerAsync)));
    }

    public async ValueTask ExecuteAsync(ICommandSource source, string command, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.ExecuteAsync(command, source, cancellationToken);
    }

    private int StopServer(CommandContext context)
    {
        logger.LogInformation("Stopping proxy...");
        hostApplicationLifetime.StopApplication();
        return 1;
    }

    private async ValueTask<int> KickPlayerAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (players.TryGetByName(context.GetArgument<string>("name"), out var player))
        {
            await players.KickPlayerAsync(player, "You have been kicked by an operator.", cancellationToken);
            return 1;
        }

        return 0;
    }
}
