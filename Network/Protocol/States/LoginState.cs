using MinecraftProxy.Network.Protocol.Forwarding;
using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.Packets.Serverbound;

namespace MinecraftProxy.Network.Protocol.States;

public class LoginState(Player player) : ProtocolState, IPlayableState
{
    protected override Dictionary<int, Type> serverboundPackets => new()
    {
        { 0x00, typeof(LoginStartPacket) },
        { 0x01, typeof(EncryptionResponsePacket) },
        { 0x02, typeof(LoginPluginResponse) },
        { 0x03, typeof(LoginAcknowledgedPacket) }
    };

    protected override Dictionary<int, Type> clientboundPackets => new()
    {
        { 0x00, typeof(DisconnectPacket) },
        { 0x01, typeof(EncryptionRequestPacket) },
        { 0x02, typeof(LoginSuccessPacket) },
        { 0x03, typeof(SetCompressionPacket) },
        { 0x04, typeof(LoginPluginRequest) }
    };

    private byte[]? verifyToken;

    public async Task<bool> HandleAsync(LoginStartPacket packet)
    {
        player.SetGameProfile(new()
        {
            Id = packet.Guid,
            Name = packet.Username
        });

        var encryptionRequestPacket = GenerateEncryptionRequest();
        verifyToken = encryptionRequestPacket.VerifyToken;

        await player.SendPacketAsync(PacketDirection.Clientbound, encryptionRequestPacket);
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

        if (!Proxy.RSA.Decrypt(packet.VerifyToken, false).SequenceEqual(verifyToken))
            throw new Exception("Unable to verify encryption token");

        var secret = Proxy.RSA.Decrypt(packet.SharedSecret, false);
        player.EnableEncryption(secret);

        await player.RequestGameProfileAsync(secret);
        await player.SendPacketAsync(PacketDirection.Serverbound, GenerateLoginStartPacket());

        return true;
    }

    public Task<bool> HandleAsync(SetCompressionPacket packet)
    {
        if (packet.Threshold > 0)
            player.SetCompressionThreshold(PacketDirection.Serverbound, packet.Threshold);

        return Task.FromResult(true); // enable compression only with server
    }

    public Task<bool> HandleAsync(LoginSuccessPacket packet)
    {
        if (player.GameProfile is null)
            throw new Exception("Game profile not loaded yet");

        if (packet.Guid != player.GameProfile.Id)
            throw new Exception($"Server sent wrong player UUID: {packet.Guid}, online is: {player.GameProfile.Id}");

        return Task.FromResult(false);
    }

    public Task<bool> HandleAsync(LoginAcknowledgedPacket packet)
    {
        player.SwitchState(3);
        return Task.FromResult(false);
    }

    public async Task<bool> HandleAsync(LoginPluginRequest packet)
    {
        if (player.CurrentServer.Forwarding is not ModernForwarding forwarding)
            return false;

        if (!packet.Identifier.Equals("velocity:player_info"))
            return false;

        var data = forwarding.GenerateForwardingData(packet.Data, player);
        await player.SendPacketAsync(PacketDirection.Serverbound, new LoginPluginResponse
        {
            MessageId = packet.MessageId,
            Successful = true,
            Data = data
        });

        return true;
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
            Username = player.GameProfile.Name
        };
    }

    public Task<bool> HandleAsync(DisconnectPacket packet)
    {
        return Task.FromResult(false);
    }
}