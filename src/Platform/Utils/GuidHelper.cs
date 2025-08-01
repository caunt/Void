using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Void.Proxy.Utils;

public static class GuidHelper
{
    public static Guid FromStringHash(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var i128 = new Int128();
        MD5.HashData(Encoding.UTF8.GetBytes(text), i128.AsSpan());

        i128.version = (byte)((i128.version & 0x0f) | 0x30);
        i128.variant = (byte)((i128.variant & 0x3f) | 0x80);

        return Unsafe.As<Int128, Guid>(ref i128);
    }

    public static Guid FromLongs(long mostSig, long leastSig)
    {
        var mostSigBytes = BitConverter.GetBytes(mostSig);
        var leastSigBytes = BitConverter.GetBytes(leastSig);

        Span<byte> guidBytes =
            //Is there a better way??
            [
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
            ];

        return new Guid(guidBytes);
    }

    public static int GetVersion(Guid guid)
    {
        ref var i128 = ref Unsafe.As<Guid, Int128>(ref guid);
        return i128.version >> 4;
    }

    public static int GetVariant(Guid guid)
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

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    private struct Int128
    {
        [FieldOffset(0)] public int a;
        [FieldOffset(4)] public int b;
        [FieldOffset(8)] public int c;
        [FieldOffset(12)] public int d;

        [FieldOffset(0)] private byte _start;

        [FieldOffset(7)] public byte version;
        [FieldOffset(8)] public byte variant;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan()
        {
            return MemoryMarshal.CreateSpan(ref _start, 16);
        }
    }
}

public sealed class GuidConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetGuid();
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
