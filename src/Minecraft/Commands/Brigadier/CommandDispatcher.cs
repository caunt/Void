using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Brigadier.Tree;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier;

public class CommandDispatcher(RootCommandNode root)
{
    public const string ArgumentSeparator = " ";
    public const char ArgumentSeparatorChar = ' ';

    private const string UsageOptionalOpen = "[";
    private const string UsageOptionalClose = "]";
    private const string UsageRequiredOpen = "(";
    private const string UsageRequiredClose = ")";
    private const string UsageOr = "|";

    public ResultConsumer Consumer { get; set; } = (context, success, result) => { };

    public CommandDispatcher() : this(new RootCommandNode())
    {
        // Empty
    }

    public LiteralCommandNode Register(LiteralArgumentBuilder command)
    {
        var build = command.Build();
        root.AddChild(build);
        return build;
    }

    public LiteralCommandNode Register(Func<IArgumentContext, LiteralArgumentBuilder> command)
    {
        var build = command(default(ArgumentContext)).Build();
        root.AddChild(build);
        return build;
    }

    public async ValueTask<int> Execute(string input, ICommandSource source)
    {
        return await Execute(new StringReader(input), source);
    }

    public async ValueTask<int> Execute(StringReader input, ICommandSource source)
    {
        var parse = await Parse(input, source);
        return await Execute(parse);
    }

    public async ValueTask<int> Execute(ParseResults parse)
    {
        if (parse.Reader is { CanRead: true })
        {
            if (parse.Exceptions is { Count: 1 })
            {
                throw parse.Exceptions.Values.First();
            }
            else if (parse.Context.Range.IsEmpty)
            {
                throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownCommand.CreateWithContext(parse.Reader);
            }
            else
            {
                throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument.CreateWithContext(parse.Reader);
            }
        }

        var command = parse.Reader.Source;
        var original = parse.Context.Build(command);

        var flatContext = ContextChain.TryFlatten(original);
        if (flatContext is null)
        {
            Consumer(original, false, 0);
            throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownCommand.CreateWithContext(parse.Reader);
        }

        return await flatContext.ExecuteAllAsync(original.Source, Consumer);
    }

    public async ValueTask<ParseResults> Parse(string command, ICommandSource source)
    {
        return await Parse(new StringReader(command), source);
    }

    public async ValueTask<ParseResults> Parse(StringReader command, ICommandSource source)
    {
        var context = new CommandContextBuilder(this, source, root, command.Cursor);
        return await ParseNodes(root, command, context);
    }

    private async ValueTask<ParseResults> ParseNodes(CommandNode node, StringReader originalReader, CommandContextBuilder contextSoFar)
    {
        var source = contextSoFar.Source;
        var errors = new Dictionary<CommandNode, CommandSyntaxException>();
        var potentials = new List<ParseResults>();
        int cursor = originalReader.Cursor;

        foreach (var child in node.GetRelevantNodes(originalReader))
        {
            if (!await child.CanUseAsync(source))
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
                    if (reader.Peek != ArgumentSeparatorChar)
                    {
                        throw CommandSyntaxException.BuiltInExceptions.DispatcherExpectedArgumentSeparator.CreateWithContext(reader);
                    }
                }
            }
            catch (CommandSyntaxException exception)
            {
                errors[child] = exception;
                reader.Cursor = cursor;
                continue;
            }

            context.WithExecutor(child.Executor);
            if (reader.CanReadLength(child.RedirectTarget == null ? 2 : 1))
            {
                reader.Skip();
                if (child.RedirectTarget != null)
                {
                    var childContext = new CommandContextBuilder(this, source, child.RedirectTarget, reader.Cursor);
                    var parse = await ParseNodes(child.RedirectTarget, reader, childContext);
                    context.WithChild(parse.Context);
                    return new ParseResults(context, parse.Reader, parse.Exceptions);
                }
                else
                {
                    var parse = await ParseNodes(child, reader, context);
                    potentials.Add(parse);
                }
            }
            else
            {
                potentials.Add(new ParseResults(context, reader));
            }
        }

        if (potentials != null)
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

    public async ValueTask<string[]> GetAllUsage(CommandNode node, ICommandSource source, bool restricted)
    {
        var result = new List<string>();
        await GetAllUsage(node, source, result, "", restricted);
        return [.. result];
    }

    private async ValueTask GetAllUsage(CommandNode node, ICommandSource source, List<string> result, string prefix, bool restricted)
    {
        if (restricted && !await node.CanUseAsync(source))
            return;

        if (node.Executor != null)
            result.Add(prefix);

        if (node.RedirectTarget != null)
        {
            var redirect = node.RedirectTarget == root ? "..." : "=> " + node.RedirectTarget.UsageText;
            result.Add(prefix.Length == 0 ? node.UsageText + ArgumentSeparator + redirect : prefix + ArgumentSeparator + redirect);
        }
        else if (node.Children.Any())
        {
            foreach (var child in node.Children)
            {
                await GetAllUsage(child, source, result, prefix.Length == 0 ? child.UsageText : prefix + ArgumentSeparator + child.UsageText, restricted);
            }
        }
    }

    public async ValueTask<Dictionary<CommandNode, string>> GetSmartUsage(CommandNode node, ICommandSource source)
    {
        var result = new Dictionary<CommandNode, string>();

        var optional = node.Executor != null;
        foreach (var child in node.Children)
        {
            var usage = await GetSmartUsage(child, source, optional, false);

            if (usage != null)
                result[child] = usage;
        }
        return result;
    }

    private async ValueTask<string?> GetSmartUsage(CommandNode node, ICommandSource source, bool optional, bool deep)
    {
        if (!await node.CanUseAsync(source))
            return null;

        string self = optional ? UsageOptionalOpen + node.UsageText + UsageOptionalClose : node.UsageText;
        bool childOptional = node.Executor != null;
        string open = childOptional ? UsageOptionalOpen : UsageRequiredOpen;
        string close = childOptional ? UsageOptionalClose : UsageRequiredClose;

        if (!deep)
        {
            if (node.RedirectTarget != null)
            {
                string redirect = node.RedirectTarget == root ? "..." : "=> " + node.RedirectTarget.UsageText;
                return self + ArgumentSeparator + redirect;
            }
            else
            {
                var children = node.Children.Where(c => c.CanUseAsync(source).Result);
                if (children.Count() == 1)
                {
                    var usage = await GetSmartUsage(children.First(), source, childOptional, childOptional);
                    if (usage != null)
                    {
                        return self + ArgumentSeparator + usage;
                    }
                }
                else if (children.Count() > 1)
                {
                    var childUsage = new List<string>();
                    foreach (var child in children)
                    {
                        var usage = await GetSmartUsage(child, source, childOptional, true);
                        if (usage != null)
                        {
                            childUsage.Add(usage);
                        }
                    }
                    if (childUsage.Count == 1)
                    {
                        string usage = childUsage.First();
                        return self + ArgumentSeparator + (childOptional ? UsageOptionalOpen + usage + UsageOptionalClose : usage);
                    }
                    else if (childUsage.Count > 1)
                    {
                        var builder = new StringBuilder(open);
                        int count = 0;
                        foreach (CommandNode child in children)
                        {
                            if (count > 0)
                            {
                                builder.Append(UsageOr);
                            }
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

    public async ValueTask<Suggestions> GetCompletionSuggestions(ParseResults parse)
    {
        return await GetCompletionSuggestions(parse, parse.Reader.TotalLength);
    }

    public async ValueTask<Suggestions> GetCompletionSuggestions(ParseResults parse, int cursor)
    {
        CommandContextBuilder context = parse.Context;

        SuggestionContext nodeBeforeCursor = context.BuildSuggestions(cursor);
        CommandNode parent = nodeBeforeCursor.Parent;
        int start = Math.Min(nodeBeforeCursor.Start, cursor);

        string fullInput = parse.Reader.Source;
        string truncatedInput = fullInput.Substring(0, cursor);
        string truncatedInputLowerCase = truncatedInput.ToLower();
        var suggestions = await Task.WhenAll(parent.Children.Select(async node => await node.ListSuggestionsAsync(context.Build(truncatedInput), new SuggestionsBuilder(truncatedInput, start))));

        return Suggestions.Merge(fullInput, suggestions);
    }

    public RootCommandNode GetRoot()
    {
        return root;
    }

    public List<string> GetPath(CommandNode target)
    {
        var nodes = new List<List<CommandNode>>();
        AddPaths(root, nodes, new List<CommandNode>());

        foreach (List<CommandNode> list in nodes)
        {
            if (list[^1] == target)
            {
                var result = new List<string>();
                foreach (CommandNode node in list)
                {
                    if (node != root)
                    {
                        result.Add(node.Name);
                    }
                }
                return result;
            }
        }

        return [];
    }

    public CommandNode? FindNode(List<string> path)
    {
        CommandNode node = root;
        foreach (string name in path)
        {
            node = node.GetChild(name);
            if (node == null)
            {
                return null;
            }
        }
        return node;
    }

    public void FindAmbiguities(AmbiguousConsumer consumer)
    {
        root.FindAmbiguities(consumer);
    }

    private void AddPaths(CommandNode node, List<List<CommandNode>> result, List<CommandNode> parents)
    {
        var current = new List<CommandNode>(parents) { node };
        result.Add(current);

        foreach (CommandNode child in node.Children)
        {
            AddPaths(child, result, current);
        }
    }

    private bool HasCommand(CommandNode node)
    {
        return node != null && (node.Executor != null || node.Children.Any(HasCommand));
    }
}
