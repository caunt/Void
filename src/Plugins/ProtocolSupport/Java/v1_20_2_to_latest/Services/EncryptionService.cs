using System.Security.Cryptography;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Network;
using Void.Proxy.Common.Network.IO.Streams.Encryption;
using Void.Proxy.Common.Network.IO.Streams.Packet;
using Void.Proxy.Common.Services;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Services;

public class EncryptionService : IPluginService
{
    private static readonly byte[] PrivateKey = Convert.FromHexString("30820277020100300d06092a864886f70d0101010500048202613082025d02010002818100e9675bb7efd2f2b75ee5f1ea07399fe38239a4196506fd7f6489961d6de770e7580c69e84fb040fe19721f06e7d8dc634280f0dbb71b79292f783eb2b6c409c5438197c3bd869b17264bb5aa448614fefccd3d841b278f05c202429d075deb2f5d1d2d24d80363f69444974358f23912951b9d3c2a4a86a0e5c13a84a997a04b02030100010281802e0fb94088023be72741bee79e0c67bae8d8c24346b645f1cd9fff71885e7be013f6c331d704241761632daf59b2e8ef67d0f5778edfcb9deea1ced1cb12ce1070f7d5226093bf9f2c7587a38239837849ed4728bbbba8da60b297b0397d6cafae8f427775aa8e22a42e04c7c8208da79a8f940ff2d8c31a22e723c56e136ce1024100ed595c08a640f5a71573ee45c79247bca08d4cb478d658aab90c5133b9bb0187b0e8801066adc6bb8f1a9903ccf548e7c746883443e96d3fde505116ee9466eb024100fbbea1f5dd1b2f63f867597a66fb26b85a0faea928525e5906635d3366e14ef4d02e851015e34ce78652e0a6a22e0cdfe92c6dbebc1da9daae2086d86e44142102401780268db0b073e2444c834623798762d4dec8be81cc6f61100b792acef40635c23d7318aca1fe3069fdef32a223934167c8c309b1c3b60e81db9ffbce49a15b024100a685c02e99567d2f9cc6086b2e299dc03e5ab7474fd3c4731105b345e81ccb94a70cce9a085075b384a7d7d081e102452ec163cad236b0ff654540cd738af6e1024100abd64d321c66d4154a4d3ff1dfb265609d3ec650610bad4f00e461357fadf9542058ecc8458f0acd2744020f96ef1c5f21ea8c82028d837ac2e4a61f2ce35689");

    [Subscribe]
    public void OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Message is not EncryptionResponsePacket encryptionResponse)
            return;

        var secret = Decrypt(PrivateKey, encryptionResponse.SharedSecret);

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