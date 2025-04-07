using Void.Common;
using Void.Minecraft.Network;
using Void.Proxy.Api.Crypto;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Links.Extensions;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Services.Encryption;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Encryption;

public class EncryptionService(IEventService events, ICryptoService crypto) : AbstractEncryptionService(events, crypto)
{
    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }

    protected override async ValueTask SendEncryptionRequestAsync(ILink link, EncryptionRequest request, CancellationToken cancellationToken)
    {
        var encryptionRequest = new EncryptionRequestPacket
        {
            PublicKey = request.PublicKey,
            ServerId = request.ServerId,
            VerifyToken = request.VerifyToken
        };

        await link.SendPacketAsync(encryptionRequest, cancellationToken);
    }

    protected override async ValueTask<EncryptionResponse> ReceiveEncryptionResponseAsync(ILink link, CancellationToken cancellationToken)
    {
        var encryptionResponse = await link.ReceivePacketAsync<EncryptionResponsePacket>(cancellationToken);
        return new EncryptionResponse(encryptionResponse.SharedSecret, encryptionResponse.VerifyToken);
    }

    protected override bool IsEncrypionResponsePacket(INetworkMessage message, out byte[] sharedSecret)
    {
        if (message is EncryptionResponsePacket encryptionResponse)
        {
            sharedSecret = encryptionResponse.SharedSecret;
            return true;
        }

        sharedSecret = [];
        return false;
    }
}
