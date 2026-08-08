using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier.Suggestion;

/// <summary>Contains normalized, sorted suggestions sharing a replacement range.</summary>
/// <param name="Range">The common replacement range.</param>
/// <param name="All">The suggestion list.</param>
public record Suggestions(StringRange Range, List<Suggestion> All)
{
    /// <summary>Gets the shared empty suggestion result at position zero.</summary>
    public static Suggestions Empty { get; } = new Suggestions(StringRange.At(0), []);

    /// <summary>Gets whether the list contains no suggestions.</summary>
    public bool IsEmpty => All.Count == 0;

    /// <summary>Returns the shared empty result as a completed asynchronous value.</summary>
    /// <returns>A completed value containing <see cref="Empty"/>.</returns>
    public static ValueTask<Suggestions> EmptyAsync()
    {
        return ValueTask.FromResult(Empty);
    }

    /// <summary>Merges suggestion sets and normalizes their replacement ranges.</summary>
    /// <param name="command">The complete command input.</param>
    /// <param name="input">The suggestion sets.</param>
    /// <returns>The empty result, the sole input instance, or a merged result.</returns>
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

    /// <summary>Deduplicates, range-normalizes, and ordinally sorts suggestions.</summary>
    /// <param name="command">The complete command input.</param>
    /// <param name="suggestions">The suggestions to normalize.</param>
    /// <returns>The normalized result, or <see cref="Empty"/> for no suggestions.</returns>
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
