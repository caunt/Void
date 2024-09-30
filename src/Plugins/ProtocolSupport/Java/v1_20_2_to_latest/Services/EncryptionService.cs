using System.Security.Cryptography;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Encryption;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Services;
using Void.Proxy.Common.Network.IO.Streams.Encryption;
using Void.Proxy.Common.Network.IO.Streams.Packet;
using Void.Proxy.Common.Network.IO.Streams.Transparent;
using Void.Proxy.Common.Services;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Services;

public class EncryptionService(IEventService events) : IPluginService
{
    [Subscribe(PostOrder.First)]
    public async ValueTask OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Message is not EncryptionResponsePacket encryptionResponse)
            return;

        var privateKey = await events.ThrowWithResultAsync(new SearchServerPrivateKey { Server = @event.Link.Server }, cancellationToken);

        if (privateKey is null)
        {
            @event.Link.ServerChannel.Remove<MinecraftPacketMessageStream>();
            @event.Link.PlayerChannel.Remove<MinecraftPacketMessageStream>();

            @event.Link.ServerChannel.Add<MinecraftTransparentMessageStream>();
            @event.Link.PlayerChannel.Add<MinecraftTransparentMessageStream>();
        }
        else
        {
            var secret = Decrypt(privateKey, encryptionResponse.SharedSecret);

            @event.Link.ServerChannel.AddBefore<MinecraftPacketMessageStream, AesCfb8BufferedStream>(new AesCfb8BufferedStream(secret));
            @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, AesCfb8BufferedStream>(new AesCfb8BufferedStream(secret));
        }
    }

    private static byte[] Decrypt(byte[] key, byte[] data)
    {
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(key, out _);
        return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
    }
}