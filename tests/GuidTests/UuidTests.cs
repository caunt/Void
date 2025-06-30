using System;
using Void.Minecraft.Profiles;
using Void.Proxy.Utils;
using Xunit;

namespace Void.Tests;

public class UuidTests
{
    [Fact]
    public void NewUuid_GeneratesUniqueNonEmpty()
    {
        var uuid1 = Uuid.NewUuid();
        var uuid2 = Uuid.NewUuid();
        Assert.NotEqual(uuid1, uuid2);
        Assert.NotEqual(Uuid.Empty, uuid1);
        Assert.NotEqual(Uuid.Empty, uuid2);
    }

    [Fact]
    public void Parse_IntArray_ParsesCorrectly()
    {
        int[] parts =
        {
            0x11223344,
            0x55667788,
            unchecked((int)0x99aabbcc),
            unchecked((int)0xddeeff00)
        };
        var uuid = Uuid.Parse(parts);
        byte[] expected =
        {
            0x44, 0x33, 0x22, 0x11,
            0x66, 0x55, 0x88, 0x77,
            0x99, 0xaa, 0xbb, 0xcc,
            0xdd, 0xee, 0xff, 0x00
        };
        Assert.Equal(new Guid(expected), uuid.AsGuid);
    }

    [Theory]
    [InlineData(0x00, 0)]
    [InlineData(0x80, 1)]
    [InlineData(0xC0, 2)]
    [InlineData(0xF0, 3)]
    public void GetVariant_ReturnsExpected(byte variant, int expected)
    {
        var bytes = new byte[16];
        bytes[8] = variant;
        var uuid = new Uuid(new Guid(bytes));
        Assert.Equal(expected, uuid.GetVariant());
        Assert.Equal(expected, GuidHelper.GetVariant(uuid.AsGuid));
    }

    [Fact]
    public void Parse_String_ParsesCorrectly()
    {
        const string text = "11223344-5566-7788-99aa-bbccddeeff00";
        var uuid = Uuid.Parse(text);
        Assert.Equal(Guid.Parse(text), uuid.AsGuid);
    }

    [Fact]
    public void FromStringHash_ProducesVersion3Guid()
    {
        const string input = "hash-me";
        var uuid = Uuid.FromStringHash(input);
        var guid = GuidHelper.FromStringHash(input);

        Assert.Equal(guid, uuid.AsGuid);
        Assert.Equal(3, uuid.GetVersion());
        Assert.Equal(1, uuid.GetVariant());
        Assert.Equal(3, GuidHelper.GetVersion(guid));
        Assert.Equal(1, GuidHelper.GetVariant(guid));
    }

    [Fact]
    public void FromLongs_ConstructsGuid()
    {
        const long most = 0x1122334455667788L;
        const long least = unchecked((long)0x99AABBCCDDEEFF00UL);
        var uuid = Uuid.FromLongs(most, least);
        var guid = GuidHelper.FromLongs(most, least);

        Assert.Equal(guid, uuid.AsGuid);
    }

    [Fact]
    public void Offline_YieldsSameAsFromStringHash()
    {
        const string name = "Steve";
        var offline = Uuid.Offline(name);
        var expected = Uuid.FromStringHash($"OfflinePlayer:{name}");
        Assert.Equal(expected, offline);
    }

    [Fact]
    public void EqualityAndComparison_WorkAsExpected()
    {
        var g = Guid.NewGuid();
        var u1 = new Uuid(g);
        var u2 = new Uuid(g);

        Assert.Equal(u1, u2);
        Assert.True(u1 == u2);
        Assert.False(u1 != u2);
        Assert.Equal(0, u1.CompareTo(u2));
    }
}
