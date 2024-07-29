using Void.Proxy.Models.General;
using Void.Proxy.Models.Minecraft.Encryption;
using Void.Proxy.Models.Minecraft.Profile;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.Forwarding;
using Void.Proxy.Network.Protocol.Packets.Clientbound;
using Void.Proxy.Network.Protocol.Packets.Serverbound;
using Void.Proxy.Network.Protocol.Registry;
using Void.Proxy.Network.Protocol.States.Custom;

namespace Void.Proxy.Network.Protocol.States.Common;

public class LoginState(Link link) : ProtocolState, ILoginConfigurePlayState
{
    private byte[]? verifyToken;
    protected override StateRegistry Registry { get; } = Registries.LoginStateRegistry;

    public Task<bool> HandleAsync(DisconnectPacket packet)
    {
        return Task.FromResult(false);
    }

    public async Task<bool> HandleAsync(LoginStartPacket packet)
    {
        link.Player.SetGameProfile(new GameProfile { Id = packet.Guid, Name = packet.Username });

        if (packet.IdentifiedKey is not null)
        {
            if (packet.IdentifiedKey.ExpiresAt < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                throw new Exception("multiplayer.disconnect.invalid_public_key_signature");

            var isKeyValid = packet.IdentifiedKey.Revision == IdentifiedKeyRevision.LINKED_V2 ? packet.IdentifiedKey.AddGuid(packet.Guid) : packet.IdentifiedKey.IsSignatureValid.HasValue && packet.IdentifiedKey.IsSignatureValid.Value;

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
                throw new Exception("Encryption response received but IdentifiedKey still not available for player");

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

        if (link.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_8 && Proxy.Settings.CompressionThreshold > 0)
        {
            var compressionPacket = new SetCompressionPacket { Threshold = Proxy.Settings.CompressionThreshold };
            await link.Player.SendPacketAsync(compressionPacket);
            link.Player.EnableCompression(Proxy.Settings.CompressionThreshold);
        }

        await link.Player.RequestGameProfileAsync(secret);

        var loginStartPacket = GenerateLoginStartPacket();
        link.SaveLoginStart(loginStartPacket);

        await link.Server.SendPacketAsync(loginStartPacket);

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

        if (link.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_20_2 && link.IsSwitching)
        {
            await link.ReplaceRedirectionClientChannel();
            return true;
        }

        return link.IsSwitching;
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

        // this whole block just for testing purposes
        if (packet.Identifier == "fml:loginwrapper")
        {
            (int, string, byte[]) Test()
            {
                var buffer = new MinecraftBuffer(packet.Data);
                var channel = buffer.ReadString();
                var length = buffer.ReadVarInt();
                var id = buffer.ReadVarInt();
                var data = buffer.ReadToEnd();

                return (id, channel, data.ToArray());
            }

            if (link.IsSwitching)
            {
                var (id, channel, data) = Test();

                switch (channel)
                {
                    case "fml:handshake":
                        switch (id)
                        {
                            case 1:

                                byte[] Reply(byte[] modlist)
                                {
                                    var input = new MinecraftBuffer(modlist);

                                    var names = new string[input.ReadVarInt()];
                                    for (var i = 0; i < names.Length; i++)
                                        names[i] = input.ReadString();

                                    var channels = new KeyValuePair<string, string>[input.ReadVarInt()];
                                    for (var i = 0; i < channels.Length; i++)
                                        channels[i] = new KeyValuePair<string, string>(input.ReadString(), input.ReadString());

                                    var registries = new string[input.ReadVarInt()];
                                    for (var i = 0; i < registries.Length; i++)
                                        registries[i] = input.ReadString();

                                    var output = new MinecraftBuffer(modlist.Length * 2);

                                    output.WriteVarInt(names.Length);
                                    foreach (var name in names)
                                        output.WriteString(name);

                                    output.WriteVarInt(channels.Length);
                                    foreach (var (name, marker) in channels)
                                    {
                                        output.WriteString(name);
                                        output.WriteString(marker);
                                    }

                                    output.WriteVarInt(registries.Length);
                                    foreach (var name in registries)
                                    {
                                        output.WriteString(name);
                                        output.WriteString("1");
                                    }

                                    var packetData = output.Span[..output.Position].ToArray();
                                    var buffer = new MinecraftBuffer(packet.Data.Length * 2);
                                    buffer.WriteString(channel!);
                                    buffer.WriteVarInt(MinecraftBuffer.GetVarIntSize(2) + packetData.Length);
                                    buffer.WriteVarInt(2);
                                    buffer.Write(packetData);

                                    return buffer.Span[..buffer.Position].ToArray();
                                }

                                await link.Server.SendPacketAsync(new LoginPluginResponse
                                {
                                    Data = Reply(data),
                                    MessageId = packet.MessageId,
                                    Successful = true
                                });
                                break;
                            case 3 or 4:
                                await link.Server.SendPacketAsync(new LoginPluginResponse
                                {
                                    Data = Convert.FromHexString("0D666D6C3A68616E647368616B650163"),
                                    MessageId = packet.MessageId,
                                    Successful = true
                                });
                                break;
                            case 5:
                                break;
                            default:
                                Proxy.Logger.Debug($"2 Don't know what to do with forge channel {channel} packet id {id}, answering as succesful");
                                await link.Server.SendPacketAsync(new LoginPluginResponse
                                {
                                    Data = [],
                                    MessageId = packet.MessageId,
                                    Successful = true
                                });
                                break;
                        }

                        break;
                    case "silentgear:network":
                        break;
                    case "exnihilosequentia:handshake":
                        await link.Server.SendPacketAsync(new LoginPluginResponse
                        {
                            Data = Convert.FromHexString("1B65786E6968696C6F73657175656E7469613A68616E647368616B650163"),
                            MessageId = packet.MessageId,
                            Successful = true
                        });
                        break;
                    default:
                        Proxy.Logger.Debug($"1 Don't know what to do with forge channel {channel} packet id {id}, answering as succesful");
                        await link.Server.SendPacketAsync(new LoginPluginResponse
                        {
                            Data = [],
                            MessageId = packet.MessageId,
                            Successful = true
                        });
                        break;
                }
            }

            Proxy.Logger.Debug($"Received Clientbound Login plugin request {packet.MessageId} on plugin channel {packet.Identifier} with {packet.Data.Length} bytes");
            return link.IsSwitching;
        }

        return link.IsSwitching;
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

        return new EncryptionRequestPacket { PublicKey = Proxy.RSA.ExportSubjectPublicKeyInfo(), VerifyToken = verify };
    }

    public LoginStartPacket GenerateLoginStartPacket()
    {
        if (link.Player.GameProfile is null)
            throw new Exception("Can't proceed login as we do not have online GameProfile");

        return new LoginStartPacket
        {
            Guid = link.Player.GameProfile.Id,
            Username = link.Player.GameProfile.Name,
            IdentifiedKey = link.Player.IdentifiedKey
        };
    }
}