using System.Buffers;

namespace Void.Proxy.API.Network.IO.Streams.Extensions;

public static class ManualStreamExtensions
{
    public static int ReadVarInt(this IMinecraftManualStream stream)
    {
        var numRead = 0;
        var result = 0;
        byte read;
        byte[] buffer = [0];

        do
        {
            stream.ReadExactly(buffer);

            read = buffer[0];
            var value = read & 0b01111111;
            result |= value << (7 * numRead);

            numRead++;
            if (numRead > 5)
                throw new InvalidOperationException("VarInt is too big");
        } while ((read & 0b10000000) != 0);

        return result;
    }

    public static async ValueTask<int> ReadVarIntAsync(this IMinecraftManualStream stream, CancellationToken cancellationToken = default)
    {
        var numRead = 0;
        var result = 0;
        byte read;
        byte[] buffer = [0];

        do
        {
            await stream.ReadExactlyAsync(buffer);

            read = buffer[0];
            var value = read & 0b01111111;
            result |= value << (7 * numRead);

            numRead++;
            if (numRead > 5)
                throw new InvalidOperationException("VarInt is too big");
        } while ((read & 0b10000000) != 0);

        return result;
    }

    public static void WriteVarInt(this IMinecraftManualStream stream, int value)
    {
        var unsigned = (uint)value;
        var buffer = ArrayPool<byte>.Shared.Rent(5);
        var idx = 0;

        do
        {
            var temp = (byte)(unsigned & 127);

            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            buffer[idx++] = temp;
        } while (unsigned != 0);

        stream.Write(buffer.AsSpan(0, idx));
        ArrayPool<byte>.Shared.Return(buffer);
    }

    public static async ValueTask WriteVarIntAsync(this IMinecraftManualStream stream, int value, CancellationToken cancellationToken = default)
    {
        var unsigned = (uint)value;
        var buffer = ArrayPool<byte>.Shared.Rent(5);
        var idx = 0;

        do
        {
            var temp = (byte)(unsigned & 127);

            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            buffer[idx++] = temp;
        } while (unsigned != 0);

        await stream.WriteAsync(buffer.AsMemory(0, idx));
        ArrayPool<byte>.Shared.Return(buffer);
    }
}