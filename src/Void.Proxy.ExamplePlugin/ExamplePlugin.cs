using Serilog;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Network.IO;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.ExamplePlugin;

public class ExamplePlugin : IPlugin
{
    public required ILogger Logger { get; init; }
    public string Name => nameof(ExamplePlugin);

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    [Subscribe]
    public void OnProxyStart(ProxyStart @event)
    {
        Logger.Information("Received ProxyStart event");
    }

    [Subscribe]
    public void OnProxyStop(ProxyStop @event)
    {
        Logger.Information("Received ProxyStop event");
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
