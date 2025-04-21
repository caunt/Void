using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Crypto;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Encryption;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Crypto;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Network.Streams.Encryption;
using Void.Proxy.Plugins.Common.Network.Streams.Packet;
using Void.Proxy.Plugins.Common.Network.Streams.Transparent;

namespace Void.Proxy.Plugins.Common.Services.Encryption;

public abstract class AbstractEncryptionService(IEventService events, ICryptoService crypto) : IPluginCommonService
{
    [Subscribe(PostOrder.First)]
    public async ValueTask OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Link.Player.TryGetMinecraftPlayer(out var player))
            return;

        if (!IsSupportedVersion(player.ProtocolVersion))
            return;

        if (@event.Origin is Side.Proxy)
            return;

        if (!IsEncryptionResponsePacket(@event.Message, out var sharedSecret))
            return;

        var privateKey = await events.ThrowWithResultAsync(new SearchServerPrivateKey(@event.Link.Server), cancellationToken);

        if (privateKey is null)
        {
            @event.Link.ServerChannel.Remove<MinecraftPacketMessageStream>();
            @event.Link.PlayerChannel.Remove<MinecraftPacketMessageStream>();

            @event.Link.ServerChannel.Add<TransparentMessageStream>();
            @event.Link.PlayerChannel.Add<TransparentMessageStream>();
        }
        else
        {
            var secret = Decrypt(privateKey, sharedSecret);

            @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, AesCfb8BufferedStream>(new AesCfb8BufferedStream(secret));
            @event.Link.ServerChannel.AddBefore<MinecraftPacketMessageStream, AesCfb8BufferedStream>(new AesCfb8BufferedStream(secret));
        }
    }

    [Subscribe]
    public async ValueTask OnPlayerVerifyingEncryption(PlayerVerifyingEncryptionEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Link.Player.TryGetMinecraftPlayer(out var player))
            return;

        if (!IsSupportedVersion(player.ProtocolVersion))
            return;

        var tokens = @event.Link.Player.Context.Services.GetRequiredService<ITokenHolder>();
        tokens.Store(TokenType.VerifyToken, BitConverter.GetBytes(Random.Shared.Next()));

        var request = new EncryptionRequest(crypto.Instance.ExportSubjectPublicKeyInfo(), tokens.Get(TokenType.VerifyToken).ToArray());
        await SendEncryptionRequestAsync(@event.Link, request, cancellationToken);

        var response = await ReceiveEncryptionResponseAsync(@event.Link, cancellationToken);

        if (!VerifyToken(@event.Link.Player, tokens.Get(TokenType.VerifyToken), response.VerifyToken, response.Salt))
            return; // invalid verify token

        var sharedSecret = new byte[response.SharedSecret.Length];

        if (!crypto.Instance.TryDecrypt(response.SharedSecret, sharedSecret, RSAEncryptionPadding.Pkcs1, out var length))
            return; // invalid shared secret

        @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, AesCfb8BufferedStream>(new AesCfb8BufferedStream(sharedSecret[..length]));
        @event.Result = true;
    }

    protected bool VerifyToken(IPlayer player, ReadOnlySpan<byte> original, ReadOnlySpan<byte> encrypted, long salt = 0)
    {
        if (!player.TryGetMinecraftPlayer(out var minecraftPlayer))
            return false;

        if (minecraftPlayer.IdentifiedKey is not null)
        {
            var saltBytes = BitConverter.GetBytes(salt);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(saltBytes);

            return minecraftPlayer.IdentifiedKey.VerifyDataSignature(encrypted, [.. original, .. saltBytes]);
        }

        var decrypted = new byte[encrypted.Length];

        if (!crypto.Instance.TryDecrypt(encrypted, decrypted, RSAEncryptionPadding.Pkcs1, out var length))
            return false;

        if (original.Length != length)
            return false;

        if (!original.SequenceEqual(decrypted.AsSpan(0, length)))
            return false;

        return true;
    }

    protected static byte[] Decrypt(byte[] key, byte[] data)
    {
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(key, out _);
        return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
    }

    protected abstract ValueTask SendEncryptionRequestAsync(ILink link, EncryptionRequest request, CancellationToken cancellationToken);

    protected abstract ValueTask<EncryptionResponse> ReceiveEncryptionResponseAsync(ILink link, CancellationToken cancellationToken);

    protected abstract bool IsEncryptionResponsePacket(INetworkMessage message, out byte[] sharedSecret);

    protected abstract bool IsSupportedVersion(ProtocolVersion version);
}
