using System.Net;
using System.Text.Json;
using Void.Proxy.Utils;
using Xunit;

namespace Void.Tests.UtilsTests;

public class JsonIpAddressConverterTests
{
    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonIpAddressConverter());
        return options;
    }

    [Fact]
    public void SerializeDeserialize_Ipv4_RoundTrips()
    {
        var address = IPAddress.Parse("127.0.0.1");
        var options = CreateOptions();
        var json = JsonSerializer.Serialize(address, options);
        Assert.Equal("\"127.0.0.1\"", json);
        var deserialized = JsonSerializer.Deserialize<IPAddress>(json, options);
        Assert.Equal(address, deserialized);
    }

    [Fact]
    public void SerializeDeserialize_Ipv6_RoundTrips()
    {
        var address = IPAddress.Parse("::1");
        var options = CreateOptions();
        var json = JsonSerializer.Serialize(address, options);
        Assert.Equal("\"::1\"", json);
        var deserialized = JsonSerializer.Deserialize<IPAddress>(json, options);
        Assert.Equal(address, deserialized);
    }

    [Fact]
    public void Serialize_LongIpv6_DoesNotTruncate()
    {
        var address = IPAddress.Parse("ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff");
        var options = CreateOptions();
        var json = JsonSerializer.Serialize(address, options);
        Assert.Equal("\"ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff\"", json);
        var deserialized = JsonSerializer.Deserialize<IPAddress>(json, options);
        Assert.Equal(address, deserialized);
    }
}
