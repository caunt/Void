namespace Void.Minecraft.Components.Text.Properties.Content;

public record Score(string Name, string Objective) : IContent
{
    public string Type => "score";
}
