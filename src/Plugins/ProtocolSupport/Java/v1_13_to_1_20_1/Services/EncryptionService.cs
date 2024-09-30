using System.Security.Cryptography;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Network;
using Void.Proxy.Common.Network.IO.Streams.Encryption;
using Void.Proxy.Common.Network.IO.Streams.Packet;
using Void.Proxy.Common.Services;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Services;

public class EncryptionService : IPluginService
{
    private static readonly byte[] _privateKey = Convert.FromHexString("30820275020100300d06092a864886f70d01010105000482025f3082025b020100028181008833058bd694928c004a7ffbaeca35f5bac94b070082080958c71bc8bc23a507c64432a8d38cb3d405928ef58f89f681c87a3909f3a1ec08cbdc5223efb612efcdeb639fd93562ac73e407c6bd8d68d7dc1a226e6c95c9f3e5a94edaec76a98c411caf6caec3f2618ea33169b72e42687faf4ac7478af722df6c234b5fcc82ef0203010001028180119dc27a8dd74b052a07237aa2e700b261e5d0a5d288ff0fc66dd51d7cf2d750f97204b8c01ebe26644f75323478a71658f486157ac5021f392456c8d323d25b0583aa927c9bf835017fe3af24da1c4dd6ab416c06bbedfef40ef4428adc60d4e2d5e613d3278ca87590fc91ff7d43ad359cf16215af0391e53eef1581882dc1024100a99aca2367811e10f8e15ee5472d50507756bb48c5be1cc5cb6251b43dcec7163f52913766722968d1843daf99687026010db8986ad1c58005c238eec2ca2bff024100cd940653037a7ff8e8e4aa3d1d6d543a12297538dac4c4def496fd96d030dd0c0daa32da80b14e1344a8571e140ce0d27e009cbe0a7e9124a0a90a2e0bac691102401ddc048e6b208e3c8ab492d266cf917e392469e08bffc66d043b910adc7ed50a13a7e3ad0f3a3614201eda055a4acac3c617b6520f2c534b10b87af17e15bddd023f3c3a21a03064b3193921c4be22e0e4cc1e8606d1a14604674d40ef0a3ff410ce773265b39e0053df513e0047cf97f645b4a4794733cbe0b9da57aba3d1c7b10241009905827e05808b57c0c836d29b1efe9852f502509b54ab6246539d359cbb036904283b6f6f5636659f7b25478949f0acc3a7cf4edf13cd2539376247845deff1");

    [Subscribe]
    public void OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Message is not EncryptionResponsePacket encryptionResponse)
            return;

        var secret = Decrypt(_privateKey, encryptionResponse.SharedSecret);

        @event.Link.ServerChannel.AddBefore<MinecraftPacketMessageStream, AesCfb8BufferedStream>(new AesCfb8BufferedStream(secret));
        @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, AesCfb8BufferedStream>(new AesCfb8BufferedStream(secret));
    }

    private static byte[] Decrypt(byte[] key, byte[] data)
    {
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(key, out _);
        return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
    }
}