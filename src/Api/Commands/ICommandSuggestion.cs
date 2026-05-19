namespace Void.Proxy.Api.Commands;

public interface ICommandSuggestion
{
    public int Start { get; }
    public string Text { get; }
    public string? Tooltip { get; }
}
