﻿using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Events;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Players.Contexts;
using Void.Proxy.Plugins.ForwardingSupport.Velocity.Packets;

namespace Void.Proxy.Plugins.ForwardingSupport.Velocity.Services;

public class ForwardingService(IPlayerContext context, ILogger logger, Settings settings) : IEventListener
{
    [Subscribe]
    public void OnPhaseChanged(PhaseChangedEvent @event)
    {
        if (@event.Phase is not Phase.Login)
            return;

        context.Player.RegisterPacket<LoginPluginResponsePacket>([new(0x02, ProtocolVersion.Oldest)]);
        context.Player.RegisterPacket<LoginPluginRequestPacket>([new(0x04, ProtocolVersion.Oldest)]);

        logger.LogTrace("Registered packet mappings for player {Player} at {Side} side", context.Player, @event.Side);
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
        buffer.WriteString(context.Player.RemoteEndPoint.Split(':')[0]);

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
        var signature = HMACSHA256.HashData(Encoding.UTF8.GetBytes(settings.Secret), forwardingData);

        @event.Cancel();
        await @event.Link.SendPacketAsync(new LoginPluginResponsePacket { Data = [.. signature, .. forwardingData], MessageId = packet.MessageId, Successful = true }, cancellationToken);
    }

    private ForwardingVersion FindForwardingVersion(ForwardingVersion requested)
    {
        requested = (ForwardingVersion)Math.Min((int)requested, Enum.GetValues<ForwardingVersion>().Cast<int>().Max());

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
