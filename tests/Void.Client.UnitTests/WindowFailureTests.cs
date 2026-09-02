using Void.Client;
using Xunit;

namespace Void.Client.UnitTests;

public sealed class WindowFailureTests
{
    [Fact]
    public void IdentifiesExplicitBadWindowFailure()
    {
        var exception = new ExternalProcessException(
            "xdotool",
            ["windowfocus", "0xe00007"],
            1,
            "",
            "X Error of failed request: BadWindow (invalid Window parameter)");

        Assert.True(X11FailureClassifier.IsExplicitStaleWindow(exception));
    }

    [Theory]
    [InlineData("xdotool", "BadMatch (invalid parameter attributes)")]
    [InlineData("xdotool", "permission denied")]
    [InlineData("import", "BadWindow (invalid Window parameter)")]
    public void RejectsUnrelatedExternalFailures(string fileName, string standardError)
    {
        var exception = new ExternalProcessException(fileName, ["argument"], 1, "output", standardError);

        Assert.False(X11FailureClassifier.IsExplicitStaleWindow(exception));
        Assert.Equal(fileName, exception.FileName);
        Assert.Equal(1, exception.ExitCode);
        Assert.Equal("output", exception.StandardOutput);
        Assert.Equal(standardError, exception.StandardError);
    }
}
