using System.Security.Cryptography;
using System.Text;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Plugins;

namespace Void.Proxy.Plugins.ForwardingSupport.Velocity;

public class Plugin : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.Latest);

    public string Name => nameof(Velocity);

    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event)
    {
        if (@event.Plugin != this)
            return;
    }

    [Subscribe]
    public static void OnLoginPluginRequest(LoginPluginRequestEvent @event)
    {
        if (!@event.Link.Player.TryGetMinecraftPlayer(out var player))
            return;

        if (@event.Channel is not "velocity:player_info")
            return;

        var requestedVersion = @event.Data.Length == 0 ? ForwardingVersion.Default : (ForwardingVersion)@event.Data[0];
        var actualVersion = FindForwardingVersion(player, requestedVersion);
        var array = new byte[2048];
        var buffer = new MinecraftBuffer(array);

        buffer.WriteVarInt((int)actualVersion);
        buffer.WriteString(player.RemoteEndPoint.Split(':')[0]);
        buffer.WriteUuid(player.Profile!.Id);
        buffer.WriteString(player.Profile.Username);
        buffer.WritePropertyArray(player.Profile.Properties);

        if (actualVersion.CompareTo(ForwardingVersion.WithKey) >= 0 && actualVersion.CompareTo(ForwardingVersion.LazySession) < 0)
        {
            if (player.IdentifiedKey is null)
                throw new Exception("Player identified key cannot be forwarded");

            buffer.WriteLong(player.IdentifiedKey.ExpiresAt);
            buffer.WriteVarInt(player.IdentifiedKey.PublicKey.Length);
            buffer.Write(player.IdentifiedKey.PublicKey);
            buffer.WriteVarInt(player.IdentifiedKey.Signature.Length);
            buffer.Write(player.IdentifiedKey.Signature);

            if (actualVersion.CompareTo(ForwardingVersion.WithKeyV2) >= 0)
                // if key signature holder is not null (seems to be always null)
                // WriteBoolean(true)
                // WriteGuid(key.GetSignatureHolder())
                // else
                buffer.WriteBoolean(false);
        }

        var forwardingData = array[..(int)buffer.Position];
        var signature = HMACSHA256.HashData(Encoding.UTF8.GetBytes("aaa"), forwardingData);

        @event.Result = [.. signature, .. forwardingData];
    }

    private static ForwardingVersion FindForwardingVersion(IMinecraftPlayer player, ForwardingVersion requested)
    {
        requested = (ForwardingVersion)Math.Min((int)requested, Enum.GetValues<ForwardingVersion>().Cast<int>().Max());

        if (requested.CompareTo(ForwardingVersion.Default) > 0)
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
                return ForwardingVersion.Default;
            }
        }

        return ForwardingVersion.Default;
    }
}
