using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Events.Chat;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Network;

namespace Void.Proxy.Commands;

public class CommandService(IEventService events) : ICommandService
{
    private readonly CommandDispatcher _dispatcher = new();

    public ICommandDispatcher Dispatcher => _dispatcher;

    public async ValueTask<CommandExecutionResult> ExecuteAsync(ICommandSource source, string command, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(source, command, Side.Proxy, cancellationToken);
    }

    public async ValueTask<CommandExecutionResult> ExecuteAsync(ICommandSource source, string command, Side origin, CancellationToken cancellationToken = default)
    {
        if (command.StartsWith('/'))
            command = command[1..];

        try
        {
            if (source is IMinecraftPlayer player)
                player.GetLogger().LogInformation("Entered command: {Command}", command);

            _ = await _dispatcher.ExecuteAsync(command, source, cancellationToken);
            return CommandExecutionResult.Executed;
        }
        catch (CommandSyntaxException exception) when (exception.Message.Contains("Unknown command"))
        {
            // Ignore unknown commands
            if (source is IMinecraftPlayer player)
                await events.ThrowAsync(new ChatCommandSendEvent(player, command, origin), cancellationToken);

            return CommandExecutionResult.Forwarded;
        }
        catch (CommandSyntaxException exception)
        {
            if (source is IMinecraftPlayer player)
                await player.SendChatMessageAsync(exception.Message, cancellationToken);

            return CommandExecutionResult.Exception;
        }
    }

    public async ValueTask<string[]> CompleteAsync(string input, ICommandSource source, CancellationToken cancellationToken = default)
    {
        var suggestions = await _dispatcher.SuggestAsync(input, source, cancellationToken);
        return [.. suggestions.All.Select(suggestion => suggestion.Text)];
    }
}
