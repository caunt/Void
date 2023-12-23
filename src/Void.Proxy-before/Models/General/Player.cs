using Minecraft.Component.Component;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using Void.Proxy.Models.Minecraft.Chat;
using Void.Proxy.Models.Minecraft.Encryption;
using Void.Proxy.Models.Minecraft.Profile;
using Void.Proxy.Network;
using Void.Proxy.Network.Protocol;
using Void.Proxy.Network.Protocol.Packets;
using Void.Proxy.Network.Protocol.Packets.Clientbound;
using Void.Proxy.Network.Protocol.Packets.Shared;

namespace Void.Proxy.Models.General;

public class Player(Link link)
{
    public Link Link { get; } = link;
    public string? Brand { get; protected set; }
    public ClientType ClientType { get; protected set; }
    public GameProfile? GameProfile { get; protected set; }
    public IdentifiedKey? IdentifiedKey { get; protected set; }

    public void SetBrand(string brand)
    {
        Brand = brand;
    }

    public void SetClientType(ClientType clientType)
    {
        ClientType = clientType;
    }

    public void SetGameProfile(GameProfile gameProfile)
    {
        GameProfile = gameProfile;
    }

    public void SetIdentifiedKey(IdentifiedKey identifiedKey)
    {
        IdentifiedKey = identifiedKey;
    }

    public void EnableEncryption(byte[] secret)
    {
        Link.PlayerChannel.EnableEncryption(secret);
        Proxy.Logger.Information($"Player {this} enabled encryption");
    }

    public void EnableCompression(int threshold)
    {
        Link.PlayerChannel.EnableCompression(threshold);
        Proxy.Logger.Information($"Player {this} enabled compression");
    }

    public async Task SendPacketAsync(IMinecraftPacket packet) => await Link.SendPacketAsync(Direction.Clientbound, packet);

    public async Task SendMessageAsync(string text) => await SendMessageAsync(ChatComponent.FromLegacy(text));

    public async Task SendMessageAsync(ChatComponent component)
    {
        IMinecraftPacket packet;

        if (Link.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_19)
            packet = new SystemChatMessage { Type = ChatMessageType.System, Message = component };
        else
            packet = new ChatMessage(Direction.Clientbound) { Type = ChatMessageType.System, Message = component.ToString() };

        await SendPacketAsync(packet);
    }

    public async Task<GameProfile?> RequestGameProfileAsync(byte[] secret)
    {
        if (GameProfile is null)
            throw new Exception("Can't request online GameProfile without any GameProfile received before (usually sent by player in login packet)");

        static byte[] TwosComplement(byte[] data)
        {
            int i;
            bool carry = true;

            for (i = data.Length - 1; i >= 0; i--)
            {
                data[i] = unchecked((byte)~data[i]);
                if (carry)
                {
                    carry = data[i] == 0xFF;
                    data[i]++;
                }
            }

            return data;
        }

        var serverId = SHA1.HashData([.. secret, .. Proxy.RSA.ExportSubjectPublicKeyInfo()]);

        var negative = (serverId[0] & 0x80) == 0x80;
        if (negative)
            serverId = TwosComplement(serverId);

        var serverIdComplement = Convert.ToHexString(serverId).TrimStart('0');

        if (negative)
            serverIdComplement = "-" + serverIdComplement;

        var url = $"{Environment.GetEnvironmentVariable("mojang.sessionserver") ?? "https://sessionserver.mojang.com/session/minecraft/hasJoined"}?username={GameProfile.Name}&serverId={serverIdComplement}";

        var playerIp = Link.PlayerRemoteEndPoint switch { IPEndPoint ipEndPoint => ipEndPoint.Address.ToString(), _ => null };
        var preventProxyConnections = false;

        if (preventProxyConnections && playerIp is not null)
            url += "&ip=" + playerIp;

        var response = await Proxy.HttpClient.GetAsync(url);

        if (response.StatusCode is HttpStatusCode.NoContent)
            throw new Exception("Offline user connected to online-mode proxy");

        GameProfile = JsonSerializer.Deserialize<GameProfile>(await response.Content.ReadAsStreamAsync(), Proxy.JsonSerializerOptions);

        if (GameProfile != null && IdentifiedKey != null && IdentifiedKey.Revision == IdentifiedKeyRevision.LINKED_V2)
        {
            if (!IdentifiedKey.AddGuid(GameProfile.Id))
                throw new Exception("multiplayer.disconnect.invalid_public_key");
        }

        return GameProfile;
    }

    public override string ToString() => GameProfile?.Name ?? Link.PlayerRemoteEndPoint?.ToString() ?? "Disposed?";
}
