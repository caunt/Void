using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Streams;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest;

public class ProtocolSupportPlugin(ILogger<ProtocolSupportPlugin> logger) : IPlugin
{
    public static readonly ProtocolVersion[] SupportedVersions = ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.Latest);
    public string Name => nameof(ProtocolSupportPlugin);

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    [Subscribe]
    public void OnProxyStarting(ProxyStartingEvent @event)
    {
    }

    [Subscribe]
    public void OnProxyStopping(ProxyStoppingEvent @event)
    {
    }

    [Subscribe]
    public void OnCreateChannelBuilder(CreateChannelBuilderEvent @event)
    {
        if (!IsSupportedHandshake(@event.Buffer))
            return;

        @event.Result = stream =>
        {
            var channel = new SimpleMinecraftChannel(new SimpleNetworkStream(stream));
            // channel.Add<MinecraftCodecMessageStream>();
            return Task.FromResult<IMinecraftChannel>(channel);
        };
    }

    public static bool IsSupportedHandshake(Memory<byte> memory)
    {
        var buffer = new MinecraftBuffer(memory.Span);
        var length = buffer.ReadVarInt();
        var packet = buffer.Read(length);

        buffer = new MinecraftBuffer(packet);
        var packetId = buffer.ReadVarInt();
        var protocolVersion = buffer.ReadVarInt();
        var serverAddress = buffer.ReadString(255);
        var serverPort = buffer.ReadUnsignedShort();
        var nextState = buffer.ReadVarInt();

        return packetId == 0 && SupportedVersions.Contains(ProtocolVersion.Get(protocolVersion)) && buffer.Position == buffer.Length;
    }
}