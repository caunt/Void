using System.Net;
using System.Security.Cryptography;
using System.Text;
using Void.Proxy.Models.General;
using Void.Proxy.Network.IO;

namespace Void.Proxy.Network.Protocol.Forwarding;

public class ModernForwarding(string secret) : IForwarding
{
    public ForwardingMode Mode => ForwardingMode.Modern;
    public string Secret { get; set; } = secret;

    public byte[] GenerateForwardingData(byte[] data, Player player)
    {
        if (player.GameProfile is null)
            throw new Exception($"Player {player} does not have data to forward");

        var requestedVersion = data.Length == 0 ? Version.Default : (Version)data[0];
        var actualVersion = FindForwardingVersion(requestedVersion, player);
        var buffer = new MinecraftBuffer();

        buffer.WriteVarInt((int)actualVersion);

        var address = player.Link.PlayerRemoteEndPoint is IPEndPoint ipEndPoint ? ipEndPoint.Address.ToString() : throw new NotImplementedException($"Cannot forward player {player} address {player.Link.PlayerRemoteEndPoint}");
        buffer.WriteString(address);

        buffer.WriteGuid(player.GameProfile.Id);
        buffer.WriteString(player.GameProfile.Name);
        buffer.WritePropertyList(player.GameProfile.Properties);

        if (actualVersion.CompareTo(Version.WithKey) >= 0 && actualVersion.CompareTo(Version.LazySession) < 0)
        {
            if (player.IdentifiedKey is null)
                throw new Exception($"Player {player} identified key cannot be forwarded");

            buffer.WriteIdentifiedKey(player.IdentifiedKey);

            if (actualVersion.CompareTo(Version.WithKeyV2) >= 0)
            {
                // if key signature holder is not null (seems to be always null)
                // WriteBoolean(true)
                // WriteGuid(key.GetSignatureHolder())
                // else
                buffer.WriteBoolean(false);
            }
        }

        var forwardingData = buffer.Span[..buffer.Position];
        var signature = HMACSHA256.HashData(Encoding.UTF8.GetBytes(Secret), forwardingData);

        return [.. signature, .. forwardingData];
    }

    // TODO: https://github.com/PaperMC/Velocity/blob/07a525be7f90f1f3ccd515f7c196824d12ed0fff/proxy/src/main/java/com/velocitypowered/proxy/connection/backend/LoginSessionHandler.java#L199
    private Version FindForwardingVersion(Version requested, Player player)
    {
        requested = (Version)Math.Min((int)requested, Enum.GetValues(typeof(Version)).Cast<int>().Max());

        if (requested.CompareTo(Version.Default) > 0)
        {
            // if protocol version > 1.19.3
            // return requested.CompareTo(Version.LazySession) >= 0 ? Version.LazySession : Versions.Default

            if (player.IdentifiedKey is not null)
            {
                // if key revision is Generic (protocol version 1.19)
                // return Versions.WithKey
                // else if key revision is Linked (protocol version every other)
                // return requested.CompareTo(Versions.WithKeyV2) >= 0 ? Versions.WithKeyV2 : Versions.Default
            }
            else
            {
                return Version.Default;
            }
        }

        return Version.Default;
    }

    private enum Version
    {
        Default = 1,
        WithKey = 2,
        WithKeyV2 = 3,
        LazySession = 4
    }
}