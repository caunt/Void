using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Events;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Console;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players.Contexts;
using Void.Proxy.Plugins.ForwardingSupport.Velocity.Packets;

namespace Void.Proxy.Plugins.ForwardingSupport.Velocity.Services;

public class ForwardingService(IPlayerContext context, ILogger logger, IConsoleService console, Plugin plugin, Settings settings) : IEventListener
{
    private static readonly int MaxForwarding = Enum.GetValues<ForwardingVersion>().Cast<int>().Max();

    [Subscribe]
    public void OnPhaseChanged(PhaseChangedEvent @event)
    {
        if (@event.Phase is not Phase.Login)
            return;

        if (@event.Side is Side.Server)
            LoginPluginRequestPacket.Register(context.Player);
        else if (@event.Side is Side.Client)
            LoginPluginResponsePacket.Register(context.Player);
        else
            throw new InvalidOperationException($"Unexpected side {@event.Side}");

        context.Logger.LogTrace("Registered forwarding packet mappings at {Side} side", @event.Side);
    }

    [Subscribe]
    public async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Message is not LoginPluginRequestPacket packet)
            return;

        if (!context.Player.IsMinecraft)
            return;

        if (settings is null)
            throw new Exception("Settings are not initialized yet");

        if (!settings.Enabled)
            return;

        if (packet.Channel is not "velocity:player_info")
            return;

        var requestedVersion = packet.Data.Length == 0 ? ForwardingVersion.Default : (ForwardingVersion)packet.Data[0];
        var actualVersion = FindForwardingVersion(requestedVersion);
        var buffer = new BufferSpan(stackalloc byte[2048]);

        buffer.WriteVarInt((int)actualVersion);

        var remoteEndPoint = context.Player.RemoteEndPoint.AsSpan();
        var colonIndex = remoteEndPoint.IndexOf(':');

        buffer.WriteString(colonIndex >= 0 ? remoteEndPoint[..colonIndex] : remoteEndPoint);

        if (context.Player.Profile is not { } profile)
        {
            logger.LogWarning("Player profile is null, velocity forwarding will not work.");
            return;
        }

        buffer.WriteUuid(profile.Id);
        buffer.WriteString(profile.Username);
        buffer.WritePropertyArray(profile.Properties);

        if (actualVersion >= ForwardingVersion.WithKey && actualVersion < ForwardingVersion.LazySession)
        {
            if (context.Player.IdentifiedKey is not { } identifiedKey)
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

        if (!console.TryGetOptionValue(plugin.ForwardingModernKeyOption, out var secretKey))
            secretKey = settings.Secret;

        var secretLength = Encoding.UTF8.GetByteCount(secretKey);
        Span<byte> secretBytes = stackalloc byte[secretLength];
        Encoding.UTF8.GetBytes(secretKey, secretBytes);
        Span<byte> signature = stackalloc byte[32];

        if (!HMACSHA256.TryHashData(secretBytes, forwardingData, signature, out var written))
        {
            throw new InvalidOperationException("Failed to compute HMAC signature.");
        }

        @event.Cancel();
        await @event.Link.SendPacketAsync(new LoginPluginResponsePacket { Data = [.. signature[..written], .. forwardingData], MessageId = packet.MessageId, Successful = true }, cancellationToken);
    }

    private ForwardingVersion FindForwardingVersion(ForwardingVersion requested)
    {
        requested = (ForwardingVersion)Math.Min((int)requested, MaxForwarding);

        if (requested > ForwardingVersion.Default)
        {
            if (context.Player.ProtocolVersion > ProtocolVersion.MINECRAFT_1_19_3)
                return requested >= ForwardingVersion.LazySession ? ForwardingVersion.LazySession : ForwardingVersion.Default;

            var identifiedKey = context.Player.IdentifiedKey;

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
