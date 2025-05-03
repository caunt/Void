using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Plugins;

namespace Void.Proxy.Plugins.ForwardingSupport.Velocity;

public class Plugin(ILogger logger) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.Latest);

    public string Name => nameof(Velocity);

    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != this)
            return;
    }

    [Subscribe]
    public void OnLoginPluginRequest(LoginPluginRequestEvent @event)
    {
        if (!@event.Player.IsMinecraft)
            return;

        var player = @event.Player;

        if (@event.Channel is not "velocity:player_info")
            return;

        var requestedVersion = @event.Data.Length == 0 ? ForwardingVersion.Default : (ForwardingVersion)@event.Data[0];
        var actualVersion = FindForwardingVersion(player, requestedVersion);
        var array = (Span<byte>)stackalloc byte[2048];
        var buffer = new MinecraftBuffer(array);

        buffer.WriteVarInt((int)actualVersion);
        buffer.WriteString(player.RemoteEndPoint.Split(':')[0]);

        if (player.Profile is not { } profile)
        {
            logger.LogWarning("Player profile is null, velocity forwarding will not work.");
            return;
        }

        buffer.WriteUuid(profile.Id);
        buffer.WriteString(profile.Username);
        buffer.WritePropertyArray(profile.Properties);

        if (actualVersion.CompareTo(ForwardingVersion.WithKey) >= 0 && actualVersion.CompareTo(ForwardingVersion.LazySession) < 0)
        {
            if (player.IdentifiedKey is not { } identifiedKey)
                throw new Exception("Player identified key cannot be forwarded");

            buffer.WriteLong(identifiedKey.ExpiresAt);
            buffer.WriteVarInt(identifiedKey.PublicKey.Length);
            buffer.Write(identifiedKey.PublicKey);
            buffer.WriteVarInt(identifiedKey.Signature.Length);
            buffer.Write(identifiedKey.Signature);

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

    private static ForwardingVersion FindForwardingVersion(IPlayer player, ForwardingVersion requested)
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
