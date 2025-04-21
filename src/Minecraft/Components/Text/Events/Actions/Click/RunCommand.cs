namespace Void.Minecraft.Components.Text.Events.Actions.Click;

public record RunCommand(string Command) : IClickEventAction
{
    public string ActionName => "run_command";
}
