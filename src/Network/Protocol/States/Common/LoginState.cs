using MinecraftProxy.Models.General;
using MinecraftProxy.Models.Minecraft.Encryption;
using MinecraftProxy.Network.Protocol.Forwarding;
using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.Packets.Serverbound;
using MinecraftProxy.Network.Protocol.Registry;
using MinecraftProxy.Network.Protocol.States.Custom;

namespace MinecraftProxy.Network.Protocol.States.Common;

public class LoginState(Link link) : ProtocolState, ILoginConfigurePlayState
{
    protected override StateRegistry Registry { get; } = Registries.LoginStateRegistry;

    private byte[]? verifyToken;

    public async Task<bool> HandleAsync(LoginStartPacket packet)
    {
        link.Player.SetGameProfile(new()
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

            link.Player.SetIdentifiedKey(packet.IdentifiedKey);
        }

        var encryptionRequestPacket = GenerateEncryptionRequest();
        verifyToken = encryptionRequestPacket.VerifyToken;

        await link.Player.SendPacketAsync(encryptionRequestPacket);
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
            if (link.Player.IdentifiedKey is null)
                throw new Exception($"Encryption response received but IdentifiedKey still not available for player");

            var salt = BitConverter.GetBytes(packet.Salt.Value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(salt);

            if (!link.Player.IdentifiedKey.VerifyDataSignature(packet.VerifyToken, [.. verifyToken, .. salt]))
                throw new Exception("Invalid client public signature");
        }
        else
        {
            if (!Proxy.RSA.Decrypt(packet.VerifyToken, false).SequenceEqual(verifyToken))
                throw new Exception("Unable to verify encryption token");
        }

        var secret = Proxy.RSA.Decrypt(packet.SharedSecret, false);
        link.Player.EnableEncryption(secret);

        if (link.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_8 && Proxy.CompressionThreshold > 0)
        {
            var compressionPacket = new SetCompressionPacket { Threshold = Proxy.CompressionThreshold };
            await link.Player.SendPacketAsync(compressionPacket);
            link.Player.EnableCompression(Proxy.CompressionThreshold);
        }

        await link.Player.RequestGameProfileAsync(secret);
        await link.Server.SendPacketAsync(GenerateLoginStartPacket());

        return true;
    }

    public async Task<bool> HandleAsync(SetCompressionPacket packet)
    {
        if (packet.Threshold > 0)
            link.Server.EnableCompression(packet.Threshold);

        // we should complete encryption before sending compression packet
        return true;
    }

    public async Task<bool> HandleAsync(LoginSuccessPacket packet)
    {
        if (link.Player.GameProfile is null)
            throw new Exception("Game profile not loaded yet");

        if (link.ServerInfo.Forwarding is not NoneForwarding)
        {
            if (packet.Guid != link.Player.GameProfile.Id)
                throw new Exception($"Server sent wrong player UUID: {packet.Guid}, online is: {link.Player.GameProfile.Id}");
        }
        else
        {
            // fallback to offline GameProfile
            link.Player.GameProfile.Id = packet.Guid;
            link.Player.GameProfile.Name = packet.Username;
            link.Player.GameProfile.Properties = packet.Properties;
        }

        if (link.ProtocolVersion < ProtocolVersion.MINECRAFT_1_20_2)
            link.SwitchState(4);

        return false;
    }

    public Task<bool> HandleAsync(LoginAcknowledgedPacket packet)
    {
        link.SwitchState(3);
        return Task.FromResult(false);
    }

    public async Task<bool> HandleAsync(LoginPluginRequest packet)
    {
        if (link.ServerInfo.Forwarding is ModernForwarding forwarding)
        {
            if (!packet.Identifier.Equals("velocity:player_info"))
                return false;

            var data = forwarding.GenerateForwardingData(packet.Data, link.Player);
            await link.Server.SendPacketAsync(new LoginPluginResponse
            {
                MessageId = packet.MessageId,
                Successful = true,
                Data = data
            });

            return true;
        }

        if (packet.Identifier == "fml:loginwrapper")
        {
            Proxy.Logger.Debug($"Received Clientbound Login plugin request {packet.Identifier} with {packet.Data.Length} bytes");
            return false;
        }

        return false;
    }

    public Task<bool> HandleAsync(LoginPluginResponse packet)
    {
        Proxy.Logger.Debug($"Received Clientbound Login plugin response {packet.MessageId} with {packet.Data.Length} bytes");
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
        if (link.Player.GameProfile is null)
            throw new Exception("Can't proceed login as we do not have online GameProfile");

        return new()
        {
            Guid = link.Player.GameProfile.Id,
            Username = link.Player.GameProfile.Name,
            IdentifiedKey = link.Player.IdentifiedKey
        };
    }

    public Task<bool> HandleAsync(DisconnectPacket packet)
    {
        return Task.FromResult(false);
    }
}