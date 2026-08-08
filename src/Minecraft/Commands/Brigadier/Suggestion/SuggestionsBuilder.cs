using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier.Suggestion;

/// <summary>Accumulates replacements for the suffix of command input beginning at a fixed cursor.</summary>
/// <param name="Input">The complete command input.</param>
/// <param name="Start">The inclusive replacement start.</param>
public class SuggestionsBuilder(string Input, int Start)
{
    private readonly List<Suggestion> _result = [];

    /// <summary>Gets the complete input converted to lowercase using the current culture.</summary>
    public string InputLowerCase { get; } = Input.ToLower();
    /// <summary>Gets the input suffix beginning at <c>Start</c>.</summary>
    public string Remaining { get; private set; } = Input[Start..];
    /// <summary>Gets the lowercased input suffix beginning at <c>Start</c>.</summary>
    public string RemainingLowerCase { get; private set; } = Input.ToLower()[Start..];

    /// <summary>Builds normalized suggestions from the accumulated entries.</summary>
    /// <returns>The completed suggestions.</returns>
    public Suggestions Build()
    {
        return Suggestions.Create(Input, _result);
    }

    /// <summary>Builds suggestions synchronously and wraps them in a completed asynchronous value.</summary>
    /// <param name="_">An unused cancellation token.</param>
    /// <returns>The completed suggestions.</returns>
    public ValueTask<Suggestions> BuildAsync(CancellationToken _)
    {
        return ValueTask.FromResult(Build());
    }

    /// <summary>Adds a text replacement unless it exactly equals the remaining input.</summary>
    /// <param name="text">The replacement text.</param>
    /// <returns>This builder.</returns>
    public SuggestionsBuilder Suggest(string text)
    {
        if (text == Remaining)
            return this;

        _result.Add(new Suggestion(StringRange.Between(Start, Input.Length), text));
        return this;
    }

    /// <summary>Adds a text replacement with a tooltip unless it exactly equals the remaining input.</summary>
    /// <param name="text">The replacement text.</param>
    /// <param name="tooltip">The tooltip.</param>
    /// <returns>This builder.</returns>
    public SuggestionsBuilder Suggest(string text, IMessage tooltip)
    {
        if (text == Remaining)
            return this;

        _result.Add(new Suggestion(StringRange.Between(Start, Input.Length), text, tooltip));
        return this;
    }

    /// <summary>Adds an integer replacement.</summary>
    /// <param name="value">The integer to format and suggest.</param>
    /// <returns>This builder.</returns>
    public SuggestionsBuilder Suggest(int value)
    {
        _result.Add(new IntegerSuggestion(StringRange.Between(Start, Input.Length), value));
        return this;
    }

    /// <summary>
    /// Adds an integer suggestion with an associated tooltip for the portion of the input from this builder's start position to the end of the input.
    /// </summary>
    /// <param name="value">The integer value to suggest.</param>
    /// <param name="tooltip">The message to associate with the suggestion as its tooltip.</param>
    /// <returns>This builder, allowing additional suggestions to be added.</returns>
    public SuggestionsBuilder Suggest(int value, IMessage tooltip)
    {
        _result.Add(new IntegerSuggestion(StringRange.Between(Start, Input.Length), value, tooltip));
        return this;
    }

    /// <summary>Appends every accumulated suggestion from another builder.</summary>
    /// <param name="other">The builder whose private results are copied.</param>
    /// <returns>This builder.</returns>
    public SuggestionsBuilder Add(SuggestionsBuilder other)
    {
        _result.AddRange(other._result);
        return this;
    }

    /// <summary>Creates an empty builder for the same input at another replacement start.</summary>
    /// <param name="start">The new inclusive start.</param>
    /// <returns>The new builder.</returns>
    public SuggestionsBuilder CreateOffset(int start)
    {
        return new SuggestionsBuilder(Input, start);
    }

    /// <summary>Creates an empty builder using this builder's original start.</summary>
    /// <returns>The new builder.</returns>
    public SuggestionsBuilder Restart()
    {
        return CreateOffset(Start);
    }
}
