using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Void.Minecraft.Profiles.Serializers;

namespace Void.Minecraft.Profiles;

/// <summary>
/// Represents a Java-compatible UUID with canonical network-order byte semantics.
/// </summary>
[JsonConverter(typeof(UuidJsonConverter))]
public readonly struct Uuid : IComparable, IComparable<Uuid>, IEquatable<Uuid>, IParsable<Uuid>, ISpanFormattable, ISpanParsable<Uuid>, IUtf8SpanFormattable, IUtf8SpanParsable<Uuid>
{
    private const int MaxStackAllocatedByteCount = 256;
    private const int UuidByteCount = 16;
    private readonly Guid _value;

    /// <summary>Represents a UUID whose value is all zeros.</summary>
    public static readonly Uuid Empty;

    /// <summary>Gets a UUID whose value has every bit set.</summary>
    public static Uuid AllBitsSet => new(Guid.AllBitsSet);

    /// <summary>Initializes a UUID from an equivalent <see cref="Guid"/> value.</summary>
    public Uuid(Guid value) => _value = value;

    /// <summary>Initializes a UUID from a supported textual representation.</summary>
    public Uuid(string input) => _value = new Guid(input);

    /// <summary>Initializes a UUID from 16 canonical network-order bytes.</summary>
    public Uuid(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        _value = new Guid(bytes, bigEndian: true);
    }

    /// <summary>Initializes a UUID from 16 canonical network-order bytes.</summary>
    public Uuid(ReadOnlySpan<byte> bytes) => _value = new Guid(bytes, bigEndian: true);

    /// <summary>Initializes a UUID from 16 bytes using the specified byte order.</summary>
    public Uuid(ReadOnlySpan<byte> bytes, bool bigEndian) => _value = new Guid(bytes, bigEndian);

    /// <summary>Gets the UUID version field.</summary>
    public int Version => _value.Version;

    /// <summary>Gets the UUID variant using Java's <c>UUID.variant()</c> values.</summary>
    public int Variant => _value.Variant switch
    {
        <= 0b0111 => 0,
        <= 0b1011 => 2,
        <= 0b1101 => 6,
        _ => 7
    };

    /// <summary>Gets the most significant 64 bits as a signed Java <see langword="long"/>.</summary>
    public long MostSignificantBits
    {
        get
        {
            GetLongs(out var mostSignificantBits, out _);
            return mostSignificantBits;
        }
    }

    /// <summary>Gets the least significant 64 bits as a signed Java <see langword="long"/>.</summary>
    public long LeastSignificantBits
    {
        get
        {
            GetLongs(out _, out var leastSignificantBits);
            return leastSignificantBits;
        }
    }

    /// <summary>Creates a random version-4 UUID.</summary>
    public static Uuid NewUuid() => new(Guid.NewGuid());

    /// <summary>Creates a version-7 UUID using the current UTC time.</summary>
    public static Uuid CreateVersion7() => new(Guid.CreateVersion7());

    /// <summary>Creates a version-7 UUID using the supplied UTC timestamp.</summary>
    public static Uuid CreateVersion7(DateTimeOffset timestamp) => new(Guid.CreateVersion7(timestamp));

    /// <summary>Creates a Java-compatible version-3 UUID from UTF-8 text.</summary>
    public static Uuid CreateVersion3(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return CreateVersion3([], name);
    }

    /// <summary>Creates a Java-compatible version-3 UUID from the supplied name bytes.</summary>
    public static Uuid CreateVersion3(ReadOnlySpan<byte> name)
    {
        Span<byte> hash = stackalloc byte[MD5.HashSizeInBytes];
        MD5.HashData(name, hash);
        hash[6] = (byte)(hash[6] & 0x0f | 0x30);
        hash[8] = (byte)(hash[8] & 0x3f | 0x80);
        return new Uuid(hash);
    }

    /// <summary>Creates the Java-compatible offline-player UUID for a Minecraft username.</summary>
    public static Uuid CreateOfflinePlayer(string username)
    {
        ArgumentNullException.ThrowIfNull(username);
        return CreateVersion3("OfflinePlayer:", username);
    }

    /// <summary>Creates a UUID from Java's signed most- and least-significant halves.</summary>
    public static Uuid FromLongs(long mostSignificantBits, long leastSignificantBits)
    {
        Span<byte> bytes = stackalloc byte[UuidByteCount];
        BinaryPrimitives.WriteInt64BigEndian(bytes[..8], mostSignificantBits);
        BinaryPrimitives.WriteInt64BigEndian(bytes[8..], leastSignificantBits);
        return new Uuid(bytes);
    }

    /// <summary>Creates a UUID from the four-integer representation used by Minecraft.</summary>
    public static Uuid FromIntArray(ReadOnlySpan<int> parts)
    {
        if (parts.Length is not 4)
            throw new ArgumentException($"Expected 4 parts but got {parts.Length}. A UUID requires exactly 4 integer parts.", nameof(parts));

        Span<byte> bytes = stackalloc byte[UuidByteCount];
        BinaryPrimitives.WriteInt32BigEndian(bytes[..4], parts[0]);
        BinaryPrimitives.WriteInt32BigEndian(bytes[4..8], parts[1]);
        BinaryPrimitives.WriteInt32BigEndian(bytes[8..12], parts[2]);
        BinaryPrimitives.WriteInt32BigEndian(bytes[12..], parts[3]);
        return new Uuid(bytes);
    }

    /// <inheritdoc cref="Guid.Parse(string)"/>
    public static Uuid Parse(string input) => new(Guid.Parse(input));

    /// <inheritdoc cref="Guid.Parse(string, IFormatProvider?)"/>
    public static Uuid Parse(string input, IFormatProvider? provider) => new(Guid.Parse(input, provider));

    /// <inheritdoc cref="Guid.Parse(ReadOnlySpan{char})"/>
    public static Uuid Parse(ReadOnlySpan<char> input) => new(Guid.Parse(input));

    /// <inheritdoc cref="Guid.Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
    public static Uuid Parse(ReadOnlySpan<char> input, IFormatProvider? provider) => new(Guid.Parse(input, provider));

    /// <inheritdoc cref="Guid.Parse(ReadOnlySpan{byte})"/>
    public static Uuid Parse(ReadOnlySpan<byte> utf8Text) => new(Guid.Parse(utf8Text));

    /// <inheritdoc cref="Guid.Parse(ReadOnlySpan{byte}, IFormatProvider?)"/>
    public static Uuid Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => new(Guid.Parse(utf8Text, provider));

    /// <inheritdoc cref="Guid.ParseExact(string, string)"/>
    public static Uuid ParseExact(string input, string format) => new(Guid.ParseExact(input, format));

    /// <inheritdoc cref="Guid.ParseExact(ReadOnlySpan{char}, ReadOnlySpan{char})"/>
    public static Uuid ParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format) => new(Guid.ParseExact(input, format));

    /// <inheritdoc cref="Guid.TryParse(string?, out Guid)"/>
    public static bool TryParse(string? input, out Uuid result) => WrapTryParseResult(Guid.TryParse(input, out var value), value, out result);

    /// <inheritdoc cref="Guid.TryParse(string?, IFormatProvider?, out Guid)"/>
    public static bool TryParse(string? input, IFormatProvider? provider, out Uuid result) => WrapTryParseResult(Guid.TryParse(input, provider, out var value), value, out result);

    /// <inheritdoc cref="Guid.TryParse(ReadOnlySpan{char}, out Guid)"/>
    public static bool TryParse(ReadOnlySpan<char> input, out Uuid result) => WrapTryParseResult(Guid.TryParse(input, out var value), value, out result);

    /// <inheritdoc cref="Guid.TryParse(ReadOnlySpan{char}, IFormatProvider?, out Guid)"/>
    public static bool TryParse(ReadOnlySpan<char> input, IFormatProvider? provider, out Uuid result) => WrapTryParseResult(Guid.TryParse(input, provider, out var value), value, out result);

    /// <inheritdoc cref="Guid.TryParse(ReadOnlySpan{byte}, out Guid)"/>
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out Uuid result) => WrapTryParseResult(Guid.TryParse(utf8Text, out var value), value, out result);

    /// <inheritdoc cref="Guid.TryParse(ReadOnlySpan{byte}, IFormatProvider?, out Guid)"/>
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out Uuid result) => WrapTryParseResult(Guid.TryParse(utf8Text, provider, out var value), value, out result);

    /// <inheritdoc cref="Guid.TryParseExact(string?, string?, out Guid)"/>
    public static bool TryParseExact(string? input, string? format, out Uuid result) => WrapTryParseResult(Guid.TryParseExact(input, format, out var value), value, out result);

    /// <inheritdoc cref="Guid.TryParseExact(ReadOnlySpan{char}, ReadOnlySpan{char}, out Guid)"/>
    public static bool TryParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format, out Uuid result) => WrapTryParseResult(Guid.TryParseExact(input, format, out var value), value, out result);

    /// <summary>Returns the UUID as 16 canonical network-order bytes.</summary>
    public byte[] ToByteArray() => _value.ToByteArray(bigEndian: true);

    /// <inheritdoc cref="Guid.ToByteArray(bool)"/>
    public byte[] ToByteArray(bool bigEndian) => _value.ToByteArray(bigEndian);

    /// <summary>Writes the UUID as 16 canonical network-order bytes.</summary>
    public bool TryWriteBytes(Span<byte> destination) => _value.TryWriteBytes(destination, bigEndian: true, out _);

    /// <inheritdoc cref="Guid.TryWriteBytes(Span{byte}, bool, out int)"/>
    public bool TryWriteBytes(Span<byte> destination, bool bigEndian, out int bytesWritten) => _value.TryWriteBytes(destination, bigEndian, out bytesWritten);

    /// <inheritdoc cref="Guid.ToString()"/>
    public override string ToString() => _value.ToString();

    /// <inheritdoc cref="Guid.ToString(string?)"/>
    public string ToString(string? format) => _value.ToString(format);

    /// <inheritdoc cref="Guid.ToString(string?, IFormatProvider?)"/>
    public string ToString(string? format, IFormatProvider? provider) => _value.ToString(format, provider);

    /// <inheritdoc cref="Guid.TryFormat(Span{char}, out int, ReadOnlySpan{char})"/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default) => _value.TryFormat(destination, out charsWritten, format);

    /// <inheritdoc cref="Guid.TryFormat(Span{byte}, out int, ReadOnlySpan{char})"/>
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format = default) => _value.TryFormat(utf8Destination, out bytesWritten, format);

    /// <summary>Compares UUIDs using Java's signed most- and least-significant halves.</summary>
    public int CompareTo(Uuid other)
    {
        GetLongs(out var mostSignificantBits, out var leastSignificantBits);
        other.GetLongs(out var otherMostSignificantBits, out var otherLeastSignificantBits);

        var result = mostSignificantBits.CompareTo(otherMostSignificantBits);
        return result is not 0 ? result : leastSignificantBits.CompareTo(otherLeastSignificantBits);
    }

    /// <inheritdoc cref="IComparable.CompareTo(object?)"/>
    public int CompareTo(object? value)
    {
        if (value is null)
            return 1;

        if (value is not Uuid uuid)
            throw new ArgumentException($"Object must be of type {nameof(Uuid)}.", nameof(value));

        return CompareTo(uuid);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    public bool Equals(Uuid other) => _value.Equals(other._value);

    /// <inheritdoc cref="object.Equals(object?)"/>
    public override bool Equals(object? value) => value is Uuid other && Equals(other);

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>Determines whether two UUIDs are equal.</summary>
    public static bool operator ==(Uuid left, Uuid right) => left.Equals(right);

    /// <summary>Determines whether two UUIDs are unequal.</summary>
    public static bool operator !=(Uuid left, Uuid right) => !left.Equals(right);

    /// <summary>Determines whether one UUID precedes another using Java ordering.</summary>
    public static bool operator <(Uuid left, Uuid right) => left.CompareTo(right) < 0;

    /// <summary>Determines whether one UUID follows another using Java ordering.</summary>
    public static bool operator >(Uuid left, Uuid right) => left.CompareTo(right) > 0;

    /// <summary>Determines whether one UUID precedes or equals another using Java ordering.</summary>
    public static bool operator <=(Uuid left, Uuid right) => left.CompareTo(right) <= 0;

    /// <summary>Determines whether one UUID follows or equals another using Java ordering.</summary>
    public static bool operator >=(Uuid left, Uuid right) => left.CompareTo(right) >= 0;

    /// <summary>Converts a UUID to its equivalent <see cref="Guid"/> value.</summary>
    public static implicit operator Guid(Uuid value) => value._value;

    /// <summary>Converts a <see cref="Guid"/> to its equivalent UUID value.</summary>
    public static implicit operator Uuid(Guid value) => new(value);

    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(destination, out charsWritten, format);

    bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => TryFormat(utf8Destination, out bytesWritten, format);

    private static bool WrapTryParseResult(bool success, Guid value, out Uuid result)
    {
        result = new Uuid(value);
        return success;
    }

    private static Uuid CreateVersion3(ReadOnlySpan<char> prefix, ReadOnlySpan<char> name)
    {
        var byteCount = Encoding.UTF8.GetByteCount(prefix) + Encoding.UTF8.GetByteCount(name);
        byte[]? rentedBytes = null;
        Span<byte> bytes = byteCount <= MaxStackAllocatedByteCount
            ? stackalloc byte[byteCount]
            : (rentedBytes = ArrayPool<byte>.Shared.Rent(byteCount)).AsSpan(0, byteCount);

        try
        {
            var bytesWritten = Encoding.UTF8.GetBytes(prefix, bytes);
            Encoding.UTF8.GetBytes(name, bytes[bytesWritten..]);
            return CreateVersion3(bytes);
        }
        finally
        {
            if (rentedBytes is not null)
                ArrayPool<byte>.Shared.Return(rentedBytes, clearArray: true);
        }
    }

    private void GetLongs(out long mostSignificantBits, out long leastSignificantBits)
    {
        Span<byte> bytes = stackalloc byte[UuidByteCount];
        _ = TryWriteBytes(bytes);
        mostSignificantBits = BinaryPrimitives.ReadInt64BigEndian(bytes[..8]);
        leastSignificantBits = BinaryPrimitives.ReadInt64BigEndian(bytes[8..]);
    }
}
