using Void.Client;
using Xunit;

namespace Void.Client.UnitTests;

public sealed class GameImagingTests
{
    [Theory]
    [InlineData(250, 174, true)]
    [InlineData(250, 175, false)]
    [InlineData(2, 0, false)]
    public void DetectsChatInputFromBrightnessRatio(byte brightnessAboveInput, byte inputBrightness, bool expected)
    {
        byte[] pixelsAboveInput = [brightnessAboveInput, brightnessAboveInput, brightnessAboveInput];
        byte[] inputPixels = [inputBrightness, inputBrightness, inputBrightness];

        Assert.Equal(expected, ChatInputVisibilityDetector.IsVisible(pixelsAboveInput, inputPixels));
    }

    [Fact]
    public void CalculatesRec709Luminance()
    {
        byte[] pixels = [68, 89, 42];

        Assert.Equal(31.820392, ChatInputVisibilityDetector.CalculateAverageLuminance(pixels), 6);
    }

    [Fact]
    public void RejectsMismatchedChatRows()
    {
        byte[] pixelsAboveInput = [255, 255, 255];
        byte[] inputPixels = [0, 0, 0, 0, 0, 0];

        var exception = Assert.Throws<ArgumentException>(() => ChatInputVisibilityDetector.IsVisible(pixelsAboveInput, inputPixels));

        Assert.Contains("matching lengths", exception.Message);
    }
}
