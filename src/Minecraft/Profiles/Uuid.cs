using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Void.Minecraft.Profiles.Serializers;

namespace Void.Minecraft.Profiles;

/// <summary>
/// A Minecraft-compatible UUID backed by a .NET <see cref="Guid"/>, with factory methods for the
/// wire-format encodings used by the Java Edition protocol.
/// </summary>
[JsonConverter(typeof(UuidJsonConverter))]
public readonly struct Uuid(Guid guid) : IComparable<Uuid>, IEquatable<Uuid>
{
    private const int MaxStackAllocatedByteCount = 256;

    /// <summary>
    /// Gets the zero UUID (<c>00000000-0000-0000-0000-000000000000</c>), wrapping <see cref="Guid.Empty"/>.
    /// </summary>
    public static Uuid Empty { get; } = new(Guid.Empty);

    /// <summary>
    /// Gets the underlying .NET <see cref="Guid"/> value.
    /// </summary>
    public readonly Guid AsGuid => guid;

    /// <summary>
    /// Returns the standard hyphenated lowercase UUID string representation,
    /// for example <c>"550e8400-e29b-41d4-a716-446655440000"</c>.
    /// </summary>
    public override readonly string ToString()
    {
        return AsGuid.ToString();
    }

    /// <summary>
    /// Creates a new random UUID (Version 4).
    /// </summary>
    /// <returns>A new <see cref="Uuid"/> backed by a freshly generated <see cref="Guid"/>.</returns>
    public static Uuid NewUuid()
    {
        return new Uuid(Guid.NewGuid());
    }

    /// <summary>
    /// Parses a UUID from its standard string representation.
    /// </summary>
    /// <param name="text">The UUID string to parse.</param>
    /// <returns>The parsed <see cref="Uuid"/>.</returns>
    /// <exception cref="FormatException">
    /// <paramref name="text"/> is not in a recognized UUID format.
    /// </exception>
    public static Uuid Parse(string text)
    {
        return new Uuid(Guid.Parse(text));
    }

    /// <summary>
    /// Attempts to parse a UUID string. Returns <see langword="true"/> and sets <paramref name="uuid"/>
    /// on success; returns <see langword="false"/> and sets <paramref name="uuid"/> to the default value on failure.
    /// </summary>
    /// <param name="text">The UUID string to parse, or <see langword="null"/>.</param>
    /// <param name="uuid">
    /// When this method returns <see langword="true"/>, contains the parsed <see cref="Uuid"/>;
    /// otherwise, the default <see cref="Uuid"/> value.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="text"/> was successfully parsed; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryParse(string? text, out Uuid uuid)
    {
        if (Guid.TryParse(text, out var guid))
        {
            uuid = new Uuid(guid);
            return true;
        }

        uuid = default;
        return false;
    }

    /// <summary>
    /// Constructs a UUID from exactly four integers as encoded in the Minecraft Java Edition protocol.
    /// </summary>
    /// <remarks>
    /// In the Java Edition protocol, a UUID is transmitted as two 64-bit halves, each split into two
    /// big-endian <see langword="int"/> values. This method reorders the bytes to produce the equivalent
    /// .NET <see cref="Guid"/> representation.
    /// </remarks>
    /// <param name="parts">An array of exactly four <see langword="int"/> values representing the UUID.</param>
    /// <returns>The <see cref="Uuid"/> reconstructed from the four integer parts.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="parts"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="parts"/> does not contain exactly four elements.
    /// </exception>
    public static Uuid Parse(params int[] parts)
    {
        ArgumentNullException.ThrowIfNull(parts);

        if (parts.Length is not 4)
            throw new ArgumentException($"Expected 4 parts but got {parts.Length}. A UUID requires exactly 4 integer parts.", nameof(parts));

        Span<byte> bytes = stackalloc byte[16];
        BinaryPrimitives.WriteInt32BigEndian(bytes[..4], parts[0]);
        BinaryPrimitives.WriteInt32BigEndian(bytes[4..8], parts[1]);
        BinaryPrimitives.WriteInt32BigEndian(bytes[8..12], parts[2]);
        BinaryPrimitives.WriteInt32BigEndian(bytes[12..], parts[3]);

        return new Uuid(new Guid(bytes, bigEndian: true));
    }

    /// <summary>
    /// Derives a deterministic UUID from a UTF-8 string by computing its MD5 hash and stamping
    /// the result with UUID Version 3 bits and the RFC 4122 variant bits.
    /// </summary>
    /// <param name="text">The input string to hash. Cannot be <see langword="null"/>.</param>
    /// <returns>A UUID whose 128-bit value is the MD5 hash of <paramref name="text"/>, with the version and variant fields set.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
    public static Uuid FromStringHash(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var byteCount = Encoding.UTF8.GetByteCount(text);
        byte[]? rentedBytes = null;
        Span<byte> textBytes = byteCount <= MaxStackAllocatedByteCount
            ? stackalloc byte[byteCount]
            : (rentedBytes = ArrayPool<byte>.Shared.Rent(byteCount)).AsSpan(0, byteCount);

        try
        {
            Encoding.UTF8.GetBytes(text, textBytes);

            Span<byte> hash = stackalloc byte[MD5.HashSizeInBytes];
            MD5.HashData(textBytes, hash);
            hash[6] = (byte)(hash[6] & 0x0f | 0x30);
            hash[8] = (byte)(hash[8] & 0x3f | 0x80);

            return new Uuid(new Guid(hash, bigEndian: true));
        }
        finally
        {
            if (rentedBytes is not null)
                ArrayPool<byte>.Shared.Return(rentedBytes, clearArray: true);
        }
    }

    /// <summary>
    /// Reconstructs a UUID from the Java <c>UUID.getMostSignificantBits()</c> and
    /// <c>UUID.getLeastSignificantBits()</c> long values.
    /// </summary>
    /// <remarks>
    /// The byte reordering translates from Java's big-endian UUID representation to the mixed-endian
    /// layout used by .NET's <see cref="Guid"/>.
    /// </remarks>
    /// <param name="mostSig">The most significant 64 bits of the UUID.</param>
    /// <param name="leastSig">The least significant 64 bits of the UUID.</param>
    /// <returns>The <see cref="Uuid"/> equivalent to the Java UUID with the given bit halves.</returns>
    public static Uuid FromLongs(long mostSig, long leastSig)
    {
        Span<byte> bytes = stackalloc byte[16];
        BinaryPrimitives.WriteInt64BigEndian(bytes[..8], mostSig);
        BinaryPrimitives.WriteInt64BigEndian(bytes[8..], leastSig);

        return new Uuid(new Guid(bytes, bigEndian: true));
    }

    /// <summary>
    /// Derives the deterministic offline-player UUID for the given player name using the
    /// Bukkit/Spigot convention.
    /// </summary>
    /// <remarks>
    /// The string <c>"OfflinePlayer:&lt;name&gt;"</c> is UTF-8 encoded, MD5-hashed, and stamped with
    /// UUID Version 3 bits and RFC 4122 variant bits. The result matches the UUID that
    /// Bukkit-compatible servers assign to players connecting in offline mode.
    /// </remarks>
    /// <param name="name">The player name. Cannot be <see langword="null"/>.</param>
    /// <returns>A deterministic <see cref="Uuid"/> for the offline player with the given name.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    public static Uuid Offline(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return FromStringHash($"OfflinePlayer:{name}");
    }

    /// <summary>
    /// Gets the UUID version number extracted from the version nibble of the underlying 128-bit value.
    /// </summary>
    /// <value>An integer between 0 and 15 representing the UUID version field.</value>
    public int Version => AsGuid.Version;

    /// <summary>
    /// Gets the UUID variant using the values returned by Java's <c>UUID.variant()</c> method.
    /// </summary>
    /// <value>
    /// <c>0</c> for NCS backward compatibility, <c>2</c> for RFC 4122, <c>6</c> for Microsoft,
    /// or <c>7</c> for future reserved.
    /// </value>
    public int Variant => AsGuid.Variant switch
    {
        <= 0b0111 => 0,
        <= 0b1011 => 2,
        <= 0b1101 => 6,
        _ => 7
    };

    /// <summary>
    /// Compares this UUID to <paramref name="other"/> using the underlying <see cref="Guid"/> comparison.
    /// </summary>
    /// <param name="other">The UUID to compare against.</param>
    /// <returns>A negative integer, zero, or a positive integer if this instance is less than, equal to,
    /// or greater than <paramref name="other"/>, respectively.</returns>
    public readonly int CompareTo(Uuid other)
    {
        return AsGuid.CompareTo(other.AsGuid);
    }

    /// <summary>
    /// Returns <see langword="true"/> if this UUID equals <paramref name="other"/> by comparing their
    /// underlying <see cref="Guid"/> values.
    /// </summary>
    /// <param name="other">The UUID to compare against.</param>
    /// <returns><see langword="true"/> if the two UUIDs are equal; otherwise, <see langword="false"/>.</returns>
    public readonly bool Equals(Uuid other)
    {
        return AsGuid.Equals(other.AsGuid);
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="obj"/> is a <see cref="Uuid"/> equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Uuid"/> with the same value;
    /// otherwise, <see langword="false"/>.</returns>
    public override readonly bool Equals(object? obj)
    {
        return obj is Uuid other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code of the underlying <see cref="Guid"/>.
    /// </summary>
    /// <returns>The hash code for this UUID.</returns>
    public override readonly int GetHashCode()
    {
        return AsGuid.GetHashCode();
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are equal.
    /// </summary>
    /// <param name="left">The first UUID to compare.</param>
    /// <param name="right">The second UUID to compare.</param>
    /// <returns><see langword="true"/> if the two UUIDs are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(Uuid left, Uuid right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are not equal.
    /// </summary>
    /// <param name="left">The first UUID to compare.</param>
    /// <param name="right">The second UUID to compare.</param>
    /// <returns><see langword="true"/> if the two UUIDs are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(Uuid left, Uuid right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Converts a Minecraft UUID to its underlying .NET GUID.
    /// </summary>
    /// <param name="uuid">The UUID to convert.</param>
    /// <returns>The underlying <see cref="Guid" /> value.</returns>
    public static implicit operator Guid(Uuid uuid)
    {
        return uuid.AsGuid;
    }

    /// <summary>
    /// Wraps a .NET GUID as a Minecraft UUID.
    /// </summary>
    /// <param name="guid">The GUID to wrap.</param>
    /// <returns>A UUID containing the same 128-bit value.</returns>
    public static implicit operator Uuid(Guid guid)
    {
        return new Uuid(guid);
    }
}
