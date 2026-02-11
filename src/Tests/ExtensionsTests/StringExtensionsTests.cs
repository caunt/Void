using Void.Proxy.Api.Extensions;
using Xunit;

namespace Void.Tests.ExtensionsTests;

public class StringExtensionsTests
{
    [Fact]
    public void SplitByDelimiters_WithDefaultDelimiters_SplitsByComma()
    {
        const string input = "a,b,c";
        var result = input.SplitByDelimiters();
        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void SplitByDelimiters_WithDefaultDelimiters_SplitsBySemicolon()
    {
        const string input = "a;b;c";
        var result = input.SplitByDelimiters();
        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void SplitByDelimiters_WithDefaultDelimiters_SplitsByNewline()
    {
        const string input = "a\nb\nc";
        var result = input.SplitByDelimiters();
        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void SplitByDelimiters_WithDefaultDelimiters_SplitsByMixedDelimiters()
    {
        const string input = "a,b;c\nd";
        var result = input.SplitByDelimiters();
        Assert.Equal(["a", "b", "c", "d"], result);
    }

    [Fact]
    public void SplitByDelimiters_WithDefaultDelimiters_RemovesEmptyEntries()
    {
        const string input = "a,,b;c";
        var result = input.SplitByDelimiters();
        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void SplitByDelimiters_WithDefaultDelimiters_TrimsEntries()
    {
        const string input = " a , b ; c ";
        var result = input.SplitByDelimiters();
        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void SplitByDelimiters_WithEmptyString_ReturnsEmptyArray()
    {
        const string input = "";
        var result = input.SplitByDelimiters();
        Assert.Empty(result);
    }

    [Fact]
    public void SplitByDelimiters_WithWhitespaceString_ReturnsEmptyArray()
    {
        const string input = "   ";
        var result = input.SplitByDelimiters();
        Assert.Empty(result);
    }

    [Fact]
    public void SplitByDelimiters_WithCustomDelimiters_SplitsCorrectly()
    {
        const string input = "a|b|c";
        var result = input.SplitByDelimiters(['|']);
        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void SplitByDelimiters_WithEscapeCharacter_HandlesEscapedDelimiters()
    {
        const string input = @"a\;b;c";
        var result = input.SplitByDelimiters([';'], escapeCharacter: '\\');
        Assert.Equal(["a;b", "c"], result);
    }

    [Fact]
    public void SplitByDelimiters_WithEscapeCharacter_HandlesMultipleEscapedDelimiters()
    {
        const string input = @"a\;b\;c;d";
        var result = input.SplitByDelimiters([';'], escapeCharacter: '\\');
        Assert.Equal(["a;b;c", "d"], result);
    }

    [Fact]
    public void SplitByDelimiters_WithEscapeCharacter_HandlesUnescapedDelimiters()
    {
        const string input = "a;b;c";
        var result = input.SplitByDelimiters([';'], escapeCharacter: '\\');
        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void SplitByDelimiters_WithEscapeCharacter_HandlesComplexCase()
    {
        const string input = @"https://user:pass@repo1.com\;https://user:pass@repo2.com;https://repo3.com";
        var result = input.SplitByDelimiters([';'], escapeCharacter: '\\');
        Assert.Equal(["https://user:pass@repo1.com;https://user:pass@repo2.com", "https://repo3.com"], result);
    }

    [Fact]
    public void SplitByDelimiters_RemoveEmptyEntriesFalse_KeepsEmptyEntries()
    {
        const string input = "a,,b;c";
        var result = input.SplitByDelimiters(removeEmptyEntries: false);
        Assert.Equal(["a", "", "b", "c"], result);
    }

    [Fact]
    public void SplitByDelimiters_VoidPluginsScenario_WorksCorrectly()
    {
        const string input = "plugin1.dll,plugin2.dll;plugin3.dll";
        var result = input.SplitByDelimiters();
        Assert.Equal(["plugin1.dll", "plugin2.dll", "plugin3.dll"], result);
    }

    [Fact]
    public void SplitByDelimiters_VoidNugetRepositoriesScenario_WorksCorrectly()
    {
        const string input = @"https://user1:pass1@repo1.com\;extra;https://repo2.com";
        var result = input.SplitByDelimiters([';'], escapeCharacter: '\\');
        Assert.Equal(["https://user1:pass1@repo1.com;extra", "https://repo2.com"], result);
    }
}
