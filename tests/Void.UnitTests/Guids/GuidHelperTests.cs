using Void.Proxy.Utils;
using Xunit;

namespace Void.UnitTests.Guids;

public class GuidHelperTests
{
    [Fact]
    public void FromStringHash_MatchesJavaNameUuidFromBytes()
    {
        var guid = GuidHelper.FromStringHash("hash-me");

        Assert.Equal("0b893466-231e-315a-b152-0cfb1f761f4f", guid.ToString());
        Assert.Equal(3, GuidHelper.GetVersion(guid));
        Assert.Equal(2, GuidHelper.GetVariant(guid));
    }

    [Fact]
    public void FromLongs_ConstructsExpectedGuid()
    {
        const long mostSig = 0x1122334455667788L;
        const long leastSig = unchecked((long)0x99AABBCCDDEEFF00UL);
        var guid = GuidHelper.FromLongs(mostSig, leastSig);

        Assert.Equal("11223344-5566-7788-99aa-bbccddeeff00", guid.ToString());
    }
}
