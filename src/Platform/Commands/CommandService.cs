using Nito.Disposables.Internals;
using Void.Common.Commands;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Commands;

public class CommandService(ILogger<CommandService> logger, IPlayerService players, IHostApplicationLifetime hostApplicationLifetime) : ICommandService
{
    private readonly CommandDispatcher _dispatcher = new();

    public ICommandDispatcher Dispatcher => _dispatcher;

    public async ValueTask ExecuteAsync(ICommandSource source, string command, CancellationToken cancellationToken = default)
    {
        _ = await _dispatcher.ExecuteAsync(command, source, cancellationToken);
    }

    public async ValueTask<string[]> CompleteAsync(string input, ICommandSource source, CancellationToken cancellationToken = default)
    {
        var suggestions = await _dispatcher.SuggestAsync(input, source, cancellationToken);
        return [.. suggestions.All.Select(suggestion => suggestion.Text)];
    }

    private int StopServer(CommandContext context)
    {
        logger.LogInformation("Stopping proxy...");
        hostApplicationLifetime.StopApplication();
        return 1;
    }

    private async ValueTask<int> KickPlayerAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (players.TryGetByName(context.GetArgument<string>("username"), out var player))
        {
            if (!player.TryGetMinecraftPlayer(out var minecraftPlayer))
                return 0;

            await players.KickPlayerAsync(minecraftPlayer, "You have been kicked by an operator.", cancellationToken);
            return 1;
        }

        return 0;
    }

    private Suggestions SuggestPlayer(CommandContext context, SuggestionsBuilder builder)
    {
        return Suggestions.Create(context.Input, players.All.Select(player =>
        {
            if (!player.TryGetMinecraftPlayer(out var minecraftPlayer))
                return null;

            if (minecraftPlayer.Profile is null)
                return null;

            var name = minecraftPlayer.Profile.Username;
            return new Suggestion(StringRange.Between(0, context.Input.Length), name);
        }).WhereNotNull());
    }
}
