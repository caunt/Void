using Ionic.Zlib;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System.Numerics;

namespace MinecraftProxy.Network;

public class MinecraftStream(Stream source)
{
    public bool CanRead => source.CanRead;
    public bool CanWrite => source.CanWrite;

    public int CompressionThreshold { get; set; }

    public async Task<(int, MinecraftBuffer)> ReadPacketAsync()
    {
        var length = await ReadVarIntAsync();

        if (length <= 0)
            return (-1, null!); // Connection closed

        var packet = new byte[length];
        await ReadAsync(packet);

        var buffer = new MinecraftBuffer(packet);
        var compressionEnabled = CompressionThreshold > 0;
        var dataLength = compressionEnabled ? buffer.ReadVarInt() : 0;
        var compressed = dataLength > 0;

        if (compressed)
        {
            var input = buffer.ReadUInt8Array((int)buffer.Length - GetVarIntSize(dataLength));
            var output = new byte[dataLength];

            var zlibCodec = new ZlibCodec
            {
                InputBuffer = input,
                NextIn = 0,
                OutputBuffer = output,
                NextOut = 0
            };

            var status = zlibCodec.InitializeInflate();

            if (status != ZlibConstants.Z_OK)
                throw new IOException($"Cannot initialize zlib inflate: {status}");

            while (zlibCodec.TotalBytesOut < output.Length && zlibCodec.TotalBytesIn < input.Length) 
            {
                zlibCodec.AvailableBytesIn = zlibCodec.AvailableBytesOut = 1; // force small buffers
                status = zlibCodec.Inflate(FlushType.None);

                if (status != ZlibConstants.Z_OK)
                    throw new IOException($"Cannot do zlib inflate: {status}");
            }

            status = zlibCodec.EndInflate();

            if (status != ZlibConstants.Z_OK)
                throw new IOException($"Cannot end zlib inflate: {status}");

            buffer = new(output);
            length = dataLength;
        }

        var packetId = buffer.ReadVarInt();
        var data = buffer.ReadUInt8Array((int)(buffer.Length - buffer.Position) /*length - GetVarIntSize(packetId) - (compressionEnabled ? GetVarIntSize(dataLength) : 0)*/);

        if (buffer.HasData)
            throw new Exception("Packet wasn't readed to end");

        return (packetId, new(data));
    }

    public async Task WritePacketAsync(int packetId, MinecraftBuffer buffer)
    {
        byte[] packet = [.. IntToVarInt(packetId), .. buffer.ToArray()];
        var length = packet.Length;

        var compressionEnabled = CompressionThreshold > 0;
        var compressed = compressionEnabled && length > CompressionThreshold;

        if (compressed) 
        {
            var input = packet;
            var output = packet;

            var zlibCodec = new ZlibCodec
            {
                InputBuffer = input,
                NextIn = 0,
                OutputBuffer = output,
                NextOut = 0
            };

            var status = zlibCodec.InitializeDeflate();

            if (status != ZlibConstants.Z_OK)
                throw new IOException($"Cannot initialize zlib deflate: {status}");

            while (zlibCodec.TotalBytesOut < output.Length && zlibCodec.TotalBytesIn < input.Length)
            {
                zlibCodec.AvailableBytesIn = zlibCodec.AvailableBytesOut = 1; // force small buffers
                status = zlibCodec.Deflate(FlushType.None);

                if (status != ZlibConstants.Z_OK)
                    throw new IOException($"Cannot do zlib deflate: {status}");
            }

            status = zlibCodec.EndDeflate();

            if (status != ZlibConstants.Z_OK)
                throw new IOException($"Cannot end zlib deflate: {status}");

            var outputLength = (int)zlibCodec.TotalBytesOut;

            packet = output[..outputLength];
            // length = outputLength;
        }

        if (compressionEnabled)
        {
            var dataLength = compressed ? length : 0;
            await WriteVarIntAsync(GetVarIntSize(dataLength) + packet.Length);
            await WriteVarIntAsync(dataLength);

            Console.WriteLine($"compression enabled: {(compressed ? "compressed" : "uncompressed")} {GetVarIntSize(dataLength) + packet.Length} {dataLength} {packet.Length}");
        }
        else
        {
            await WriteVarIntAsync(length);
            Console.WriteLine($"compression disabled: {length} {packet.Length}");
        }

        await WriteAsync(packet);
    }

    public void EnableEncryption(byte[] secret)
    {
        var encryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        encryptCipher.Init(true, new ParametersWithIV(new KeyParameter(secret), secret, 0, 16));

        var decryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        decryptCipher.Init(false, new ParametersWithIV(new KeyParameter(secret), secret, 0, 16));
        
        source = new CipherStream(source, decryptCipher, encryptCipher);
    }

    protected byte[] IntToVarInt(int value)
    {
        var unsigned = (uint)value;
        var result = new List<byte>();

        do
        {
            var temp = (byte)(unsigned & 127);
            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            result.Add(temp);
        }
        while (unsigned != 0);

        return result.ToArray();
    }

    protected async Task<int> ReadVarIntAsync()
    {
        int numRead = 0;
        int result = 0;
        byte read;
        do
        {
            read = await ReadUnsignedByteAsync();
            int value = read & 0b01111111;
            result |= value << (7 * numRead);

            numRead++;
            if (numRead > 5)
            {
                throw new InvalidOperationException("VarInt is too big");
            }
        } while ((read & 0b10000000) != 0);

        return result;
    }

    protected async Task WriteVarIntAsync(int value)
    {
        var unsigned = (uint)value;

        do
        {
            var temp = (byte)(unsigned & 127);

            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            await WriteUnsignedByteAsync(temp);
        }
        while (unsigned != 0);
    }

    protected async Task<byte> ReadUnsignedByteAsync()
    {
        var buffer = new byte[1];
        await source.ReadAsync(buffer);
        return buffer[0];
    }

    protected async Task WriteUnsignedByteAsync(byte value)
    {
        await WriteAsync([value]);
    }

    protected int GetVarIntSize(int value)
    {
        return (BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171 >> 13;
    }

    protected async Task ReadAsync(byte[] buffer, CancellationToken cancellationToken = default)
    {
        // await source.ReadExactlyAsync(buffer, cancellationToken);
        await Task.Run(() => source.ReadExactly(buffer, 0, buffer.Length), cancellationToken);
    }

    protected async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default)
    {
        // await source.WriteAsync(buffer, cancellationToken);
        await Task.Run(() => source.Write(buffer, 0, buffer.Length), cancellationToken);
        await source.FlushAsync();
    }
}