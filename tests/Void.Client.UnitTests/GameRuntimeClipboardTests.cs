using Void.Client;
using Xunit;

namespace Void.Client.UnitTests;

public sealed class GameRuntimeClipboardTests
{
    [Theory]
    [InlineData("host.docker.internal:25565", "host.docker.internal:25565", true)]
    [InlineData("host.docker.internal:2556", "host.docker.internal:25565", false)]
    [InlineData("existinghost.docker.internal:25565", "host.docker.internal:25565", false)]
    [InlineData("host.docker.internal:25565existing", "host.docker.internal:25565", false)]
    [InlineData("", "host.docker.internal:25565", false)]
    public void MatchesExactServerAddress(string clipboardText, string serverAddress, bool expected)
    {
        Assert.Equal(expected, GameRuntime.ClipboardMatchesServerAddress(clipboardText, serverAddress));
    }

    [Fact]
    public void CreatesOrderedServerAddressPasteCommand()
    {
        Assert.Equal(["xdotool", "key", "--clearmodifiers", "ctrl+a", "ctrl+v"], GameRuntime.CreateServerAddressPasteCommand());
    }

    [Fact]
    public void CreatesOrderedServerAddressClearCommand()
    {
        Assert.Equal(["xdotool", "key", "--clearmodifiers", "BackSpace"], GameRuntime.CreateServerAddressClearCommand());
    }

    [Fact]
    public void CreatesOrderedServerAddressCopyCommand()
    {
        Assert.Equal(["xdotool", "key", "--clearmodifiers", "ctrl+a", "ctrl+c"], GameRuntime.CreateServerAddressCopyCommand());
    }
}
