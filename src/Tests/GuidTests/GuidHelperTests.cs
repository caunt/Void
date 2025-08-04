using Void.Proxy.Utils;
using Xunit;

namespace Void.Tests.GuidTests;

public class GuidHelperTests
{
    [Fact]
    public void FromStringHash_GeneratesConsistentGuid()
    {
        const string input = "test";
        var guid = GuidHelper.FromStringHash(input);
        Assert.Equal(guid, GuidHelper.FromStringHash(input));
        Assert.Equal(3, GuidHelper.GetVersion(guid));
        Assert.Equal(1, GuidHelper.GetVariant(guid));
    }

    [Fact]
    public void FromLongs_ConstructsExpectedGuid()
    {
        const long mostSig = 0x1122334455667788L;
        const long leastSig = unchecked((long)0x99AABBCCDDEEFF00UL);
        var guid = GuidHelper.FromLongs(mostSig, leastSig);
        Assert.Equal(guid, GuidHelper.FromLongs(mostSig, leastSig));
    }
}

