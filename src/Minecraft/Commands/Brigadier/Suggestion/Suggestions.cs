using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier.Suggestion;

public record Suggestions(StringRange Range, List<Suggestion> All)
{
    public static Suggestions Empty { get; } = new Suggestions(StringRange.At(0), []);

    public bool IsEmpty => All.Count == 0;

    public static ValueTask<Suggestions> EmptyAsync()
    {
        return ValueTask.FromResult(Empty);
    }

    public static Suggestions Merge(string command, IEnumerable<Suggestions> input)
    {
        if (!input.Any())
            return Empty;
        else if (input.Count() == 1)
            return input.ElementAt(0);

        var texts = new HashSet<Suggestion>();

        foreach (var suggestions in input)
            foreach (var suggestion in suggestions.All)
                texts.Add(suggestion);

        return Create(command, texts);
    }

    public static Suggestions Create(string command, IEnumerable<Suggestion> suggestions)
    {
        if (!suggestions.Any())
            return Empty;

        var start = int.MaxValue;
        var end = int.MinValue;

        foreach (var suggestion in suggestions)
        {
            start = Math.Min(suggestion.Range.Start, start);
            end = Math.Max(suggestion.Range.End, end);
        }

        var range = new StringRange(start, end);
        var texts = new HashSet<Suggestion>();

        foreach (var suggestion in suggestions)
            texts.Add(suggestion.Expand(command, range));

        var sorted = new List<Suggestion>(texts);
        sorted.Sort();

        return new Suggestions(range, sorted);
    }
}
