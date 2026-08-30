using Void.Client;
using Xunit;

namespace Void.Client.UnitTests;

public sealed class ConnectionTextMatcherTests
{
    [Fact]
    public void MatchesPixelFontRecognitionNoiseToKnownAction()
    {
        var matches = ConnectionTextMatcher.Match([CreateRecognizedText("D1rect Connectlon", 0.91)]);

        var match = Assert.Contains(ConnectionTextAction.DirectConnection, matches);
        Assert.Equal(ConnectionTextAction.DirectConnection, match.Action);
    }

    [Fact]
    public void RejectsLowConfidenceAndUnrelatedText()
    {
        var matches = ConnectionTextMatcher.Match([
            CreateRecognizedText("Multiplayer", 0.40),
            CreateRecognizedText("Minecraft Realms", 0.99)
        ]);

        Assert.Empty(matches);
    }

    [Theory]
    [InlineData("Back to Game", "backtogame")]
    [InlineData("  JOIN-SERVER! ", "joinserver")]
    [InlineData("Multíplayer", "multiplayer")]
    public void NormalizesButtonLabels(string text, string expected)
    {
        Assert.Equal(expected, ConnectionTextMatcher.Normalize(text));
    }

    [Theory]
    [InlineData("Multiplayer", false, "Multiplayer")]
    [InlineData("Proceed", false, "Proceed")]
    [InlineData("Direct Connection", false, "DirectConnection")]
    [InlineData("Back", false, "Back")]
    [InlineData("Back to Server List", false, "Back")]
    [InlineData("Return to Server List", false, "Back")]
    [InlineData("Back to Game", false, "BackToGame")]
    public void SelectsActionFromCurrentScreenText(string text, bool hasServerAddressField, string expected)
    {
        var matches = ConnectionTextMatcher.Match([CreateRecognizedText(text, 0.99)]);
        var selection = ConnectionNavigationSelector.Select(matches, hasServerAddressField);

        Assert.NotNull(selection);
        Assert.Equal(expected, selection.Kind.ToString());
    }

    [Fact]
    public void SelectsJoinOnlyForDirectConnectionForm()
    {
        var matches = ConnectionTextMatcher.Match([
            CreateRecognizedText("Server Address", 0.99),
            CreateRecognizedText("Join Server", 0.99)
        ]);

        Assert.Null(ConnectionNavigationSelector.Select(matches, hasServerAddressField: false));
        Assert.Equal(ConnectionNavigationKind.JoinServer, ConnectionNavigationSelector.Select(matches, hasServerAddressField: true)?.Kind);
    }

    [Fact]
    public void PrioritizesDirectConnectionOverServerListJoinButton()
    {
        var matches = ConnectionTextMatcher.Match([
            CreateRecognizedText("Join Server", 0.99),
            CreateRecognizedText("Direct Connection", 0.99)
        ]);

        Assert.Equal(ConnectionNavigationKind.DirectConnection, ConnectionNavigationSelector.Select(matches, hasServerAddressField: false)?.Kind);
    }

    [Theory]
    [InlineData(0.85, 0.90, true)]
    [InlineData(0.84, 1.00, false)]
    [InlineData(1.00, 0.89, false)]
    public void IdentifiesReliableFastMatches(double confidence, double similarity, bool expected)
    {
        var match = new ConnectionTextMatch(ConnectionTextAction.Multiplayer, "Multiplayer", confidence, similarity, new(10, 10, 100, 20));

        Assert.Equal(expected, ConnectionOcrFallbackPolicy.IsReliable(match));
    }

    [Fact]
    public void RejectsMissingFastMatch()
    {
        Assert.False(ConnectionOcrFallbackPolicy.IsReliable(null));
    }

    private static RecognizedText CreateRecognizedText(string text, double confidence)
    {
        return new(text, confidence, [[10, 10], [110, 10], [110, 30], [10, 30]]);
    }
}
