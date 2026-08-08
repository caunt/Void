namespace Void.Minecraft.Components.Text.Properties.Content;

/// <summary>Represents text resolved from a scoreboard value.</summary>
/// <param name="Name">The score holder name or selector.</param>
/// <param name="Objective">The scoreboard objective name.</param>
public record ScoreContent(string Name, string Objective) : IContent
{
    /// <summary>Gets the <c>score</c> content discriminator.</summary>
    public string Type => "score";
}
