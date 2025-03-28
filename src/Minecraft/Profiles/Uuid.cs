using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Void.Minecraft.Profiles;

public struct Uuid(Guid guid)
{
    public static Uuid Empty { get; } = new(Guid.Empty);

    public readonly Guid AsGuid => guid;

    public override readonly string ToString()
    {
        return AsGuid.ToString();
    }

    public static Uuid Parse(string text)
    {
        return new Uuid(Guid.Parse(text));
    }

    public static Uuid Parse(params int[] parts)
    {
        if (parts.Length is not 4)
            throw new ArgumentException("Arguments size should be 4", nameof(parts));

        var m0Bytes = BitConverter.GetBytes(parts[0]); // m0: first 4 bytes (little-endian)
        var m1Bytes = BitConverter.GetBytes(parts[1]); // m1: next 4 bytes
        var l0Bytes = BitConverter.GetBytes(parts[2]); // l0: third int
        var l1Bytes = BitConverter.GetBytes(parts[3]); // l1: fourth int

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

    public static Uuid FromStringHash(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var i128 = new Int128();
        MD5.TryHashData(Encoding.UTF8.GetBytes(text), i128.AsSpan(), out _);

        i128.version = (byte)(i128.version & 0x0f | 0x30);
        i128.variant = (byte)(i128.variant & 0x3f | 0x80);

        return new Uuid(Unsafe.As<Int128, Guid>(ref i128));
    }

    public static Uuid FromLongs(long mostSig, long leastSig)
    {
        var mostSigBytes = BitConverter.GetBytes(mostSig);
        var leastSigBytes = BitConverter.GetBytes(leastSig);

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

    public int GetVersion()
    {
        ref var i128 = ref Unsafe.As<Guid, Int128>(ref guid);
        return i128.version >> 4;
    }

    public int GetVariant()
    {
        ref var i128 = ref Unsafe.As<Guid, Int128>(ref guid);
        return (i128.variant >> 4) switch
        {
            <= 0b0111 => 0,
            <= 1011 => 1,
            <= 1101 => 2,
            1110 => 3,
            _ => -1
        };
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