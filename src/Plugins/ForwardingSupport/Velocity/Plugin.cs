using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Plugins;

namespace Void.Proxy.Plugins.ForwardingSupport.Velocity;

public class Plugin(ILogger logger, IConfigurationService configs) : IProtocolPlugin
{
    private Settings? _settings;

    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.Latest);

    public string Name => nameof(Velocity);

    [Subscribe]
    public async ValueTask OnPluginLoading(PluginLoadingEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != this)
            return;

        _settings = await configs.GetAsync<Settings>(cancellationToken);

        if (string.IsNullOrWhiteSpace(_settings.Secret))
            _settings.Secret = Guid.NewGuid().ToString("N");
    }

    [Subscribe]
    public void OnLoginPluginRequest(LoginPluginRequestEvent @event)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (_settings is null)
            throw new Exception("Settings are not initialized yet");

        var player = @event.Player;

        if (@event.Channel is not "velocity:player_info")
            return;

        var requestedVersion = @event.Data.Length == 0 ? ForwardingVersion.Default : (ForwardingVersion)@event.Data[0];
        var actualVersion = FindForwardingVersion(player, requestedVersion);
        var array = (Span<byte>)stackalloc byte[2048];
        var buffer = new BufferSpan(array);

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

        var forwardingData = array[..buffer.Position];
        var signature = HMACSHA256.HashData(Encoding.UTF8.GetBytes(_settings.Secret), forwardingData);

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
