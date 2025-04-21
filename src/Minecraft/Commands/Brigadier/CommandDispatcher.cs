using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Brigadier.Tree;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier;

public record CommandDispatcher(RootCommandNode Root) : ICommandDispatcher
{
    public const char ArgumentSeparator = ' ';

    private const char UsageOptionalOpen = '[';
    private const char UsageOptionalClose = ']';
    private const char UsageRequiredOpen = '(';
    private const char UsageRequiredClose = ')';
    private const char UsageOr = '|';

    public ResultConsumer Consumer { get; set; } = (context, success, result) => { };

    public CommandDispatcher() : this(new RootCommandNode())
    {
        // Empty
    }

    public void Add(ICommandNode node)
    {
        if (node is not CommandNode commandNode)
            throw new ArgumentException($"Node must be a {nameof(CommandNode)}.");

        Root.AddChild(commandNode);
    }

    public LiteralCommandNode Register(LiteralArgumentBuilder command)
    {
        var build = command.Build();
        Root.AddChild(build);
        return build;
    }

    public LiteralCommandNode Register(Func<IArgumentContext, LiteralArgumentBuilder> command)
    {
        var build = command(default(ArgumentContext)).Build();
        Root.AddChild(build);
        return build;
    }

    public IEnumerable<CommandNode> All(CommandNode? root = null)
    {
        root ??= Root;

        foreach (var child in root.Children)
        {
            yield return child;

            foreach (var grandChild in All(child))
                yield return grandChild;
        }
    }

    public async ValueTask<Suggestions> SuggestAsync(string input, ICommandSource source, CancellationToken cancellationToken)
    {
        return await SuggestAsync(new StringReader(input), source, cancellationToken);
    }

    public async ValueTask<Suggestions> SuggestAsync(StringReader input, ICommandSource source, CancellationToken cancellationToken)
    {
        var parse = await ParseAsync(input, source, cancellationToken);
        return await GetCompletionSuggestions(parse, cancellationToken);
    }

    public async ValueTask<int> ExecuteAsync(string input, ICommandSource source, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(new StringReader(input), source, cancellationToken);
    }

    public async ValueTask<int> ExecuteAsync(StringReader input, ICommandSource source, CancellationToken cancellationToken)
    {
        var parse = await ParseAsync(input, source, cancellationToken);
        return await ExecuteAsync(parse, cancellationToken);
    }

    public async ValueTask<int> ExecuteAsync(ParseResults parse, CancellationToken cancellationToken)
    {
        if (parse.Reader.CanRead)
        {
            if (parse.Exceptions.Count is 1)
                throw parse.Exceptions.Values.First();
            else if (parse.Context.Range.IsEmpty)
                throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownCommand.CreateWithContext(parse.Reader);
            else
                throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument.CreateWithContext(parse.Reader);
        }

        var command = parse.Reader.Source;
        var original = parse.Context.Build(command);

        var flatContext = ContextChain.TryFlatten(original);

        if (flatContext is null)
        {
            Consumer(original, false, 0);
            throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownCommand.CreateWithContext(parse.Reader);
        }

        return await flatContext.ExecuteAllAsync(original.Source, Consumer, cancellationToken);
    }

    public async ValueTask<ParseResults> Parse(string command, ICommandSource source, CancellationToken cancellationToken)
    {
        return await ParseAsync(new StringReader(command), source, cancellationToken);
    }

    public async ValueTask<ParseResults> ParseAsync(StringReader command, ICommandSource source, CancellationToken cancellationToken)
    {
        var context = new CommandContextBuilder(this, source, Root, command.Cursor);
        return await ParseNodesAsync(Root, command, context, cancellationToken);
    }

    private async ValueTask<ParseResults> ParseNodesAsync(CommandNode node, StringReader originalReader, CommandContextBuilder contextSoFar, CancellationToken cancellationToken)
    {
        var source = contextSoFar.Source;
        var errors = new Dictionary<CommandNode, CommandSyntaxException>();
        var potentials = new List<ParseResults>();
        var cursor = originalReader.Cursor;

        foreach (var child in node.GetRelevantNodes(originalReader))
        {
            if (!await child.CanUseAsync(source, cancellationToken))
                continue;

            var context = contextSoFar.Copy();
            var reader = new StringReader(originalReader);

            try
            {
                try
                {
                    child.Parse(reader, context);
                }
                catch (Exception exception)
                {
                    throw CommandSyntaxException.BuiltInExceptions.DispatcherParseException.CreateWithContext(reader, exception.Message);
                }

                if (reader.CanRead)
                {
                    if (reader.Peek != ArgumentSeparator)
                        throw CommandSyntaxException.BuiltInExceptions.DispatcherExpectedArgumentSeparator.CreateWithContext(reader);
                }
            }
            catch (CommandSyntaxException exception)
            {
                errors[child] = exception;
                reader.Cursor = cursor;
                continue;
            }

            context.WithExecutor(child.Executor);

            if (reader.CanReadLength(child.RedirectTarget is null ? 2 : 1))
            {
                reader.Skip();

                if (child.RedirectTarget is not null)
                {
                    var childContext = new CommandContextBuilder(this, source, child.RedirectTarget, reader.Cursor);
                    var parse = await ParseNodesAsync(child.RedirectTarget, reader, childContext, cancellationToken);

                    context.WithChild(parse.Context);

                    return new ParseResults(context, parse.Reader, parse.Exceptions);
                }
                else
                {
                    var parse = await ParseNodesAsync(child, reader, context, cancellationToken);
                    potentials.Add(parse);
                }
            }
            else
            {
                potentials.Add(new ParseResults(context, reader, []));
            }
        }

        if (potentials.Count > 0)
        {
            if (potentials.Count > 1)
            {
                potentials.Sort((a, b) =>
                {
                    if (!a.Reader.CanRead && b.Reader.CanRead)
                    {
                        return -1;
                    }
                    if (a.Reader.CanRead && !b.Reader.CanRead)
                    {
                        return 1;
                    }
                    if (a.Exceptions.Count == 0 && b.Exceptions.Count > 0)
                    {
                        return -1;
                    }
                    if (a.Exceptions.Count > 0 && b.Exceptions.Count == 0)
                    {
                        return 1;
                    }
                    return 0;
                });
            }

            return potentials[0];
        }

        return new ParseResults(contextSoFar, originalReader, errors);
    }

    public async ValueTask<string[]> GetAllUsageAsync(CommandNode node, ICommandSource source, bool restricted, CancellationToken cancellationToken)
    {
        var result = new List<string>();
        await GetAllUsageAsync(node, source, result, "", restricted, cancellationToken);
        return [.. result];
    }

    private async ValueTask GetAllUsageAsync(CommandNode node, ICommandSource source, List<string> result, string prefix, bool restricted, CancellationToken cancellationToken)
    {
        if (restricted && !await node.CanUseAsync(source, cancellationToken))
            return;

        if (node.Executor is not null)
            result.Add(prefix);

        if (node.RedirectTarget is not null)
        {
            var redirect = node.RedirectTarget == Root ? "..." : "=> " + node.RedirectTarget.UsageText;
            result.Add(prefix.Length is 0 ? node.UsageText + ArgumentSeparator + redirect : prefix + ArgumentSeparator + redirect);
        }
        else if (node.Children.Any())
        {
            foreach (var child in node.Children)
                await GetAllUsageAsync(child, source, result, prefix.Length is 0 ? child.UsageText : prefix + ArgumentSeparator + child.UsageText, restricted, cancellationToken);
        }
    }

    public async ValueTask<Dictionary<CommandNode, string>> GetSmartUsageAsync(CommandNode node, ICommandSource source, CancellationToken cancellationToken)
    {
        var result = new Dictionary<CommandNode, string>();
        var optional = node.Executor is not null;

        foreach (var child in node.Children)
        {
            var usage = await GetSmartUsageAsync(child, source, optional, false, cancellationToken);

            if (usage is not null)
                result[child] = usage;
        }

        return result;
    }

    private async ValueTask<string?> GetSmartUsageAsync(CommandNode node, ICommandSource source, bool optional, bool deep, CancellationToken cancellationToken)
    {
        if (!await node.CanUseAsync(source, cancellationToken))
            return null;

        var self = optional ? UsageOptionalOpen + node.UsageText + UsageOptionalClose : node.UsageText;
        var childOptional = node.Executor is not null;
        var open = childOptional ? UsageOptionalOpen : UsageRequiredOpen;
        var close = childOptional ? UsageOptionalClose : UsageRequiredClose;

        if (!deep)
        {
            if (node.RedirectTarget is not null)
            {
                var redirect = node.RedirectTarget == Root ? "..." : "=> " + node.RedirectTarget.UsageText;
                return self + ArgumentSeparator + redirect;
            }
            else
            {
                var childsUsable = await Task.WhenAll(node.Children.Select(async child => (child, await child.CanUseAsync(source, cancellationToken))));
                var children = childsUsable.Where(pair => pair.Item2).Select(pair => pair.child);

                if (children.Count() == 1)
                {
                    var usage = await GetSmartUsageAsync(children.First(), source, childOptional, childOptional, cancellationToken);

                    if (usage is not null)
                        return self + ArgumentSeparator + usage;
                }
                else if (children.Count() > 1)
                {
                    var childUsage = new List<string>();

                    foreach (var child in children)
                    {
                        var usage = await GetSmartUsageAsync(child, source, childOptional, true, cancellationToken);

                        if (usage is not null)
                            childUsage.Add(usage);
                    }
                    if (childUsage.Count == 1)
                    {
                        var usage = childUsage.First();
                        return self + ArgumentSeparator + (childOptional ? UsageOptionalOpen + usage + UsageOptionalClose : usage);
                    }
                    else if (childUsage.Count > 1)
                    {
                        var builder = new StringBuilder(open);
                        var count = 0;

                        foreach (var child in children)
                        {
                            if (count > 0)
                                builder.Append(UsageOr);

                            builder.Append(child.UsageText);
                            count++;
                        }

                        if (count > 0)
                        {
                            builder.Append(close);
                            return self + ArgumentSeparator + builder.ToString();
                        }
                    }
                }
            }
        }

        return self;
    }

    public static async ValueTask<Suggestions> GetCompletionSuggestions(ParseResults parse, CancellationToken cancellationToken)
    {
        return await GetCompletionSuggestions(parse, parse.Reader.TotalLength, cancellationToken);
    }

    public static async ValueTask<Suggestions> GetCompletionSuggestions(ParseResults parse, int cursor, CancellationToken cancellationToken)
    {
        var context = parse.Context;

        var nodeBeforeCursor = context.BuildSuggestions(cursor);
        var parent = nodeBeforeCursor.Parent;
        var start = Math.Min(nodeBeforeCursor.Start, cursor);

        var fullInput = parse.Reader.Source;
        var truncatedInput = fullInput[..cursor];
        var truncatedInputLowerCase = truncatedInput.ToLower();
        var suggestions = await Task.WhenAll(parent.Children.Select(async node => await node.ListSuggestionsAsync(context.Build(truncatedInput), new SuggestionsBuilder(truncatedInput, start), cancellationToken)));

        return Suggestions.Merge(fullInput, suggestions);
    }

    public List<string> GetPath(CommandNode target)
    {
        var nodes = new List<List<CommandNode>>();
        AddPaths(Root, nodes, []);

        foreach (var list in nodes)
        {
            if (list[^1] == target)
            {
                var result = new List<string>();

                foreach (var node in list)
                {
                    if (node != Root)
                        result.Add(node.Name);
                }

                return result;
            }
        }

        return [];
    }

    public CommandNode? FindNode(List<string> path)
    {
        var node = Root as CommandNode;

        foreach (var name in path)
        {
            node = node.GetChild(name);

            if (node is null)
                return null;
        }

        return node;
    }

    private static void AddPaths(CommandNode node, List<List<CommandNode>> result, List<CommandNode> current)
    {
        current.Add(node);
        result.Add([.. current]);

        foreach (var child in node.Children)
            AddPaths(child, result, current);

        current.RemoveAt(current.Count - 1);
    }


    private bool HasCommand(CommandNode node)
    {
        return node is { Executor: not null } || node.Children.Any(HasCommand);
    }
}
