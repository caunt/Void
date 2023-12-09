using MinecraftProxy.Models;
using MinecraftProxy.Network.Protocol.Forwarding;
using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.Packets.Serverbound;
using MinecraftProxy.Network.Protocol.Registry;
using MinecraftProxy.Network.Protocol.States.Custom;

namespace MinecraftProxy.Network.Protocol.States.Common;

public class LoginState(Player player, Server? server) : ProtocolState, ILoginConfigurePlayState
{
    protected override StateRegistry Registry { get; } = Registries.LoginStateRegistry;

    private byte[]? verifyToken;

    public async Task<bool> HandleAsync(LoginStartPacket packet)
    {
        player.SetGameProfile(new()
        {
            Id = packet.Guid,
            Name = packet.Username
        });

        if (packet.IdentifiedKey is not null)
        {
            if (packet.IdentifiedKey.ExpiresAt < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                throw new Exception("multiplayer.disconnect.invalid_public_key_signature");

            var isKeyValid = packet.IdentifiedKey.Revision == IdentifiedKeyRevision.LINKED_V2 ?
                packet.IdentifiedKey.AddGuid(packet.Guid) :
                packet.IdentifiedKey.IsSignatureValid.HasValue && packet.IdentifiedKey.IsSignatureValid.Value;

            if (!isKeyValid)
                throw new Exception("multiplayer.disconnect.invalid_public_key");

            player.SetIdentifiedKey(packet.IdentifiedKey);
        }

        var encryptionRequestPacket = GenerateEncryptionRequest();
        verifyToken = encryptionRequestPacket.VerifyToken;

        await player.SendPacketAsync(encryptionRequestPacket);
        return true; // cancelling as we will send login packet to server later with online game profile
    }

    public Task<bool> HandleAsync(EncryptionRequestPacket packet)
    {
        throw new Exception("Backend server is in online-mode");
    }

    public async Task<bool> HandleAsync(EncryptionResponsePacket packet)
    {
        if (verifyToken is null)
            throw new Exception("Encryption verify token is not set yet");

        if (packet.Salt.HasValue)
        {
            if (player.IdentifiedKey is null)
                throw new Exception($"Encryption response received but IdentifiedKey still not available for player");

            var salt = BitConverter.GetBytes(packet.Salt.Value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(salt);

            if (!player.IdentifiedKey.VerifyDataSignature(packet.VerifyToken, [.. verifyToken, .. salt]))
                throw new Exception("Invalid client public signature");
        }
        else
        {
            if (!Proxy.RSA.Decrypt(packet.VerifyToken, false).SequenceEqual(verifyToken))
                throw new Exception("Unable to verify encryption token");
        }

        ArgumentNullException.ThrowIfNull(server);

        var secret = Proxy.RSA.Decrypt(packet.SharedSecret, false);
        player.EnableEncryption(secret);

        if (player.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_8)
        {
            var compressionPacket = new SetCompressionPacket { Threshold = Proxy.CompressionThreshold };
            await player.SendPacketAsync(compressionPacket);
            player.EnableCompression(Proxy.CompressionThreshold);
        }

        await player.RequestGameProfileAsync(secret);
        await server.SendPacketAsync(player, GenerateLoginStartPacket());

        return true;
    }

    public async Task<bool> HandleAsync(SetCompressionPacket packet)
    {
        ArgumentNullException.ThrowIfNull(server);

        if (packet.Threshold > 0)
            server.EnableCompression(packet.Threshold);

        return true; // we should complete encryption before sending compression packet
    }

    public async Task<bool> HandleAsync(LoginSuccessPacket packet)
    {
        ArgumentNullException.ThrowIfNull(server);

        if (player.GameProfile is null)
            throw new Exception("Game profile not loaded yet");

        if (server is null)
            throw new Exception("Server not chosen yet");

        if (server.Forwarding is not NoneForwarding)
        {
            if (packet.Guid != player.GameProfile.Id)
                throw new Exception($"Server sent wrong player UUID: {packet.Guid}, online is: {player.GameProfile.Id}");
        }
        else
        {
            // fallback to offline GameProfile
            player.GameProfile.Id = packet.Guid;
            player.GameProfile.Name = packet.Username;
            player.GameProfile.Properties = packet.Properties;
        }

        if (player.ProtocolVersion < ProtocolVersion.MINECRAFT_1_20_2)
            player.SwitchState(4);

        return false;
    }

    public Task<bool> HandleAsync(LoginAcknowledgedPacket packet)
    {
        player.SwitchState(3);
        return Task.FromResult(false);
    }

    public async Task<bool> HandleAsync(LoginPluginRequest packet)
    {
        ArgumentNullException.ThrowIfNull(server);

        if (server is null)
            throw new Exception("Server not chosen yet");

        if (server.Forwarding is ModernForwarding forwarding)
        {
            if (!packet.Identifier.Equals("velocity:player_info"))
                return false;

            var data = forwarding.GenerateForwardingData(packet.Data, player);
            await server.SendPacketAsync(player, new LoginPluginResponse
            {
                MessageId = packet.MessageId,
                Successful = true,
                Data = data
            });

            return true;
        }

        return false;
    }

    public Task<bool> HandleAsync(LoginPluginResponse packet)
    {
        return Task.FromResult(false);
    }

    public EncryptionRequestPacket GenerateEncryptionRequest()
    {
        var verify = new byte[4];
        Random.Shared.NextBytes(verify);

        return new()
        {
            PublicKey = Proxy.RSA.ExportSubjectPublicKeyInfo(),
            VerifyToken = verify
        };
    }

    public LoginStartPacket GenerateLoginStartPacket()
    {
        if (player.GameProfile is null)
            throw new Exception("Can't proceed login as we do not have online GameProfile");

        return new()
        {
            Guid = player.GameProfile.Id,
            Username = player.GameProfile.Name,
            IdentifiedKey = player.IdentifiedKey
        };
    }

    public Task<bool> HandleAsync(DisconnectPacket packet)
    {
        return Task.FromResult(false);
    }
}