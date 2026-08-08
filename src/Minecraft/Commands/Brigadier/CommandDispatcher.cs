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

/// <summary>Owns a Brigadier command tree and provides registration, parsing, execution, usage, and completion operations.</summary>
/// <param name="Root">The root command node.</param>
public record CommandDispatcher(RootCommandNode Root) : ICommandDispatcher
{
    /// <summary>Specifies the single space used to separate command arguments.</summary>
    public const char ArgumentSeparator = ' ';

    private const char UsageOptionalOpen = '[';
    private const char UsageOptionalClose = ']';
    private const char UsageRequiredOpen = '(';
    private const char UsageRequiredClose = ')';
    private const char UsageOr = '|';

    /// <summary>Gets or sets the callback notified after execution attempts.</summary>
    public ResultConsumer Consumer { get; set; } = (context, success, result) => { };

    /// <summary>Creates a dispatcher with a new empty root.</summary>
    public CommandDispatcher() : this(new RootCommandNode())
    {
        // Empty
    }

    /// <summary>Adds a proxy API command node to the root.</summary>
    /// <param name="node">The node to add.</param>
    /// <exception cref="ArgumentException"><paramref name="node"/> is not this Brigadier implementation's <see cref="CommandNode"/>.</exception>
    public void Add(ICommandNode node)
    {
        if (node is not CommandNode commandNode)
            throw new ArgumentException($"Node must be a {nameof(CommandNode)}.");

        Root.AddChild(commandNode);
    }

    /// <summary>Builds and registers a literal command at the root.</summary>
    /// <param name="command">The command builder.</param>
    /// <returns>The built node.</returns>
    public LiteralCommandNode Register(LiteralArgumentBuilder command)
    {
        var build = command.Build();
        Root.AddChild(build);
        return build;
    }

    /// <summary>Invokes a command-builder factory with the default argument context and registers its result.</summary>
    /// <param name="command">The builder factory.</param>
    /// <returns>The built node.</returns>
    public LiteralCommandNode Register(Func<IArgumentContext, LiteralArgumentBuilder> command)
    {
        var build = command(default(ArgumentContext)).Build();
        Root.AddChild(build);
        return build;
    }

    /// <summary>Enumerates descendants depth-first, excluding the supplied root itself.</summary>
    /// <param name="root">The traversal root, or <see langword="null"/> for <see cref="Root"/>.</param>
    /// <returns>The lazy descendant sequence.</returns>
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

    /// <summary>Parses a string and computes completions at its end.</summary>
    /// <param name="input">The command input.</param>
    /// <param name="source">The command source.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The completions.</returns>
    public async ValueTask<Suggestions> SuggestAsync(string input, ICommandSource source, CancellationToken cancellationToken)
    {
        return await SuggestAsync(new StringReader(input), source, cancellationToken);
    }

    /// <summary>Parses a reader and computes completions at the end of its source.</summary>
    /// <param name="input">The command reader.</param>
    /// <param name="source">The command source.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The completions.</returns>
    public async ValueTask<Suggestions> SuggestAsync(StringReader input, ICommandSource source, CancellationToken cancellationToken)
    {
        var parse = await ParseAsync(input, source, cancellationToken);
        return await GetCompletionSuggestions(parse, cancellationToken);
    }

    /// <summary>Parses and executes command text.</summary>
    /// <param name="input">The command input.</param>
    /// <param name="source">The command source.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The command result or accumulated fork result.</returns>
    public async ValueTask<int> ExecuteAsync(string input, ICommandSource source, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(new StringReader(input), source, cancellationToken);
    }

    /// <summary>Parses and executes input from a reader.</summary>
    /// <param name="input">The command reader.</param>
    /// <param name="source">The command source.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The command result or accumulated fork result.</returns>
    public async ValueTask<int> ExecuteAsync(StringReader input, ICommandSource source, CancellationToken cancellationToken)
    {
        var parse = await ParseAsync(input, source, cancellationToken);
        return await ExecuteAsync(parse, cancellationToken);
    }

    /// <summary>Validates and executes precomputed parse results.</summary>
    /// <param name="parse">The parse result.</param>
    /// <param name="cancellationToken">A cancellation token passed to execution.</param>
    /// <returns>The command result or accumulated fork result.</returns>
    /// <exception cref="CommandSyntaxException">Input remains unparsed, no executable context exists, or execution reports a syntax failure.</exception>
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

    /// <summary>Parses command text into the best matching command-tree path.</summary>
    /// <param name="command">The command input.</param>
    /// <param name="source">The command source used for access checks.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The best parse result, including recoverable branch exceptions.</returns>
    public async ValueTask<ParseResults> Parse(string command, ICommandSource source, CancellationToken cancellationToken)
    {
        return await ParseAsync(new StringReader(command), source, cancellationToken);
    }

    /// <summary>Parses a command reader into the best matching command-tree path.</summary>
    /// <param name="command">The command reader.</param>
    /// <param name="source">The command source used for access checks.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The best parse result.</returns>
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

    /// <summary>Enumerates every executable and redirect usage below a node.</summary>
    /// <param name="node">The traversal root.</param>
    /// <param name="source">The source used for access checks.</param>
    /// <param name="restricted">Whether to omit inaccessible branches.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The usage strings.</returns>
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

    /// <summary>Builds compact usage strings for each accessible direct child.</summary>
    /// <param name="node">The parent node.</param>
    /// <param name="source">The source used for access checks.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A map of child nodes to usage strings.</returns>
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
                var childrenUsable = await Task.WhenAll(node.Children.Select(async child => (child, await child.CanUseAsync(source, cancellationToken))));
                var children = childrenUsable.Where(pair => pair.Item2).Select(pair => pair.child);

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

    /// <summary>Computes completions at the end of parsed input.</summary>
    /// <param name="parse">The parse result.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Merged suggestions from relevant child nodes.</returns>
    public static async ValueTask<Suggestions> GetCompletionSuggestions(ParseResults parse, CancellationToken cancellationToken)
    {
        return await GetCompletionSuggestions(parse, parse.Reader.TotalLength, cancellationToken);
    }

    /// <summary>Computes completions at an explicit cursor.</summary>
    /// <param name="parse">The parse result.</param>
    /// <param name="cursor">The completion cursor.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Merged suggestions from relevant child nodes.</returns>
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

    /// <summary>Finds the first root-relative name path to a node by reference identity.</summary>
    /// <param name="target">The target node.</param>
    /// <returns>The name path, or an empty list when absent or when targeting the root.</returns>
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

    /// <summary>Traverses exact child names from the root.</summary>
    /// <param name="path">The root-relative name path.</param>
    /// <returns>The reached node; an empty path returns <see cref="Root"/>.</returns>
    /// <exception cref="KeyNotFoundException">A path segment is not a child of the current node.</exception>
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
