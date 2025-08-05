using BenchmarkDotNet.Attributes;
using Microsoft.IO;
using Void.Proxy.Plugins.Common.Network.Messages.Binary;
using Void.Proxy.Plugins.Common.Network.Streams.Compression;

namespace Void.Benchmarks.Streams;

public class Compression
{
    private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new(new RecyclableMemoryStreamManager.Options
    {
        BlockSize = 1024,
        LargeBufferMultiple = 1024 * 1024,
        MaximumBufferSize = 16 * 1024 * 1024,
        GenerateCallStacks = false,
        AggressiveBufferReturn = true,
        MaximumLargePoolFreeBytes = 16 * 1024 * 1024,
        MaximumSmallPoolFreeBytes = 100 * 1024
    });

    private readonly IonicZlibCompressionMessageStream _ionicZlibStream = new() { BaseStream = new MinecraftMemoryStream(), CompressionThreshold = 256 };
    private readonly SharpZipLibCompressionMessageStream _sharpZipLibStream = new() { BaseStream = new MinecraftMemoryStream(), CompressionThreshold = 256 };
    private byte[] _buffer = [];
    private const int IterationCount = 1000;

    [Params(32, 1024)] public int BufferSize { get; set; }

    /*
     * BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4112/23H2/2023Update/SunValley3)
     * 12th Gen Intel Core i9-12900KF, 1 CPU, 24 logical and 16 physical cores
     * .NET SDK 9.0.100-preview.7.24407.12
     *   [Host]     : .NET 9.0.0 (9.0.24.40507), X64 RyuJIT AVX2 [AttachedDebugger]
     *   DefaultJob : .NET 9.0.0 (9.0.24.40507), X64 RyuJIT AVX2
     *
     *
     * 1KB buffer (with compression)
     *
     * | Method            | BufferSize | Mean        | Error       | StdDev    |
     * |------------------ |----------- |------------:|------------:|----------:|
     * | SharpZipLib_Write | 1024       | 25,013.6 us |   384.20 us | 359.38 us |
     * | SharpZipLib_Read  | 1024       |  1,229.1 us |    24.51 us |  46.04 us |
     * | IonicZlib_Write   | 1024       | 80,828.7 us | 1,184.75 us | 989.32 us |
     * | IonicZlib_Read    | 1024       |  5,429.7 us |   108.03 us | 280.79 us |
     *
     * 32B buffer (without compression)
     *
     * | Method            | BufferSize | Mean        | Error       | StdDev    |
     * |------------------ |----------- |------------:|------------:|----------:|
     * | SharpZipLib_Write | 32         |  1,081.5 us |    40.94 us | 120.06 us |
     * | SharpZipLib_Read  | 32         |    947.2 us |    27.13 us |  79.58 us |
     * | IonicZlib_Write   | 32         |  1,022.6 us |    20.42 us |  35.23 us |
     * | IonicZlib_Read    | 32         |    928.0 us |    21.75 us |  62.05 us |
     */

    [GlobalSetup]
    public void GlobalSetup()
    {
        _buffer = new byte[BufferSize];
        Random.Shared.NextBytes(_buffer);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        var sharpZipLibMemoryStream = (MinecraftMemoryStream)(_sharpZipLibStream.BaseStream ?? throw new InvalidOperationException($"{nameof(_sharpZipLibStream.BaseStream)} is null."));
        var ionicZlibMemoryStream = (MinecraftMemoryStream)(_ionicZlibStream.BaseStream ?? throw new InvalidOperationException("Base stream is null."));

        sharpZipLibMemoryStream.Reset(0);
        ionicZlibMemoryStream.Reset(0);

        SharpZipLib_Write().AsTask().GetAwaiter().GetResult();
        IonicZlib_Write().AsTask().GetAwaiter().GetResult();

        sharpZipLibMemoryStream.Reset();
        ionicZlibMemoryStream.Reset();
    }

    [Benchmark]
    public async ValueTask SharpZipLib_Write()
    {
        for (var i = 0; i < IterationCount; i++)
        {
            var stream = MemoryStreamManager.GetStream();
            stream.Write(_buffer);

            var message = new CompleteBinaryMessage(stream);
            await _sharpZipLibStream.WriteMessageAsync(message);
        }
    }

    [Benchmark]
    public async ValueTask SharpZipLib_Read()
    {
        var sharpZipLibMemoryStream = (MinecraftMemoryStream)(_sharpZipLibStream.BaseStream ?? throw new InvalidOperationException("Base stream is null."));
        sharpZipLibMemoryStream.Reset();

        for (var i = 0; i < IterationCount; i++)
        {
            _ = await _sharpZipLibStream.ReadMessageAsync();
        }
    }

    [Benchmark]
    public async ValueTask IonicZlib_Write()
    {
        for (var i = 0; i < IterationCount; i++)
        {
            var stream = MemoryStreamManager.GetStream();
            stream.Write(_buffer);

            var message = new CompleteBinaryMessage(stream);
            await _ionicZlibStream.WriteMessageAsync(message);
        }
    }

    [Benchmark]
    public async ValueTask IonicZlib_Read()
    {
        var ionicZlibMemoryStream = (MinecraftMemoryStream)(_ionicZlibStream.BaseStream ?? throw new InvalidOperationException("Base stream is null."));
        ionicZlibMemoryStream.Reset();

        for (var i = 0; i < IterationCount; i++)
        {
            _ = await _ionicZlibStream.ReadMessageAsync();
        }
    }
}
