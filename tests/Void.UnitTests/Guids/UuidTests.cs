using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Void.Minecraft.Profiles;
using Xunit;

namespace Void.UnitTests.Guids;

public class UuidTests
{
    private const string CanonicalText = "11223344-5566-7788-99aa-bbccddeeff00";

    private static readonly byte[] CanonicalBytes =
    [
        0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,
        0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff, 0x00
    ];

    [Fact]
    public void EmptyAndAllBitsSet_ReturnExpectedValues()
    {
        Assert.Equal("00000000-0000-0000-0000-000000000000", Uuid.Empty.ToString());
        Assert.Equal("ffffffff-ffff-ffff-ffff-ffffffffffff", Uuid.AllBitsSet.ToString());
    }

    [Fact]
    public void NewUuid_ReturnsVersionFourRfcUuid()
    {
        var uuid = Uuid.NewUuid();

        Assert.NotEqual(Uuid.Empty, uuid);
        Assert.Equal(4, uuid.Version);
        Assert.Equal(2, uuid.Variant);
    }

    [Fact]
    public void CreateVersion7_ReturnsVersionSevenRfcUuid()
    {
        var current = Uuid.CreateVersion7();
        var timestamped = Uuid.CreateVersion7(DateTimeOffset.UnixEpoch);

        Assert.Equal(7, current.Version);
        Assert.Equal(2, current.Variant);
        Assert.Equal(7, timestamped.Version);
        Assert.Equal(2, timestamped.Variant);
        Assert.Equal("00000000-0000", timestamped.ToString()[..13]);
    }

    [Theory]
    [InlineData("hash-me", "0b893466-231e-315a-b152-0cfb1f761f4f")]
    [InlineData("Minecraft世界", "652fde1d-cabd-3d90-a797-0ea9fd2d7e96")]
    public void CreateVersion3_MatchesJavaNameUuidFromBytes(string name, string expected)
    {
        var fromText = Uuid.CreateVersion3(name);
        var fromBytes = Uuid.CreateVersion3(Encoding.UTF8.GetBytes(name));

        Assert.Equal(expected, fromText.ToString());
        Assert.Equal(fromText, fromBytes);
        Assert.Equal(3, fromText.Version);
        Assert.Equal(2, fromText.Variant);
    }

    [Theory]
    [InlineData("Steve", "5627dd98-e6be-3c21-b8a8-e92344183641")]
    [InlineData("steve", "53909932-f794-33c0-9329-948045a4c1ce")]
    [InlineData("Alex", "36532b5e-c442-3dbb-a24c-c7e55d0f979a")]
    public void CreateOfflinePlayer_MatchesJavaOfflinePlayerUuid(string username, string expected)
    {
        Assert.Equal(expected, Uuid.CreateOfflinePlayer(username).ToString());
    }

    [Fact]
    public void CreateVersion3_HandlesLargeInputWithoutUsingUnboundedStackSpace()
    {
        var name = new string('界', 1_000_000);

        var first = Uuid.CreateVersion3(name);
        var second = Uuid.CreateVersion3(name);

        Assert.Equal(first, second);
        Assert.Equal(3, first.Version);
        Assert.Equal(2, first.Variant);
    }

    [Fact]
    public void ConstructorsAndByteMethods_DefaultToCanonicalOrder()
    {
        var fromArray = new Uuid(CanonicalBytes);
        var fromSpan = new Uuid(CanonicalBytes.AsSpan());
        var fromString = new Uuid(CanonicalText);
        var fromGuid = new Uuid(Guid.Parse(CanonicalText));
        Span<byte> destination = stackalloc byte[16];

        Assert.Equal(CanonicalText, fromArray.ToString());
        Assert.Equal(fromArray, fromSpan);
        Assert.Equal(fromArray, fromString);
        Assert.Equal(fromArray, fromGuid);
        Assert.Equal(CanonicalBytes, fromArray.ToByteArray());
        Assert.True(fromArray.TryWriteBytes(destination));
        Assert.True(destination.SequenceEqual(CanonicalBytes));
    }

    [Fact]
    public void ByteMethods_SupportExplicitGuidByteOrder()
    {
        var uuid = Uuid.Parse(CanonicalText);
        Guid guid = uuid;
        var guidBytes = guid.ToByteArray();
        Span<byte> destination = stackalloc byte[16];

        Assert.Equal(guidBytes, uuid.ToByteArray(bigEndian: false));
        Assert.Equal(uuid, new Uuid(guidBytes, bigEndian: false));
        Assert.True(uuid.TryWriteBytes(destination, bigEndian: false, out var bytesWritten));
        Assert.Equal(16, bytesWritten);
        Assert.True(destination.SequenceEqual(guidBytes));
    }

    [Fact]
    public void TryWriteBytes_ReturnsFalseForSmallDestination()
    {
        Span<byte> destination = stackalloc byte[15];

        Assert.False(Uuid.Empty.TryWriteBytes(destination));
        Assert.False(Uuid.Empty.TryWriteBytes(destination, bigEndian: true, out var bytesWritten));
        Assert.Equal(0, bytesWritten);
    }

    [Fact]
    public void FromIntArray_UsesCanonicalBigEndianOrder()
    {
        int[] parts =
        [
            0x11223344,
            0x55667788,
            unchecked((int)0x99aabbcc),
            unchecked((int)0xddeeff00)
        ];

        Assert.Equal(CanonicalText, Uuid.FromIntArray(parts).ToString());
    }

    [Fact]
    public void FromIntArray_RejectsIncorrectPartCounts()
    {
        Assert.Throws<ArgumentException>(() => Uuid.FromIntArray(new int[3]));
        Assert.Throws<ArgumentException>(() => Uuid.FromIntArray(new int[5]));
    }

    [Fact]
    public void FromLongsAndProperties_MatchJavaUuidHalves()
    {
        const long mostSignificantBits = 0x1122334455667788L;
        const long leastSignificantBits = unchecked((long)0x99aabbccddeeff00UL);

        var uuid = Uuid.FromLongs(mostSignificantBits, leastSignificantBits);

        Assert.Equal(CanonicalText, uuid.ToString());
        Assert.Equal(mostSignificantBits, uuid.MostSignificantBits);
        Assert.Equal(leastSignificantBits, uuid.LeastSignificantBits);
    }

    [Fact]
    public void Version_ReturnsEveryCanonicalVersionNibble()
    {
        var bytes = new byte[16];

        for (var expected = 0; expected <= 15; expected++)
        {
            bytes.AsSpan().Clear();
            bytes[6] = (byte)(expected << 4);

            Assert.Equal(expected, new Uuid(bytes).Version);
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
        var bytes = new byte[16];
        bytes[8] = variantByte;

        Assert.Equal(expected, new Uuid(bytes).Variant);
    }

    [Theory]
    [InlineData("N")]
    [InlineData("D")]
    [InlineData("B")]
    [InlineData("P")]
    [InlineData("X")]
    public void ParseAndFormat_SupportGuidFormats(string format)
    {
        var guid = Guid.Parse(CanonicalText);
        var text = guid.ToString(format);
        var utf8Text = Encoding.UTF8.GetBytes(text);

        Assert.Equal(CanonicalText, Uuid.Parse(text).ToString());
        Assert.Equal(CanonicalText, Uuid.Parse(text.AsSpan()).ToString());
        Assert.Equal(CanonicalText, Uuid.Parse(utf8Text).ToString());
        Assert.Equal(CanonicalText, Uuid.Parse(text, provider: null).ToString());
        Assert.Equal(CanonicalText, Uuid.Parse(text.AsSpan(), provider: null).ToString());
        Assert.Equal(CanonicalText, Uuid.Parse(utf8Text, provider: null).ToString());
        Assert.Equal(text.ToLowerInvariant(), Uuid.Parse(text).ToString(format).ToLowerInvariant());
    }

    [Fact]
    public void ParseExactAndTryParseExact_RequireRequestedFormat()
    {
        var compact = Guid.Parse(CanonicalText).ToString("N");

        Assert.Equal(CanonicalText, Uuid.ParseExact(compact, "N").ToString());
        Assert.Equal(CanonicalText, Uuid.ParseExact(compact.AsSpan(), "N").ToString());
        Assert.True(Uuid.TryParseExact(compact, "N", out var fromString));
        Assert.True(Uuid.TryParseExact(compact.AsSpan(), "N", out var fromSpan));
        Assert.False(Uuid.TryParseExact(compact, "D", out _));
        Assert.Equal(fromString, fromSpan);
    }

    [Fact]
    public void ParsingInterfaces_ParseCharacterAndUtf8Spans()
    {
        var fromString = Parse<Uuid>(CanonicalText);
        var fromChars = ParseSpan<Uuid>(CanonicalText);
        var fromUtf8 = ParseUtf8<Uuid>(Encoding.UTF8.GetBytes(CanonicalText));

        Assert.Equal(fromString, fromChars);
        Assert.Equal(fromString, fromUtf8);
    }

    [Theory]
    [InlineData(CanonicalText, true)]
    [InlineData("112233445566778899aabbccddeeff00", true)]
    [InlineData("not-a-uuid", false)]
    [InlineData(null, false)]
    public void TryParse_HandlesSupportedAndInvalidRepresentations(string? text, bool expected)
    {
        var result = Uuid.TryParse(text, out var uuid);

        Assert.Equal(expected, result);
        Assert.Equal(expected ? CanonicalText : Uuid.Empty.ToString(), uuid.ToString());
    }

    [Fact]
    public void TryFormat_WritesCharactersAndUtf8WithoutIntermediateStrings()
    {
        var uuid = Uuid.Parse(CanonicalText);
        Span<char> characters = stackalloc char[36];
        Span<byte> utf8Bytes = stackalloc byte[36];

        Assert.True(uuid.TryFormat(characters, out var charsWritten));
        Assert.True(uuid.TryFormat(utf8Bytes, out var bytesWritten));
        Assert.Equal(CanonicalText, new string(characters[..charsWritten]));
        Assert.Equal(CanonicalText, Encoding.UTF8.GetString(utf8Bytes[..bytesWritten]));
    }

    [Fact]
    public void CanonicalRepresentations_AgreeForDeterministicValues()
    {
        var random = new Random(0x51d);
        var bytes = new byte[16];

        for (var iteration = 0; iteration < 128; iteration++)
        {
            random.NextBytes(bytes);
            var expected = new Uuid(bytes);
            var parts = new[]
            {
                BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(..4)),
                BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(4..8)),
                BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(8..12)),
                BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(12..))
            };

            Assert.Equal(expected, Uuid.Parse(expected.ToString()));
            Assert.Equal(expected, Uuid.FromLongs(expected.MostSignificantBits, expected.LeastSignificantBits));
            Assert.Equal(expected, Uuid.FromIntArray(parts));
            Assert.Equal(expected, (Uuid)(Guid)expected);
        }
    }

    [Fact]
    public void Comparison_UsesJavaSignedHalves()
    {
        var negativeMost = Uuid.FromLongs(long.MinValue, 0);
        var zero = Uuid.Empty;
        var negativeLeast = Uuid.FromLongs(0, long.MinValue);

        Assert.True(negativeMost < zero);
        Assert.True(zero > negativeMost);
        Assert.True(negativeMost <= zero);
        Assert.True(zero >= negativeMost);
        Assert.True(negativeLeast < zero);
        Assert.Equal(0, zero.CompareTo(Uuid.Empty));
        Assert.Equal(1, zero.CompareTo(null));
        Assert.Throws<ArgumentException>(() => zero.CompareTo(CanonicalText));
    }

    [Fact]
    public void EqualityHashingAndConversions_UseTheSameValue()
    {
        var guid = Guid.Parse(CanonicalText);
        Uuid first = guid;
        var second = new Uuid(guid);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.Equal(guid, (Guid)first);
    }

    [Fact]
    public void JsonSerialization_RoundTripsValuesAndPropertyNames()
    {
        var uuid = Uuid.Parse(CanonicalText);
        var values = new Dictionary<Uuid, string> { [uuid] = "value" };

        var uuidJson = JsonSerializer.Serialize(uuid);
        var valuesJson = JsonSerializer.Serialize(values);

        Assert.Equal($"\"{CanonicalText}\"", uuidJson);
        Assert.Equal(uuid, JsonSerializer.Deserialize<Uuid>(uuidJson));
        Assert.Equal($"{{\"{CanonicalText}\":\"value\"}}", valuesJson);
        Assert.Equal("value", JsonSerializer.Deserialize<Dictionary<Uuid, string>>(valuesJson)?[uuid]);
    }

    private static T Parse<T>(string input) where T : IParsable<T> => T.Parse(input, provider: null);

    private static T ParseSpan<T>(ReadOnlySpan<char> input) where T : ISpanParsable<T> => T.Parse(input, provider: null);

    private static T ParseUtf8<T>(ReadOnlySpan<byte> input) where T : IUtf8SpanParsable<T> => T.Parse(input, provider: null);
}
