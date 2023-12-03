using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System.Numerics;
using System.Security.Cryptography;

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

        if (CompressionThreshold > 0)
        {
            var compressedDataLength = await ReadVarIntAsync();

            if (compressedDataLength > 0)
                throw new NotImplementedException();
            /*{
                var compressedBuffer = new byte[dataLength];
                await ReadAsync(compressedBuffer);

                using var deflateStream = new DeflateStream(new MemoryStream(compressedBuffer), CompressionMode.Decompress);
                var decompressedLength = deflateStream.Read(compressedBuffer);

                var minecraftBuffer = new MinecraftBuffer(compressedBuffer);
                var decompressedPacketId = minecraftBuffer.ReadVarInt();
                var decompressedData = new byte[length - GetVarIntSize(decompressedPacketId)];
                minecraftBuffer.Read(decompressedData);

                return (decompressedPacketId, new(decompressedData));
            }*/
        }

        var packetId = await ReadVarIntAsync();

        var data = new byte[length - GetVarIntSize(packetId)];
        await ReadAsync(data);

        return (packetId, new(data));
    }

    public async Task WritePacketAsync(int packetId, MinecraftBuffer buffer)
    {
        await WriteVarIntAsync((int)buffer.Length + GetVarIntSize(packetId));
        await WriteVarIntAsync(packetId);
        await WriteAsync(buffer.ToArray());
    }

    public void EnableEncryption(byte[] secret)
    {
        var encryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        encryptCipher.Init(true, new ParametersWithIV(new KeyParameter(secret), secret, 0, 16));

        var decryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        decryptCipher.Init(false, new ParametersWithIV(new KeyParameter(secret), secret, 0, 16));

        source = new CipherStream(source, decryptCipher, encryptCipher);
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
    }
}