using System.Reflection;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;
using Void.Minecraft.Events.Chat;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions.Reflection;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Commands;

public class CommandService(IEventService events) : ICommandService, IEventListener
{
    private readonly CommandDispatcher _dispatcher = new();

    public ICommandDispatcher Dispatcher => _dispatcher;

    [Subscribe]
    public void OnPluginUnloading(PluginUnloadingEvent @event)
    {
        lock (this)
            ClearByAssembly(@event.Plugin.GetType().Assembly);
    }

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
            if (source is IPlayer player)
                player.GetLogger().LogInformation("Entered command: {Command}", command);

            _ = await _dispatcher.ExecuteAsync(command, source, cancellationToken);
            return CommandExecutionResult.Executed;
        }
        catch (CommandSyntaxException exception) when (exception.Message.Contains("Unknown command"))
        {
            // Ignore unknown commands
            if (source is IPlayer player)
                await events.ThrowAsync(new ChatCommandSendEvent(player, command, origin), cancellationToken);

            return CommandExecutionResult.Forwarded;
        }
        catch (CommandSyntaxException exception)
        {
            if (source is IPlayer player)
                await player.SendChatMessageAsync(exception.Message, cancellationToken);

            return CommandExecutionResult.Exception;
        }
    }

    public async ValueTask<string[]> CompleteAsync(string input, ICommandSource source, CancellationToken cancellationToken = default)
    {
        var suggestions = await _dispatcher.SuggestAsync(input, source, cancellationToken);

        var result = new string[suggestions.All.Count];

        for (var i = 0; i < suggestions.All.Count; i++)
            result[i] = suggestions.All[i].Text;

        return result;
    }

    private void ClearByAssembly(Assembly assembly)
    {
        foreach (var node in _dispatcher.All())
        {
            if (node.Executor is not null)
            {
                foreach (var invocation in node.Executor.GetInvocationList().Cast<CommandExecutor>())
                {
                    if (invocation.Target is null)
                        continue;

                    var target = invocation.Target.GetFieldValue("command") switch
                    {
                        CommandExecutorSync value => value.Target,
                        CommandExecutor value => value.Target,
                        _ => invocation.Target
                    };

                    if (target is null)
                        continue;

                    if (target.GetType().Assembly != assembly)
                        continue;

                    node.Executor -= invocation;
                }
            }

            if (node is ArgumentCommandNode { CustomSuggestions: not null } argumentCommandNode)
            {
                foreach (var invocation in argumentCommandNode.CustomSuggestions.GetInvocationList().Cast<SuggestionProvider>())
                {
                    if (invocation.Target is null)
                        continue;

                    var target = invocation.Target.GetFieldValue("provider") switch
                    {
                        SuggestionProviderSync value => value.Target,
                        SuggestionProvider value => value.Target,
                        _ => invocation.Target
                    };

                    if (target is null)
                        continue;

                    if (target.GetType().Assembly != assembly)
                        continue;

                    argumentCommandNode.CustomSuggestions -= invocation;
                }
            }
        }
    }
}
