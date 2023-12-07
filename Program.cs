using BenchmarkDotNet.Attributes;
using MinecraftProxy;
using MinecraftProxy.Network.IO.Encryption;

Console.WriteLine("Hello, World!");

// BenchmarkRunner.Run<EncryptionStream>();
await Proxy.StartAsync();

[SimpleJob(launchCount: 5)]
public class EncryptionStream
{
    private readonly byte[] _secret;
    private readonly byte[] _memory;
    private readonly Stream _stream;

    public EncryptionStream()
    {
        _secret = new byte[16];
        _memory = new byte[16384]; // 16 KB

        Random.Shared.NextBytes(_secret);
        Random.Shared.NextBytes(_memory);

        _stream = new AesCfb8Stream(new MemoryStream(), _secret);
    }

    [Benchmark]
    public async ValueTask WriteAsync() => await _stream.WriteAsync(_memory);


    [IterationSetup(Target = nameof(ReadExactlyAsync))]
    public void FillStream()
    {
        _stream.WriteAsync(_memory).AsTask().Wait();
        _stream.Position = 0;
    }

    [Benchmark]
    public async ValueTask ReadExactlyAsync() => await _stream.ReadExactlyAsync(_memory);
}