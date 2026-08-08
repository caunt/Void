namespace Void.Minecraft.Components.Text.Events.Actions.Click;

/// <summary>
/// Represents a click event action that changes the currently displayed book page.
/// </summary>
/// <param name="Page">The target page number written to the click event payload.</param>
public record ChangePage(int Page) : IClickEventAction
{
    /// <summary>Gets the <c>change_page</c> click-action identifier.</summary>
    public string ActionName => "change_page";
}
