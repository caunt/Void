﻿using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using Void.Minecraft.Mojang;
using Void.Minecraft.Players;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Crypto;

namespace Void.Proxy.Plugins.Common.Mojang;

public class MojangService(ICryptoService crypto) : IMojangService
{
    private static readonly HttpClient Client = new();
    private static readonly string SessionServer = Environment.GetEnvironmentVariable("mojang.sessionserver") ?? "https://sessionserver.mojang.com/session/minecraft/hasJoined";
    private static readonly bool PreventProxyConnections = bool.TryParse(Environment.GetEnvironmentVariable("mojang.prevent-proxy-connections"), out var value) && value;

    public async ValueTask<GameProfile?> VerifyAsync(IMinecraftPlayer player, ReadOnlyMemory<byte> secret, CancellationToken cancellationToken = default)
    {
        if (player.Profile is null)
            throw new ArgumentNullException(nameof(player), "Player profile should be set in order to verify his session");

        var serverId = SHA1.HashData([.. secret.Span, .. crypto.Instance.ExportSubjectPublicKeyInfo()]);
        var negative = (serverId[0] & 0x80) == 0x80;

        if (negative)
            serverId = TwosComplement(serverId);

        var serverIdComplement = Convert.ToHexString(serverId).TrimStart('0');

        if (negative)
            serverIdComplement = "-" + serverIdComplement;

        var url = $"{SessionServer}?username={player.Profile.Username}&serverId={serverIdComplement}";

        if (PreventProxyConnections)
            url += "&ip=" + player.RemoteEndPoint;

        var response = await Client.GetAsync(url, cancellationToken);

        if (response.StatusCode is HttpStatusCode.NoContent)
            return null;

        if (await response.Content.ReadFromJsonAsync<GameProfile>(cancellationToken) is not { } profile)
            return null;

        if (player.IdentifiedKey is null || player.IdentifiedKey.Revision != IdentifiedKeyRevision.LinkedV2Revision)
            return profile;

        return player.IdentifiedKey.AddUuid(profile.Id) ? profile : null;
    }

    private static byte[] TwosComplement(byte[] data)
    {
        var carry = true;

        for (var i = data.Length - 1; i >= 0; i--)
        {
            data[i] = unchecked((byte)~data[i]);

            if (!carry)
                continue;

            carry = data[i] == 0xFF;
            data[i]++;
        }

        return data;
    }
}
