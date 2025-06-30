using System.Net;
using System.Text.Json;
using Void.Proxy.Utils;
using Xunit;

namespace Void.Tests;

public class JsonIpEndPointConverterTests
{
    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonIpEndPointConverter());
        return options;
    }

    [Fact]
    public void SerializeDeserialize_Ipv4_RoundTrips()
    {
        var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25565);
        var options = CreateOptions();
        var json = JsonSerializer.Serialize(ep, options);
        Assert.Equal("\"127.0.0.1:25565\"", json);
        var deserialized = JsonSerializer.Deserialize<IPEndPoint>(json, options);
        Assert.Equal(ep, deserialized);
    }

    [Fact]
    public void SerializeDeserialize_Ipv6_RoundTrips()
    {
        var ep = new IPEndPoint(IPAddress.Parse("::1"), 443);
        var options = CreateOptions();
        var json = JsonSerializer.Serialize(ep, options);
        Assert.Equal("\"[::1]:443\"", json);
        var deserialized = JsonSerializer.Deserialize<IPEndPoint>(json, options);
        Assert.Equal(ep, deserialized);
    }

    [Fact]
    public void Serialize_LongIpv6_DoesNotTruncate()
    {
        var address = "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff";
        var ep = new IPEndPoint(IPAddress.Parse(address), 65535);
        var options = CreateOptions();
        var json = JsonSerializer.Serialize(ep, options);
        Assert.Equal($"\"[{address}]:65535\"", json);
        var deserialized = JsonSerializer.Deserialize<IPEndPoint>(json, options);
        Assert.Equal(ep, deserialized);
    }
}
