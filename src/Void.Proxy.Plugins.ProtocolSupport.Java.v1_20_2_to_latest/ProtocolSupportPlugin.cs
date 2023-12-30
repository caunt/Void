using Serilog;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Network.IO;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest;

public class ProtocolSupportPlugin : IPlugin
{
    public required ILogger Logger { get; init; }
    public string Name => nameof(ProtocolSupportPlugin);

    public readonly ProtocolVersion OldestVersion = ProtocolVersion.MINECRAFT_1_20_2;
    public readonly ProtocolVersion NewestVersion = ProtocolVersion.Latest;

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    [Subscribe]
    public void OnProxyStart(ProxyStart @event)
    {
    }

    [Subscribe]
    public void OnProxyStop(ProxyStop @event)
    {
    }

    [Subscribe]
    public void OnSearchProtocolCodec(SearchClientProtocolCodec @event)
    {
        bool IsHandshake(Memory<byte> memory)
        {
            var buffer = new MinecraftBuffer(memory.Span);

            try
            {
                var length = buffer.ReadVarInt();
                var packet = buffer.Read(length);

                buffer = new(packet);
                var protocolVersion = buffer.ReadVarInt();
                var serverAddress = buffer.ReadString(255);
                var serverPort = buffer.ReadUnsignedShort();
                var nextState = buffer.ReadVarInt();

                if (buffer.Position < buffer.Length)
                    throw new NotSupportedException();

                return true;
            }
            catch (Exception exception)
            {
                Logger.Information("Handshake cannot be decoded: " + Convert.ToHexString(memory.Span) + "\n" + exception);
            }

            return false;
        }

        IsHandshake(@event.Buffer);
    }
}
