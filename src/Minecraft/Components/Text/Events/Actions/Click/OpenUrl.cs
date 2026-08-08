namespace Void.Minecraft.Components.Text.Events.Actions.Click;

/// <summary>Represents a click action that asks the client to open a URL.</summary>
/// <param name="Url">The URL supplied to the client.</param>
public record OpenUrl(string Url) : IClickEventAction
{
    /// <summary>Gets the <c>open_url</c> click-action identifier.</summary>
    public string ActionName => "open_url";
}
