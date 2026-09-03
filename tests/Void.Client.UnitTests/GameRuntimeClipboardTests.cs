using Void.Client;
using Xunit;

namespace Void.Client.UnitTests;

public sealed class GameRuntimeClipboardTests
{
    [Fact]
    public void CreatesLowercaseServerAddressSelectionProbe()
    {
        Assert.Matches("^void[0-9a-f]{16}$", GameRuntime.CreateServerAddressSelectionProbe());
    }

    [Theory]
    [InlineData("existingvoid0123456789ABCDEF", "void0123456789ABCDEF", true)]
    [InlineData("existingvoid0123456789ABCDEFaddress", "void0123456789ABCDEF", true)]
    [InlineData("clipboard0123456789ABCDEF", "void0123456789ABCDEF", false)]
    [InlineData("existingvoid0123456789ABCDE", "void0123456789ABCDEF", false)]
    [InlineData("existing", "", false)]
    public void RecognizesCompleteServerAddressSelection(string clipboardText, string selectionProbe, bool expected)
    {
        Assert.Equal(expected, GameRuntime.ClipboardContainsSelectionProbe(clipboardText, selectionProbe));
    }

    [Fact]
    public void CreatesOrderedServerAddressReplacementCommand()
    {
        const string serverAddress = "host.docker.internal:25565";

        Assert.Equal(
            ["xdotool", "keyup", "ctrl", "key", "BackSpace", "type", "--clearmodifiers", "--", serverAddress],
            GameRuntime.CreateServerAddressReplacementCommand(serverAddress));
    }
}
