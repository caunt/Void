using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Minecraft.Profiles;
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
        var buffer = new BufferSpan(stackalloc byte[2048]);

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

        if (actualVersion >= ForwardingVersion.WithKey && actualVersion < ForwardingVersion.LazySession)
        {
            if (player.IdentifiedKey is not { } identifiedKey)
                throw new Exception("Player identified key cannot be forwarded");

            buffer.WriteLong(identifiedKey.ExpiresAt);
            buffer.WriteVarInt(identifiedKey.PublicKey.Length);
            buffer.Write(identifiedKey.PublicKey);
            buffer.WriteVarInt(identifiedKey.Signature.Length);
            buffer.Write(identifiedKey.Signature);

            if (actualVersion >= ForwardingVersion.WithKeyV2)
            {
                if (identifiedKey.ProfileUuid != default)
                {
                    buffer.WriteBoolean(true);
                    buffer.WriteUuid(identifiedKey.ProfileUuid);
                }
                else
                {
                    buffer.WriteBoolean(false);
                }
            }
        }

        var forwardingData = buffer.Access(0, buffer.Position);
        var signature = HMACSHA256.HashData(Encoding.UTF8.GetBytes(_settings.Secret), forwardingData);

        @event.Result = [.. signature, .. forwardingData];
    }


    private static ForwardingVersion FindForwardingVersion(IPlayer player, ForwardingVersion requested)
    {
        requested = (ForwardingVersion)Math.Min((int)requested, Enum.GetValues<ForwardingVersion>().Cast<int>().Max());

        if (requested > ForwardingVersion.Default)
        {
            if (player.ProtocolVersion > ProtocolVersion.MINECRAFT_1_19_3)
                return requested >= ForwardingVersion.LazySession ? ForwardingVersion.LazySession : ForwardingVersion.Default;

            var identifiedKey = player.IdentifiedKey;

            if (identifiedKey is not null)
            {
                if (identifiedKey.Revision == IdentifiedKeyRevision.GenericV1Revision)
                    return ForwardingVersion.WithKey;

                if (identifiedKey.Revision == IdentifiedKeyRevision.LinkedV2Revision)
                    return requested >= ForwardingVersion.WithKeyV2 ? ForwardingVersion.WithKeyV2 : ForwardingVersion.Default;

                throw new NotSupportedException($"Unknown key revision {identifiedKey.Revision}");
            }
            else
            {
                return ForwardingVersion.Default;
            }
        }

        return ForwardingVersion.Default;
    }
}
