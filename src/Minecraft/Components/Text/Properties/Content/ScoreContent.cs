namespace Void.Minecraft.Components.Text.Properties.Content;

public record ScoreContent(string Name, string Objective) : IContent
{
    public string Type => "score";
}
