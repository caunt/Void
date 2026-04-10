using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
public struct Uuid(Guid guid) : IComparable<Uuid>, IEquatable<Uuid>
{
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
    /// <exception cref="ArgumentException">
    /// <paramref name="parts"/> does not contain exactly four elements.
    /// </exception>
    public static Uuid Parse(params int[] parts)
    {
        if (parts.Length is not 4)
            throw new ArgumentException($"Expected 4 parts but got {parts.Length}. A UUID requires exactly 4 integer parts.", nameof(parts));

        Span<byte> m0Bytes = stackalloc byte[4]; // m0: first 4 bytes (little-endian)
        BinaryPrimitives.WriteInt32LittleEndian(m0Bytes, parts[0]);
        Span<byte> m1Bytes = stackalloc byte[4]; // m1: next 4 bytes
        BinaryPrimitives.WriteInt32LittleEndian(m1Bytes, parts[1]);
        Span<byte> l0Bytes = stackalloc byte[4]; // l0: third int
        BinaryPrimitives.WriteInt32LittleEndian(l0Bytes, parts[2]);
        Span<byte> l1Bytes = stackalloc byte[4]; // l1: fourth int
        BinaryPrimitives.WriteInt32LittleEndian(l1Bytes, parts[3]);

        return new Uuid(new Guid([
            m0Bytes[0],
            m0Bytes[1],
            m0Bytes[2],
            m0Bytes[3],
            m1Bytes[2],
            m1Bytes[3],
            m1Bytes[0],
            m1Bytes[1],
            l0Bytes[3],
            l0Bytes[2],
            l0Bytes[1],
            l0Bytes[0],
            l1Bytes[3],
            l1Bytes[2],
            l1Bytes[1],
            l1Bytes[0]
        ]));
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

        var i128 = new Int128();
        Span<byte> textBytes = stackalloc byte[Encoding.UTF8.GetByteCount(text)];
        Encoding.UTF8.GetBytes(text, textBytes);
        MD5.TryHashData(textBytes, i128.AsSpan(), out _);

        i128.version = (byte)(i128.version & 0x0f | 0x30);
        i128.variant = (byte)(i128.variant & 0x3f | 0x80);

        return new Uuid(Unsafe.As<Int128, Guid>(ref i128));
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
        var mostSigBytes = bytes[..8];
        var leastSigBytes = bytes[8..];

        BinaryPrimitives.WriteInt64LittleEndian(mostSigBytes, mostSig);
        BinaryPrimitives.WriteInt64LittleEndian(leastSigBytes, leastSig);

        return new Uuid(new Guid([
            mostSigBytes[4],
            mostSigBytes[5],
            mostSigBytes[6],
            mostSigBytes[7],
            mostSigBytes[2],
            mostSigBytes[3],
            mostSigBytes[0],
            mostSigBytes[1],
            leastSigBytes[7],
            leastSigBytes[6],
            leastSigBytes[5],
            leastSigBytes[4],
            leastSigBytes[3],
            leastSigBytes[2],
            leastSigBytes[1],
            leastSigBytes[0]
        ]));
    }

    /// <summary>
    /// Derives the deterministic offline-player UUID for the given player name using the
    /// Bukkit/Spigot convention.
    /// </summary>
    /// <remarks>
    /// The string <c>"OfflinePlayer:<name>"</c> is UTF-8 encoded, MD5-hashed, and stamped with
    /// UUID Version 3 bits and RFC 4122 variant bits. The result matches the UUID that
    /// Bukkit-compatible servers assign to players connecting in offline mode.
    /// </remarks>
    /// <param name="name">The player name. Cannot be <see langword="null"/>.</param>
    /// <returns>A deterministic <see cref="Uuid"/> for the offline player with the given name.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    public static Uuid Offline(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        var i128 = new Int128();
        var text = $"OfflinePlayer:{name}";
        Span<byte> textBytes = stackalloc byte[Encoding.UTF8.GetByteCount(text)];
        Encoding.UTF8.GetBytes(text, textBytes);
        MD5.TryHashData(textBytes, i128.AsSpan(), out _);

        i128.version = (byte)(i128.version & 0x0f | 0x30);
        i128.variant = (byte)(i128.variant & 0x3f | 0x80);

        return new Uuid(Unsafe.As<Int128, Guid>(ref i128));
    }

    /// <summary>
    /// Returns the UUID version number extracted from the version nibble of the underlying 128-bit value.
    /// </summary>
    /// <returns>An integer between 1 and 5 representing the UUID version field.</returns>
    public int GetVersion()
    {
        ref var i128 = ref Unsafe.As<Guid, Int128>(ref guid);
        return i128.version >> 4;
    }

    /// <summary>
    /// Returns the UUID variant as an integer decoded from the variant byte.
    /// </summary>
    /// <returns>
    /// <c>0</c> for NCS backward compatibility, <c>1</c> for RFC 4122, <c>2</c> for Microsoft,
    /// <c>3</c> for future reserved, or <c>-1</c> if the variant byte is not in an expected range.
    /// </returns>
    public int GetVariant()
    {
        ref var i128 = ref Unsafe.As<Guid, Int128>(ref guid);
        return (i128.variant >> 4) switch
        {
            <= 0b0111 => 0,
            <= 0b1011 => 1,
            <= 0b1101 => 2,
            <= 0b1111 => 3,
            _ => -1
        };
    }

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

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    private struct Int128
    {
        [FieldOffset(0)] public int a;
        [FieldOffset(4)] public int b;
        [FieldOffset(8)] public int c;
        [FieldOffset(12)] public int d;

        [FieldOffset(0)] private byte start;

        [FieldOffset(7)] public byte version;
        [FieldOffset(8)] public byte variant;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan()
        {
            return MemoryMarshal.CreateSpan(ref start, 16);
        }
    }
}
