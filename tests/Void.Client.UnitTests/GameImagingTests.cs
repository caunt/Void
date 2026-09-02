using System.Text;
using Void.Client;
using Xunit;

namespace Void.Client.UnitTests;

public sealed class GameImagingTests
{
    [Theory]
    [InlineData(250, 175, true)]
    [InlineData(250, 176, false)]
    [InlineData(2, 0, false)]
    public void DetectsChatInputFromBrightnessRatio(byte brightnessAboveInput, byte inputBrightness, bool expected)
    {
        using var screen = GameRuntime.ScreenImage.LoadPortablePixmap(CreatePortablePixmap(
            2,
            2,
            (brightnessAboveInput, brightnessAboveInput, brightnessAboveInput),
            (brightnessAboveInput, brightnessAboveInput, brightnessAboveInput),
            (inputBrightness, inputBrightness, inputBrightness),
            (inputBrightness, inputBrightness, inputBrightness)));

        Assert.Equal(expected, screen.IsChatInputVisible());
    }

    [Fact]
    public void CalculatesRec709Luminance()
    {
        using var screen = GameRuntime.ScreenImage.LoadPortablePixmap(CreatePortablePixmap(1, 1, (68, 89, 42)));

        Assert.Equal(31.820392, screen.CalculateAverageLuminance(0), 6);
    }

    [Fact]
    public void RejectsUnexpectedChatCaptureHeight()
    {
        using var screen = GameRuntime.ScreenImage.LoadPortablePixmap(CreatePortablePixmap(1, 1, (255, 255, 255)));

        var exception = Assert.Throws<InvalidOperationException>(screen.IsChatInputVisible);

        Assert.Contains("exactly two pixel rows", exception.Message);
    }

    private static byte[] CreatePortablePixmap(int width, int height, params (byte Red, byte Green, byte Blue)[] pixels)
    {
        Assert.Equal(width * height, pixels.Length);
        var header = Encoding.ASCII.GetBytes($"P6\n{width} {height}\n255\n");
        var image = new byte[header.Length + pixels.Length * 3];
        header.CopyTo(image, 0);

        for (var index = 0; index < pixels.Length; index++)
        {
            var offset = header.Length + index * 3;
            image[offset] = pixels[index].Red;
            image[offset + 1] = pixels[index].Green;
            image[offset + 2] = pixels[index].Blue;
        }

        return image;
    }
}
