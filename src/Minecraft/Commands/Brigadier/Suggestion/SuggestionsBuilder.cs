using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier.Suggestion;

public class SuggestionsBuilder(string Input, int Start)
{
    private readonly List<Suggestion> _result = [];

    public string InputLowerCase { get; } = Input.ToLower();
    public string Remaining { get; private set; } = Input[Start..];
    public string RemainingLowerCase { get; private set; } = Input.ToLower()[Start..];

    public Suggestions Build()
    {
        return Suggestions.Create(Input, _result);
    }

    public ValueTask<Suggestions> BuildAsync(CancellationToken _)
    {
        return ValueTask.FromResult(Build());
    }

    public SuggestionsBuilder Suggest(string text)
    {
        if (text == Remaining)
            return this;

        _result.Add(new Suggestion(StringRange.Between(Start, Input.Length), text));
        return this;
    }

    public SuggestionsBuilder Suggest(string text, IMessage tooltip)
    {
        if (text == Remaining)
            return this;

        _result.Add(new Suggestion(StringRange.Between(Start, Input.Length), text, tooltip));
        return this;
    }

    public SuggestionsBuilder Suggest(int value)
    {
        _result.Add(new IntegerSuggestion(StringRange.Between(Start, Input.Length), value));
        return this;
    }

    public SuggestionsBuilder Suggest(int value, IMessage tooltip)
    {
        _result.Add(new IntegerSuggestion(StringRange.Between(Start, Input.Length), value, tooltip));
        return this;
    }

    public SuggestionsBuilder Add(SuggestionsBuilder other)
    {
        _result.AddRange(other._result);
        return this;
    }

    public SuggestionsBuilder CreateOffset(int start)
    {
        return new SuggestionsBuilder(Input, start);
    }

    public SuggestionsBuilder Restart()
    {
        return CreateOffset(Start);
    }
}
