using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text.Json;
using Void.Minecraft.Profiles;
using Xunit;

namespace Void.UnitTests.Guids;

public class UuidTests
{
    [Fact]
    public void NewUuid_ReturnsVersionFourRfcUuid()
    {
        var uuid = Uuid.NewUuid();

        Assert.NotEqual(Uuid.Empty, uuid);
        Assert.Equal(4, uuid.Version);
        Assert.Equal(2, uuid.Variant);
    }

    [Theory]
    [InlineData("hash-me", "0b893466-231e-315a-b152-0cfb1f761f4f")]
    [InlineData("Minecraft世界", "652fde1d-cabd-3d90-a797-0ea9fd2d7e96")]
    public void FromStringHash_MatchesJavaNameUuidFromBytes(string text, string expected)
    {
        var uuid = Uuid.FromStringHash(text);

        Assert.Equal(expected, uuid.ToString());
        Assert.Equal(3, uuid.Version);
        Assert.Equal(2, uuid.Variant);
    }

    [Theory]
    [InlineData("Steve", "5627dd98-e6be-3c21-b8a8-e92344183641")]
    [InlineData("steve", "53909932-f794-33c0-9329-948045a4c1ce")]
    [InlineData("Alex", "36532b5e-c442-3dbb-a24c-c7e55d0f979a")]
    public void Offline_MatchesJavaOfflinePlayerUuid(string name, string expected)
    {
        Assert.Equal(expected, Uuid.Offline(name).ToString());
    }

    [Fact]
    public void FromStringHash_HandlesLargeInputWithoutUsingUnboundedStackSpace()
    {
        var text = new string('界', 1_000_000);

        var first = Uuid.FromStringHash(text);
        var second = Uuid.FromStringHash(text);

        Assert.Equal(first, second);
        Assert.Equal(3, first.Version);
        Assert.Equal(2, first.Variant);
    }

    [Fact]
    public void ParseIntArray_UsesCanonicalBigEndianOrder()
    {
        var uuid = Uuid.Parse(
            0x11223344,
            0x55667788,
            unchecked((int)0x99aabbcc),
            unchecked((int)0xddeeff00));

        Assert.Equal("11223344-5566-7788-99aa-bbccddeeff00", uuid.ToString());
    }

    [Fact]
    public void Version_ReturnsEveryCanonicalVersionNibble()
    {
        Span<byte> bytes = stackalloc byte[16];

        for (var expected = 0; expected <= 15; expected++)
        {
            bytes.Clear();
            bytes[6] = (byte)(expected << 4);
            var uuid = new Uuid(new Guid(bytes, bigEndian: true));

            Assert.Equal(expected, uuid.Version);
        }
    }

    [Theory]
    [InlineData(0x00, 0)]
    [InlineData(0x7f, 0)]
    [InlineData(0x80, 2)]
    [InlineData(0xbf, 2)]
    [InlineData(0xc0, 6)]
    [InlineData(0xdf, 6)]
    [InlineData(0xe0, 7)]
    [InlineData(0xff, 7)]
    public void Variant_MatchesJavaVariantBoundaries(byte variantByte, int expected)
    {
        Span<byte> bytes = stackalloc byte[16];
        bytes[8] = variantByte;
        var uuid = new Uuid(new Guid(bytes, bigEndian: true));

        Assert.Equal(expected, uuid.Variant);
    }

    [Fact]
    public void FromLongs_MatchesJavaUuidConstructor()
    {
        const long mostSignificantBits = 0x1122334455667788L;
        const long leastSignificantBits = unchecked((long)0x99aabbccddeeff00UL);

        var uuid = Uuid.FromLongs(mostSignificantBits, leastSignificantBits);

        Assert.Equal("11223344-5566-7788-99aa-bbccddeeff00", uuid.ToString());
    }

    [Fact]
    public void CanonicalRepresentations_AgreeForDeterministicValues()
    {
        var random = new Random(0x51d);
        var bytes = new byte[16];

        for (var iteration = 0; iteration < 128; iteration++)
        {
            random.NextBytes(bytes);
            var guid = new Guid(bytes, bigEndian: true);
            var expected = new Uuid(guid);

            var mostSignificantBits = BinaryPrimitives.ReadInt64BigEndian(bytes.AsSpan(..8));
            var leastSignificantBits = BinaryPrimitives.ReadInt64BigEndian(bytes.AsSpan(8..));
            var parts = new[]
            {
                BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(..4)),
                BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(4..8)),
                BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(8..12)),
                BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(12..))
            };

            Assert.Equal(expected, Uuid.Parse(expected.ToString()));
            Assert.Equal(expected, Uuid.FromLongs(mostSignificantBits, leastSignificantBits));
            Assert.Equal(expected, Uuid.Parse(parts));
            Assert.Equal(guid, (Guid)expected);
            Assert.Equal(expected, (Uuid)guid);
        }
    }

    [Theory]
    [InlineData("11223344-5566-7788-99aa-bbccddeeff00", true)]
    [InlineData("112233445566778899aabbccddeeff00", true)]
    [InlineData("not-a-uuid", false)]
    [InlineData(null, false)]
    public void TryParse_HandlesSupportedAndInvalidRepresentations(string? text, bool expected)
    {
        var result = Uuid.TryParse(text, out var uuid);

        Assert.Equal(expected, result);
        Assert.Equal(expected ? "11223344-5566-7788-99aa-bbccddeeff00" : Uuid.Empty.ToString(), uuid.ToString());
    }

    [Fact]
    public void ParseIntArray_RejectsIncorrectPartCounts()
    {
        Assert.Throws<ArgumentException>(() => Uuid.Parse(1, 2, 3));
        Assert.Throws<ArgumentException>(() => Uuid.Parse(1, 2, 3, 4, 5));
    }

    [Fact]
    public void EqualityHashingAndComparison_DelegateToGuidValue()
    {
        var guid = Guid.Parse("11223344-5566-7788-99aa-bbccddeeff00");
        var first = new Uuid(guid);
        var second = new Uuid(guid);
        var different = Uuid.Empty;

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.True(first == second);
        Assert.True(first != different);
        Assert.Equal(0, first.CompareTo(second));
        Assert.Equal(guid.CompareTo(Guid.Empty), first.CompareTo(different));
    }

    [Fact]
    public void JsonSerialization_RoundTripsValuesAndPropertyNames()
    {
        var uuid = Uuid.Parse("11223344-5566-7788-99aa-bbccddeeff00");
        var values = new Dictionary<Uuid, string> { [uuid] = "value" };

        var uuidJson = JsonSerializer.Serialize(uuid);
        var valuesJson = JsonSerializer.Serialize(values);

        Assert.Equal("\"11223344-5566-7788-99aa-bbccddeeff00\"", uuidJson);
        Assert.Equal(uuid, JsonSerializer.Deserialize<Uuid>(uuidJson));
        Assert.Equal("{\"11223344-5566-7788-99aa-bbccddeeff00\":\"value\"}", valuesJson);
        Assert.Equal("value", JsonSerializer.Deserialize<Dictionary<Uuid, string>>(valuesJson)?[uuid]);
    }
}
