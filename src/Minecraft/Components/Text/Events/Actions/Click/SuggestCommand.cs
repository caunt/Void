namespace Void.Minecraft.Components.Text.Events.Actions.Click;

public record SuggestCommand(string Command) : IClickEventAction
{
    public string ActionName => "suggest_command";
}
