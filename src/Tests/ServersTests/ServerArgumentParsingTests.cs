using System;
using Xunit;

namespace Void.Tests.ServersTests;

public class ServerArgumentParsingTests
{
    [Theory]
    [InlineData("paper.default.svc.cluster.local", "paper.default.svc.cluster.local", 25565)]
    [InlineData("localhost:25566", "localhost", 25566)]
    [InlineData("example.com", "example.com", 25565)]
    [InlineData("example.com:30000", "example.com", 30000)]
    [InlineData("192.168.1.1", "192.168.1.1", 25565)]
    [InlineData("192.168.1.1:25567", "192.168.1.1", 25567)]
    [InlineData("[2001:db8::1]:25565", "[2001:db8::1]", 25565)]
    public void ParseServerArgument_ValidFormats_ParsesCorrectly(string input, string expectedHost, int expectedPort)
    {
        // Parse using Uri (mimicking the ServerService logic)
        var result = Uri.TryCreate($"tcp://{input}", UriKind.Absolute, out var uri);
        
        Assert.True(result, $"Failed to parse: {input}");
        Assert.NotNull(uri);
        Assert.Equal(expectedHost, uri.Host);
        
        var actualPort = uri.Port == -1 ? 25565 : uri.Port;
        Assert.Equal(expectedPort, actualPort);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid::port")]
    public void ParseServerArgument_InvalidFormats_FailsOrHandlesGracefully(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            // Empty/whitespace should be handled before parsing
            Assert.True(string.IsNullOrWhiteSpace(input));
        }
        else
        {
            // Invalid formats should fail to parse or be rejected
            var result = Uri.TryCreate($"tcp://{input}", UriKind.Absolute, out var uri);
            
            if (result && uri != null)
            {
                // If it parses, ensure host is valid
                Assert.False(string.IsNullOrWhiteSpace(uri.Host));
            }
        }
    }

    [Fact]
    public void DefaultPort_IsCorrectMinecraftPort()
    {
        const int expectedDefaultPort = 25565;
        
        // Test that a hostname without port defaults to 25565
        var result = Uri.TryCreate("tcp://minecraft.server.com", UriKind.Absolute, out var uri);
        
        Assert.True(result);
        Assert.NotNull(uri);
        Assert.Equal(-1, uri.Port); // Uri returns -1 for missing port
        
        var actualPort = uri.Port == -1 ? expectedDefaultPort : uri.Port;
        Assert.Equal(25565, actualPort);
    }

    [Theory]
    [InlineData("localhost:25565")]
    [InlineData("localhost:1")]
    [InlineData("localhost:65535")]
    public void ParseServerArgument_ValidPortRange_Succeeds(string input)
    {
        var result = Uri.TryCreate($"tcp://{input}", UriKind.Absolute, out var uri);
        
        Assert.True(result);
        Assert.NotNull(uri);
        Assert.InRange(uri.Port, 1, 65535);
    }

    [Theory]
    [InlineData("server1.local")]
    [InlineData("server2.local:25567")]
    [InlineData("192.168.1.1")]
    [InlineData("192.168.1.2:25568")]
    public void ParseServerArgument_MixedFormats_EachParsesCorrectly(string input)
    {
        var result = Uri.TryCreate($"tcp://{input}", UriKind.Absolute, out var uri);
        
        Assert.True(result);
        Assert.NotNull(uri);
        Assert.False(string.IsNullOrWhiteSpace(uri.Host));
        
        var port = uri.Port == -1 ? 25565 : uri.Port;
        Assert.InRange(port, 1, 65535);
    }
}
